using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using QuizAutoSolver.Properties;

namespace QuizAutoSolver
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        private void RegisterGlobalHotKey(Keys SecondKey, int MainKey)
        {
            if (frmMain.RegisterHotKey(base.Handle, (int)base.Handle, MainKey, (int)SecondKey) == 0)
            {
                if (Marshal.GetLastWin32Error().ToString() == "1409")
                {
                    MessageBox.Show("Impossibile registrare un Hot Key per via di un conflitto", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    returncode = 1;
                    return;
                }
                MessageBox.Show("Impossibile registrare un Hot Key. Codice errore: " + Marshal.GetLastWin32Error().ToString(), "Errore", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                frmMain.UnregisterHotKey(base.Handle, (int)base.Handle);
                base.Close();
            }
            returncode = 0;
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboMainKey.Text = Settings.Default.MainKey;
            cboSecondKey.Text = Settings.Default.SecondKey;

            RegisterGlobalHotKey((Keys)Enum.Parse(typeof(Keys), cboSecondKey.Text, true), cboMainKeyToInt(cboMainKey.Text));

            foreach (string text in Environment.GetCommandLineArgs())
            {
                if (text.Substring(1, text.Length - 1) == "startup")
                {
                    Startup = true;
                }
            }
        }

        int cboMainKeyToInt(string MainKey)
        {
            switch (MainKey)
            {
                case "Alt":
                    return 1;
                case "Ctrl":
                    return 2;
                case "Shift":
                    return 4;
                case "WinKey":
                    return 8;
            }
            return 0;
        }

        string GetQuestion(string path_read)
        {
            const Int32 BufferSize = 128;
            string UsefulLine = null;
            int UsefulLineInt = -1;
            using (var fileStream = File.OpenRead(path_read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                int line_count = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line != null && line.Length > 17)
                    {
                        if (line.Substring(line.Length - 15, 15) == "<div id=\"qdiv\">")
                        {
                            UsefulLineInt = line_count + 1;
                        }
                    }
                    if (line_count == UsefulLineInt)
                    {
                        UsefulLine = line;
                        if (UsefulLine.Substring(UsefulLine.Length - 6, 6) == "</div>")
                        {
                            UsefulLine = UsefulLine.Substring(0, UsefulLine.Length - 6);
                        }
                        break;
                    }
                    line_count++;
                }
            }
            return UsefulLine;
        }

        string CleanQuestion(string old)
        {
            string[] temp;
            temp = Regex.Replace(old, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            return temp[temp.Count() - 1];
        }

        string ToMask(string Shorthand)
        {
            int num_SH;
            string mask = null;
            int[] num = new int[4];
            for (int i = 0; i < 4; i++)
            {
                num[i] = 0;
            }
            Int32.TryParse(Shorthand, out num_SH);
            int cnt = 0;
            while (num_SH >= 8)
            {
                num[cnt] = 255;
                num_SH -= 8;
                cnt++;
            }
            //si poteva fare un simpatica funzione ricorsiva ma così è più leggibile
            switch (num_SH)
            {
                case 0:
                    num[cnt] = 0;
                    break;
                case 1:
                    num[cnt] = 128;
                    break;
                case 2:
                    num[cnt] = 192;
                    break;
                case 3:
                    num[cnt] = 224;
                    break;
                case 4:
                    num[cnt] = 240;
                    break;
                case 5:
                    num[cnt] = 248;
                    break;
                case 6:
                    num[cnt] = 252;
                    break;
                case 7:
                    num[cnt] = 254;
                    break;
            }
            mask = num[0] + "." + num[1] + "." + num[2] + "." + num[3];
            return mask;
        }

        string ToBin(string input)
        {
            return String.Join(".", (input.Split('.').Select(x => Convert.ToString(Int32.Parse(x), 2).PadLeft(8, '0'))).ToArray());
        }

        string ToDec(string input)
        {
            string[] ip = new string[4];
            ip[0] = Convert.ToInt32(input.Substring(0, 8), 2).ToString();
            ip[1] = Convert.ToInt32(input.Substring(9, 8), 2).ToString();
            ip[2] = Convert.ToInt32(input.Substring(18, 8), 2).ToString();
            ip[3] = Convert.ToInt32(input.Substring(27, 8), 2).ToString();
            return ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
        }

        string DefaultMask(string ip)
        {
            string defaultMask = null;
            string BinIP = ToBin(ip);

            if (BinIP.Substring(0, 3) == "110")
            {
                defaultMask = "255.255.255.0";
            }
            else
            if (BinIP.Substring(0, 2) == "10")
            {
                defaultMask = "255.255.0.0";
            }
            else
            if (BinIP.Substring(0, 1) == "0")
            {
                defaultMask = "255.0.0.0";
            }
            return defaultMask;
        }

        string AndInBin(string ip, string mask)
        {
            StringBuilder BinAnd = new StringBuilder("00000000.00000000.00000000.00000000");
            string Bin;
            string BinIp = ToBin(ip);
            string BinMask = ToBin(mask);

            for (int j = 0; j < BinIp.Length; j++)
            {
                if (BinIp[j] == '1' && BinMask[j] == '1')
                {
                    BinAnd[j] = '1';
                }
            }
            Bin = ToDec(BinAnd.ToString());
            return Bin;
        }

        string FirstHost(string ip, string mask)
        {
            StringBuilder BinAnd = new StringBuilder("00000000.00000000.00000000.00000000");
            string Bin;
            string BinIp = ToBin(ip);
            string BinMask = ToBin(mask);

            for (int j = 0; j < BinIp.Length; j++)
            {
                if (BinIp[j] == '1' && BinMask[j] == '1')
                {
                    BinAnd[j] = '1';
                }
            }
            Bin = ToDec(BinAnd.ToString());

            int cnt = 0;
            int value;
            for (int j = 0; j < Bin.Length; j++)
            {
                if (Bin[j] == '.')
                {
                    cnt++;
                    if (cnt == 3)
                    {
                        int.TryParse(Bin.Substring(j + 1, Bin.Length - j - 1), out value);

                        Bin = Bin.Substring(0, j + 1);
                        Bin += (value + 1).ToString();
                    }
                }
            }
            return Bin;
        }

        string LastHost(string ip, string mask)
        {
            string Bin = Broadcast(ip, mask);

            int cnt = 0;
            int value;
            for (int j = 0; j < Bin.Length; j++)
            {
                if (Bin[j] == '.')
                {
                    cnt++;
                    if (cnt == 3)
                    {
                        int.TryParse(Bin.Substring(j + 1, Bin.Length - j - 1), out value);

                        Bin = Bin.Substring(0, j + 1);
                        Bin += (value - 1).ToString();
                    }
                }
            }

            return Bin;
        }

        string Broadcast(string ip, string mask)
        {
            StringBuilder BinAnd = new StringBuilder("00000000.00000000.00000000.00000000");
            string Bin;
            string BinIp = ToBin(ip);
            string BinMask = ToBin(mask);
            int cnt0mask = 0;

            for (int j = 0; j < BinIp.Length; j++)
            {
                if (BinIp[j] == '1' && BinMask[j] == '1')
                {
                    BinAnd[j] = '1';
                }
                if (BinMask[j] == '1')
                {
                    cnt0mask++;
                    if (cnt0mask == 8)
                    {
                        cnt0mask++;
                    }
                    if (cnt0mask == 17)
                    {
                        cnt0mask++;
                    }
                    if (cnt0mask == 26)
                    {
                        cnt0mask++;
                    }
                }
            }

            for (int i = cnt0mask; i < 35; i++)
            {
                BinAnd[i] = '1';
            }

            BinAnd[8] = '.';
            BinAnd[17] = '.';
            BinAnd[26] = '.';

            Bin = ToDec(BinAnd.ToString());

            return Bin;
        }

        string QuantitaHost(string mask)
        {
            int cnt = 0;
            string BinMask = ToBin(mask);
            for (int i = 0; i < BinMask.Length; i++)
            {
                if (BinMask[i] == '1')
                {
                    cnt++;
                }
            }
            cnt = 32 - cnt;
            return (Math.Pow(2.0, cnt) - 2).ToString();
        }

        void Reply(string question)
        {

            if (question.Length < 67)
            {
                //FATTO
                if (question.Substring(0, 33) == "Quanti host ci sono nella subnet ")
                {
                    string rimanente = question.Substring(33, question.Length - 33);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 2);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            string quantitahost = QuantitaHost(mask);
                            Clipboard.SetText(quantitahost);
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 39) == "Quante subnet puoi ottenere dalla rete ")
                {
                    string rimanente = question.Substring(39, question.Length - 39);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 2);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);

                            string BinMask = ToBin(mask);
                            string BinDefaultMask = ToBin(DefaultMask(ip));

                            int cnt;
                            int cnt_bin1 = 0;
                            int cnt_bin2 = 0;
                            for (int j = 0; j < BinMask.Length; j++)
                            {
                                if (BinMask[j] == '1')
                                {
                                    cnt_bin1++;
                                }
                                if (BinDefaultMask[j] == '1')
                                {
                                    cnt_bin2++;
                                }
                            }
                            cnt = cnt_bin1 - cnt_bin2;
                            cnt = (int)Math.Pow(2.0, cnt);
                            Clipboard.SetText(cnt.ToString());
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 42) == "Inserisci l'ultimo host valido della rete ")
                {
                    string rimanente = question.Substring(42, question.Length - 42);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            Clipboard.SetText(LastHost(ip, mask));
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 42) == "Inserisci il primo host valido della rete ")
                {
                    string rimanente = question.Substring(42, question.Length - 42);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);

                            Clipboard.SetText(FirstHost(ip, mask));
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 43) == "Inserisci l'indirizzo di broadcast dell'IP ")
                {
                    string rimanente = question.Substring(43, question.Length - 43);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            Clipboard.SetText(Broadcast(ip, mask));
                        }
                    }
                }
                else
                if (question.Substring(0, 47) == "Quante subnet USABILI puoi ottenere dalla rete ")
                {
                    string rimanente = question.Substring(47, question.Length - 47);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 2);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            MessageBox.Show("shorthand = /" + shorthand + "\n" + "ip = " + ip + "\n" + "mask = " + mask);
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 50) == "Qual è la maschera corrispondente alla shorthand /")
                {
                    string shorthand = question.Substring(50, question.Length - 51);
                    string mask = ToMask(shorthand);
                    Clipboard.SetText(mask);
                }
            }
            else
            //FATTO
            if (question.Substring(0, 43) == "Inserisci l'indirizzo di broadcast dell'IP ")
            {
                string rimanente = question.Substring(43, question.Length - 43);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string ip = rimanente.Substring(0, i);
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                            Clipboard.SetText(Broadcast(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            if (question.Length < 93)
            {
                //FATTO
                if (question.Substring(0, 39) == "Quante subnet puoi ottenere dalla rete ")
                {
                    string rimanente = question.Substring(39, question.Length - 39);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        try
                        {
                            if (rimanente.Substring(i, 14) == " con maschera ")
                            {
                                string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 15);
                                string ip = rimanente.Substring(0, i);
                                string BinMask = ToBin(mask);
                                string BinDefaultMask = ToBin(DefaultMask(ip));

                                int cnt;
                                int cnt_bin1 = 0;
                                int cnt_bin2 = 0;
                                for (int j = 0; j < BinMask.Length; j++)
                                {
                                    if (BinMask[j] == '1')
                                    {
                                        cnt_bin1++;
                                    }
                                    if (BinDefaultMask[j] == '1')
                                    {
                                        cnt_bin2++;
                                    }
                                }
                                cnt = cnt_bin1 - cnt_bin2;
                                cnt = (int)Math.Pow(2.0, cnt);
                                Clipboard.SetText(cnt.ToString());
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 33) == "Quanti host ci sono nella subnet ")
                {
                    string rimanente = question.Substring(33, question.Length - 33);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        try
                        {
                            if (rimanente.Substring(i, 14) == " con maschera ")
                            {
                                string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 15);
                                string ip = rimanente.Substring(0, i);
                                string quantitahost = QuantitaHost(mask);
                                Clipboard.SetText(quantitahost);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
                if (question.Substring(0, 42) == "Inserisci l'ultimo host valido della rete " && question.Length < 87)
                {
                    string rimanente = question.Substring(42, question.Length - 42);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        try
                        {
                            if (rimanente.Substring(i, 14) == " con maschera ")
                            {
                                string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                                string ip = rimanente.Substring(0, i);
                                Clipboard.SetText(LastHost(ip, mask));
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 42) == "Inserisci il primo host valido della rete " && question.Length < 87)
                {
                    string rimanente = question.Substring(42, question.Length - 42);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        try
                        {
                            if (rimanente.Substring(i, 14) == " con maschera ")
                            {
                                string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                                string ip = rimanente.Substring(0, i);

                                Clipboard.SetText(FirstHost(ip, mask));
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                if (question.Substring(0, 47) == "Quante subnet USABILI puoi ottenere dalla rete ")
                {
                    string rimanente = question.Substring(47, question.Length - 47);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        try
                        {
                            if (rimanente.Substring(i, 14) == " con maschera ")
                            {
                                MessageBox.Show("mask = " + rimanente.Substring(i + 14, (rimanente.Length - i) - 15)
                                    + "\n" + "ip = " + rimanente.Substring(0, i)
                                    );
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 59) == "Quanti bit iniziali posti a 1 sono presenti nella maschera ")
                {
                    string binary = ToBin(question.Substring(59, question.Length - 60));
                    int cnt_bin = 0;
                    for (int i = 0; i < binary.Length; i++)
                    {
                        if (binary[i] == '1')
                        {
                            cnt_bin++;
                        }
                    }

                    Clipboard.SetText(cnt_bin.ToString());
                }
                else
                //FATTO
                if (question.Substring(0, 64) == "Inserisci l'indirizzo della subnet alla quale appartiene l'host ")
                {
                    string rimanente = question.Substring(71, question.Length - 71);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            Clipboard.SetText(AndInBin(ip, mask));
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 69) == "Inserisci l'indirizzo della subnet alla quale appartiene l'indirizzo ")
                {
                    string rimanente = question.Substring(69, question.Length - 69);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            Clipboard.SetText(AndInBin(ip, mask));
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 71) == "Inserisci il primo host valido della rete alla quale appartiene l'host ")
                {
                    string rimanente = question.Substring(71, question.Length - 71);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);

                            Clipboard.SetText(FirstHost(ip, mask));
                        }
                    }
                }
                else
                //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
                if (question.Substring(0, 71) == "Inserisci l'ultimo host valido della rete alla quale appartiene l'host ")
                {
                    string rimanente = question.Substring(71, question.Length - 71);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                        string ip = rimanente.Substring(0, i);
                        string mask = ToMask(shorthand);
                        Clipboard.SetText(LastHost(ip, mask));
                    }
                }
                else
                //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
                if (question.Substring(0, 76) == "Inserisci l'ultimo host valido della rete alla quale appartiene l'indirizzo ")
                {
                    string rimanente = question.Substring(76, question.Length - 76);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 2);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);
                            Clipboard.SetText(LastHost(ip, mask));
                        }
                    }
                }
                else
                //FATTO
                if (question.Substring(0, 76) == "Inserisci il primo host valido della rete alla quale appartiene l'indirizzo ")
                {
                    string rimanente = question.Substring(76, question.Length - 76);
                    for (int i = 0; i < rimanente.Length; i++)
                    {
                        if (rimanente[i] == '/')
                        {
                            string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                            string ip = rimanente.Substring(0, i);
                            string mask = ToMask(shorthand);

                            Clipboard.SetText(FirstHost(ip, mask));
                        }
                    }
                }

            }
            else
            //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
            if (question.Substring(0, 76) == "Inserisci l'ultimo host valido della rete alla quale appartiene l'indirizzo " && question.Length < 95)
            {
                string rimanente = question.Substring(76, question.Length - 76);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    if (rimanente[i] == '/')
                    {
                        string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 2);
                        string ip = rimanente.Substring(0, i);
                        string mask = ToMask(shorthand);
                        Clipboard.SetText(LastHost(ip, mask));
                    }
                }
            }
            else
            //FATTO
            if (question.Substring(0, 76) == "Inserisci il primo host valido della rete alla quale appartiene l'indirizzo " && question.Length < 95)
            {
                string rimanente = question.Substring(76, question.Length - 76);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    if (rimanente[i] == '/')
                    {
                        string shorthand = rimanente.Substring(i + 1, (rimanente.Length - i) - 1);
                        string ip = rimanente.Substring(0, i);
                        string mask = ToMask(shorthand);

                        Clipboard.SetText(FirstHost(ip, mask));
                    }
                }
            }
            else
            //FATTO
            if (question.Substring(0, 64) == "Inserisci l'indirizzo della subnet alla quale appartiene l'host ")
            {
                string rimanente = question.Substring(71, question.Length - 71);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string mask = rimanente.Substring(i + 14 + 1, (rimanente.Length - i) - 15);
                            string ip = rimanente.Substring(0, i);

                            Clipboard.SetText(AndInBin(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            //FATTO
            if (question.Substring(0, 69) == "Inserisci l'indirizzo della subnet alla quale appartiene l'indirizzo ")
            {
                string rimanente = question.Substring(69, question.Length - 69);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                            string ip = rimanente.Substring(0, i);
                            Clipboard.SetText(AndInBin(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            //FATTO
            if (question.Substring(0, 71) == "Inserisci il primo host valido della rete alla quale appartiene l'host ")
            {
                string rimanente = question.Substring(71, question.Length - 71);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                            string ip = rimanente.Substring(0, i);

                            Clipboard.SetText(FirstHost(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
            if (question.Substring(0, 71) == "Inserisci l'ultimo host valido della rete alla quale appartiene l'host ")
            {
                string rimanente = question.Substring(71, question.Length - 71);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                            string ip = rimanente.Substring(0, i);
                            Clipboard.SetText(LastHost(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            //FATTO
            if (question.Substring(0, 76) == "Inserisci il primo host valido della rete alla quale appartiene l'indirizzo ")
            {
                string rimanente = question.Substring(76, question.Length - 76);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string ip = rimanente.Substring(0, i);
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);

                            Clipboard.SetText(FirstHost(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            //FATTO MA A VOLTE POTREBBE NON FUNZIONARE
            if (question.Substring(0, 76) == "Inserisci l'ultimo host valido della rete alla quale appartiene l'indirizzo ")
            {
                string rimanente = question.Substring(76, question.Length - 76);
                for (int i = 0; i < rimanente.Length; i++)
                {
                    try
                    {
                        if (rimanente.Substring(i, 14) == " con maschera ")
                        {
                            string mask = rimanente.Substring(i + 14, (rimanente.Length - i) - 14);
                            string ip = rimanente.Substring(0, i);
                            Clipboard.SetText(LastHost(ip, mask));
                        }
                    }
                    catch
                    {

                    }
                }
            }

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 786)
            {
                int filecount = 1;
                IntPtr ctrlPtr = GetForegroundWindow();
                SetForegroundWindow(ctrlPtr);
                uint p_id;
                GetWindowThreadProcessId(ctrlPtr, out p_id);
                string browser = (Process.GetProcessById((int)p_id).MainModule.ModuleName);
                if (radioButton2.Checked)
                {
                    string question = (string)Clipboard.GetDataObject().GetData(DataFormats.Text);
                    question = CleanQuestion(question);
                    try
                    {
                        Reply(question);
                    }
                    catch
                    {

                    }
                }
                else
                if (radioButton1.Checked)
                {
                    if (browser == "chrome.exe" || browser == "firefox.exe")
                    {
                        if (radioButton1.Checked)
                        {

                            Thread.Sleep((int)numericUpDown1.Value);
                            SendKeys.SendWait("^s");
                            Thread.Sleep((int)numericUpDown1.Value);
                            for (int i = 0; i < 6; i++)
                            {
                                SendKeys.SendWait("{TAB}");
                                Thread.Sleep(50);
                            }
                            SendKeys.SendWait("{ENTER}");
                            SendKeys.SendWait(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                            SendKeys.SendWait("{ENTER}");
                            for (int i = 0; i < 6; i++)
                            {
                                SendKeys.SendWait("{TAB}");
                                Thread.Sleep(50);
                            }
                            SendKeys.SendWait(filecount.ToString());
                            SendKeys.SendWait(".html");
                            SendKeys.SendWait("{ENTER}");

                            Thread.Sleep((int)numericUpDown1.Value);
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + filecount + ".html";
                            string question = GetQuestion(path);
                            File.Delete(path);
                            question = CleanQuestion(question);
                            Reply(question);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Solo Firefox e Chrome sono supportati.");
                    }
                }
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmMain.UnregisterHotKey(base.Handle, (int)base.Handle);
            niMain.Visible = false;
        }

        private void cboMainKey_TextChanged(object sender, EventArgs e)
        {
            if (!cboMainKey.Items.Contains(cboMainKey.Text))
            {
                cboMainKey.Text = "WinKey";
            }
            Settings.Default.MainKey = cboMainKey.Text;
            Settings.Default.Save();
        }

        private void cboSecondKey_TextChanged(object sender, EventArgs e)
        {
            if (!cboSecondKey.Items.Contains(cboSecondKey.Text))
            {
                cboSecondKey.Text = "A";
            }
            Settings.Default.SecondKey = cboSecondKey.Text;
            Settings.Default.Save();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == base.WindowState)
            {
                base.Hide();
            }
        }

        private void niMain_Click(object sender, EventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            frmMain.UnregisterHotKey(base.Handle, (int)base.Handle);
            RegisterGlobalHotKey((Keys)Enum.Parse(typeof(Keys), cboSecondKey.Text, true), cboMainKeyToInt(cboMainKey.Text));

            if (returncode != 1)
            {
                base.Hide();
                tbMain.SelectedIndex = 0;
                return;
            }
            cboMainKey.Text = "Alt";
            cboSecondKey.Text = "A";
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Show();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (!Startup)
            {
                base.WindowState = FormWindowState.Normal;
            }
        }

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 524288;
        public const int WS_EX_TRANSPARENT = 32;
        public const int LWA_ALPHA = 2;
        private const int MOD_ALT = 1;
        private const int MOD_CONTROL = 2;
        private const int MOD_SHIFT = 4;
        private const int MOD_WIN = 8;
        private int returncode;
        private bool Startup;
    }
}
