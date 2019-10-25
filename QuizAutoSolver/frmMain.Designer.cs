namespace QuizAutoSolver
{
    public partial class frmMain : global::System.Windows.Forms.Form
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrTest = new System.Windows.Forms.Timer(this.components);
            this.btnActivate = new System.Windows.Forms.Button();
            this.tbMain = new System.Windows.Forms.TabControl();
            this.tbHotKey = new System.Windows.Forms.TabPage();
            this.lblWarning = new System.Windows.Forms.Label();
            this.cboSecondKey = new System.Windows.Forms.ComboBox();
            this.lblPlus = new System.Windows.Forms.Label();
            this.cboMainKey = new System.Windows.Forms.ComboBox();
            this.lblHotKey = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuPopUp = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbMain.SuspendLayout();
            this.tbHotKey.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.mnuPopUp.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrTest
            // 
            this.tmrTest.Enabled = true;
            // 
            // btnActivate
            // 
            this.btnActivate.Location = new System.Drawing.Point(379, 148);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(109, 23);
            this.btnActivate.TabIndex = 0;
            this.btnActivate.Text = "Attiva e nascondi";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // tbMain
            // 
            this.tbMain.Controls.Add(this.tbHotKey);
            this.tbMain.Controls.Add(this.tabPage1);
            this.tbMain.Location = new System.Drawing.Point(12, 12);
            this.tbMain.Name = "tbMain";
            this.tbMain.SelectedIndex = 0;
            this.tbMain.Size = new System.Drawing.Size(482, 130);
            this.tbMain.TabIndex = 1;
            // 
            // tbHotKey
            // 
            this.tbHotKey.BackColor = System.Drawing.Color.White;
            this.tbHotKey.Controls.Add(this.lblWarning);
            this.tbHotKey.Controls.Add(this.cboSecondKey);
            this.tbHotKey.Controls.Add(this.lblPlus);
            this.tbHotKey.Controls.Add(this.cboMainKey);
            this.tbHotKey.Controls.Add(this.lblHotKey);
            this.tbHotKey.Location = new System.Drawing.Point(4, 22);
            this.tbHotKey.Name = "tbHotKey";
            this.tbHotKey.Padding = new System.Windows.Forms.Padding(3);
            this.tbHotKey.Size = new System.Drawing.Size(474, 104);
            this.tbHotKey.TabIndex = 0;
            this.tbHotKey.Text = "Tasti";
            this.tbHotKey.UseVisualStyleBackColor = true;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Location = new System.Drawing.Point(6, 74);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(431, 13);
            this.lblWarning.TabIndex = 4;
            this.lblWarning.Text = "Attempts to create Hot Keys that conflict with the system or other applications w" +
    "ill not work";
            // 
            // cboSecondKey
            // 
            this.cboSecondKey.FormattingEnabled = true;
            this.cboSecondKey.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.cboSecondKey.Location = new System.Drawing.Point(107, 39);
            this.cboSecondKey.Name = "cboSecondKey";
            this.cboSecondKey.Size = new System.Drawing.Size(46, 21);
            this.cboSecondKey.TabIndex = 3;
            this.cboSecondKey.Text = "F12";
            this.cboSecondKey.TextChanged += new System.EventHandler(this.cboSecondKey_TextChanged);
            // 
            // lblPlus
            // 
            this.lblPlus.AutoSize = true;
            this.lblPlus.Location = new System.Drawing.Point(88, 43);
            this.lblPlus.Name = "lblPlus";
            this.lblPlus.Size = new System.Drawing.Size(13, 13);
            this.lblPlus.TabIndex = 2;
            this.lblPlus.Text = "+";
            // 
            // cboMainKey
            // 
            this.cboMainKey.FormattingEnabled = true;
            this.cboMainKey.Items.AddRange(new object[] {
            "Alt",
            "Ctrl",
            "Shift",
            "WinKey"});
            this.cboMainKey.Location = new System.Drawing.Point(9, 39);
            this.cboMainKey.Name = "cboMainKey";
            this.cboMainKey.Size = new System.Drawing.Size(73, 21);
            this.cboMainKey.TabIndex = 1;
            this.cboMainKey.Text = "WinKey";
            this.cboMainKey.TextChanged += new System.EventHandler(this.cboMainKey_TextChanged);
            // 
            // lblHotKey
            // 
            this.lblHotKey.AutoSize = true;
            this.lblHotKey.Location = new System.Drawing.Point(6, 13);
            this.lblHotKey.Name = "lblHotKey";
            this.lblHotKey.Size = new System.Drawing.Size(242, 13);
            this.lblHotKey.TabIndex = 0;
            this.lblHotKey.Text = "Imposta la combinazione per identificare il browser";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(474, 104);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Impostazioni";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tempi di attesa fra un\'azione e l\'altra del salva file (ms):";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(273, 51);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(66, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 29);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(135, 17);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Da testo che copi (tutti)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 6);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(171, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.Text = "Da salva file (Chrome e Firefox)";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // niMain
            // 
            this.niMain.Text = "Peek Through";
            this.niMain.Visible = true;
            this.niMain.Click += new System.EventHandler(this.niMain_Click);
            // 
            // mnuPopUp
            // 
            this.mnuPopUp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.mnuPopUp.Name = "mnuPopUp";
            this.mnuPopUp.Size = new System.Drawing.Size(102, 48);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 178);
            this.Controls.Add(this.tbMain);
            this.Controls.Add(this.btnActivate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subnet quiz resolver";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.tbMain.ResumeLayout(false);
            this.tbHotKey.ResumeLayout(false);
            this.tbHotKey.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.mnuPopUp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private global::System.ComponentModel.IContainer components;
        private global::System.Windows.Forms.Timer tmrTest;
        private global::System.Windows.Forms.Button btnActivate;
        private global::System.Windows.Forms.TabControl tbMain;
        private global::System.Windows.Forms.TabPage tbHotKey;
        private global::System.Windows.Forms.Label lblHotKey;
        private global::System.Windows.Forms.ComboBox cboSecondKey;
        private global::System.Windows.Forms.Label lblPlus;
        private global::System.Windows.Forms.ComboBox cboMainKey;
        private global::System.Windows.Forms.Label lblWarning;
        private global::System.Windows.Forms.NotifyIcon niMain;
        private global::System.Windows.Forms.ContextMenuStrip mnuPopUp;
        private global::System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private global::System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
