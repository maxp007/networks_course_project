namespace main_application
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.входящиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.исходящиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаПортовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скоростьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.порт1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.порт2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.CTS_label1 = new System.Windows.Forms.Label();
            this.DSR_label1 = new System.Windows.Forms.Label();
            this.CD_label1 = new System.Windows.Forms.Label();
            this.RI_label1 = new System.Windows.Forms.Label();
            this.DI_label2 = new System.Windows.Forms.Label();
            this.CD_label2 = new System.Windows.Forms.Label();
            this.DSR_label2 = new System.Windows.Forms.Label();
            this.CTS_label2 = new System.Windows.Forms.Label();
            this.CTS_progressBar1 = new System.Windows.Forms.ProgressBar();
            this.CD_progressBar1 = new System.Windows.Forms.ProgressBar();
            this.DSR_progressBar1 = new System.Windows.Forms.ProgressBar();
            this.RI_progressBar1 = new System.Windows.Forms.ProgressBar();
            this.CTS_progressBar2 = new System.Windows.Forms.ProgressBar();
            this.DSR_progressBar2 = new System.Windows.Forms.ProgressBar();
            this.CD_progressBar2 = new System.Windows.Forms.ProgressBar();
            this.RI_progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(478, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Открыть Порт1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.DataBits = 7;
            this.serialPort1.PortName = "NULL";
            this.serialPort1.RtsEnable = true;
            this.serialPort1.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort1_PinChanged);
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.настройкаПортовToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(663, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.входящиеToolStripMenuItem,
            this.исходящиеToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(121, 20);
            this.toolStripMenuItem1.Text = "Папка сообщений";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // входящиеToolStripMenuItem
            // 
            this.входящиеToolStripMenuItem.Name = "входящиеToolStripMenuItem";
            this.входящиеToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.входящиеToolStripMenuItem.Text = "Входящие";
            // 
            // исходящиеToolStripMenuItem
            // 
            this.исходящиеToolStripMenuItem.Name = "исходящиеToolStripMenuItem";
            this.исходящиеToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.исходящиеToolStripMenuItem.Text = "Отправленные";
            // 
            // настройкаПортовToolStripMenuItem
            // 
            this.настройкаПортовToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.скоростьToolStripMenuItem,
            this.порт1ToolStripMenuItem,
            this.порт2ToolStripMenuItem,
            this.дополнительноToolStripMenuItem});
            this.настройкаПортовToolStripMenuItem.Name = "настройкаПортовToolStripMenuItem";
            this.настройкаПортовToolStripMenuItem.Size = new System.Drawing.Size(120, 20);
            this.настройкаПортовToolStripMenuItem.Text = "Настройка портов";
            this.настройкаПортовToolStripMenuItem.Click += new System.EventHandler(this.настройкаПортовToolStripMenuItem_Click);
            // 
            // скоростьToolStripMenuItem
            // 
            this.скоростьToolStripMenuItem.Name = "скоростьToolStripMenuItem";
            this.скоростьToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.скоростьToolStripMenuItem.Text = "Скорость";
            this.скоростьToolStripMenuItem.MouseHover += new System.EventHandler(this.скоростьToolStripMenuItem_MouseHover);
            // 
            // порт1ToolStripMenuItem
            // 
            this.порт1ToolStripMenuItem.Name = "порт1ToolStripMenuItem";
            this.порт1ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.порт1ToolStripMenuItem.Text = "Порт1";
            this.порт1ToolStripMenuItem.DropDownOpening += new System.EventHandler(this.порт1ToolStripMenuItem_DropDownOpening);
            this.порт1ToolStripMenuItem.Click += new System.EventHandler(this.порт1ToolStripMenuItem_MouseHover);
            this.порт1ToolStripMenuItem.MouseHover += new System.EventHandler(this.порт1ToolStripMenuItem_MouseHover);
            // 
            // порт2ToolStripMenuItem
            // 
            this.порт2ToolStripMenuItem.Name = "порт2ToolStripMenuItem";
            this.порт2ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.порт2ToolStripMenuItem.Text = "Порт2";
            this.порт2ToolStripMenuItem.DropDownOpening += new System.EventHandler(this.порт2ToolStripMenuItem_DropDownOpening);
            this.порт2ToolStripMenuItem.Click += new System.EventHandler(this.порт2ToolStripMenuItem_MouseHover);
            this.порт2ToolStripMenuItem.MouseHover += new System.EventHandler(this.порт2ToolStripMenuItem_MouseHover);
            // 
            // дополнительноToolStripMenuItem
            // 
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(478, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(478, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.Location = new System.Drawing.Point(12, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(460, 225);
            this.textBox1.TabIndex = 9;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 263);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(460, 63);
            this.textBox2.TabIndex = 10;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(475, 263);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 26);
            this.button2.TabIndex = 11;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(581, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "label3";
            // 
            // serialPort2
            // 
            this.serialPort2.DataBits = 7;
            this.serialPort2.PortName = "NULL";
            this.serialPort2.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort2_PinChanged);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(478, 224);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "Открыть Порт2";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(478, 68);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 17);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "RTS";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(478, 91);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(49, 17);
            this.checkBox2.TabIndex = 15;
            this.checkBox2.Text = "DTR";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckStateChanged += new System.EventHandler(this.checkBox2_CheckStateChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(478, 198);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(49, 17);
            this.checkBox3.TabIndex = 16;
            this.checkBox3.Text = "DTR";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckStateChanged += new System.EventHandler(this.checkBox3_CheckStateChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(478, 175);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(48, 17);
            this.checkBox4.TabIndex = 17;
            this.checkBox4.Text = "RTS";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckStateChanged += new System.EventHandler(this.checkBox4_CheckStateChanged);
            // 
            // CTS_label1
            // 
            this.CTS_label1.AutoSize = true;
            this.CTS_label1.Location = new System.Drawing.Point(544, 68);
            this.CTS_label1.Name = "CTS_label1";
            this.CTS_label1.Size = new System.Drawing.Size(28, 13);
            this.CTS_label1.TabIndex = 18;
            this.CTS_label1.Text = "CTS";
            // 
            // DSR_label1
            // 
            this.DSR_label1.AutoSize = true;
            this.DSR_label1.Location = new System.Drawing.Point(544, 91);
            this.DSR_label1.Name = "DSR_label1";
            this.DSR_label1.Size = new System.Drawing.Size(30, 13);
            this.DSR_label1.TabIndex = 19;
            this.DSR_label1.Text = "DSR";
            // 
            // CD_label1
            // 
            this.CD_label1.AutoSize = true;
            this.CD_label1.Location = new System.Drawing.Point(605, 68);
            this.CD_label1.Name = "CD_label1";
            this.CD_label1.Size = new System.Drawing.Size(22, 13);
            this.CD_label1.TabIndex = 20;
            this.CD_label1.Text = "CD";
            // 
            // RI_label1
            // 
            this.RI_label1.AutoSize = true;
            this.RI_label1.Location = new System.Drawing.Point(605, 92);
            this.RI_label1.Name = "RI_label1";
            this.RI_label1.Size = new System.Drawing.Size(18, 13);
            this.RI_label1.TabIndex = 21;
            this.RI_label1.Text = "RI";
            // 
            // DI_label2
            // 
            this.DI_label2.AutoSize = true;
            this.DI_label2.Location = new System.Drawing.Point(605, 199);
            this.DI_label2.Name = "DI_label2";
            this.DI_label2.Size = new System.Drawing.Size(18, 13);
            this.DI_label2.TabIndex = 25;
            this.DI_label2.Text = "RI";
            // 
            // CD_label2
            // 
            this.CD_label2.AutoSize = true;
            this.CD_label2.Location = new System.Drawing.Point(605, 175);
            this.CD_label2.Name = "CD_label2";
            this.CD_label2.Size = new System.Drawing.Size(22, 13);
            this.CD_label2.TabIndex = 24;
            this.CD_label2.Text = "CD";
            // 
            // DSR_label2
            // 
            this.DSR_label2.AutoSize = true;
            this.DSR_label2.Location = new System.Drawing.Point(544, 198);
            this.DSR_label2.Name = "DSR_label2";
            this.DSR_label2.Size = new System.Drawing.Size(30, 13);
            this.DSR_label2.TabIndex = 23;
            this.DSR_label2.Text = "DSR";
            // 
            // CTS_label2
            // 
            this.CTS_label2.AutoSize = true;
            this.CTS_label2.Location = new System.Drawing.Point(544, 175);
            this.CTS_label2.Name = "CTS_label2";
            this.CTS_label2.Size = new System.Drawing.Size(28, 13);
            this.CTS_label2.TabIndex = 22;
            this.CTS_label2.Text = "CTS";
            // 
            // CTS_progressBar1
            // 
            this.CTS_progressBar1.Location = new System.Drawing.Point(578, 68);
            this.CTS_progressBar1.Maximum = 1;
            this.CTS_progressBar1.Name = "CTS_progressBar1";
            this.CTS_progressBar1.Size = new System.Drawing.Size(21, 13);
            this.CTS_progressBar1.TabIndex = 26;
            // 
            // CD_progressBar1
            // 
            this.CD_progressBar1.Location = new System.Drawing.Point(633, 68);
            this.CD_progressBar1.Maximum = 1;
            this.CD_progressBar1.Name = "CD_progressBar1";
            this.CD_progressBar1.Size = new System.Drawing.Size(21, 13);
            this.CD_progressBar1.TabIndex = 27;
            // 
            // DSR_progressBar1
            // 
            this.DSR_progressBar1.Location = new System.Drawing.Point(578, 91);
            this.DSR_progressBar1.Maximum = 1;
            this.DSR_progressBar1.Name = "DSR_progressBar1";
            this.DSR_progressBar1.Size = new System.Drawing.Size(21, 13);
            this.DSR_progressBar1.TabIndex = 28;
            // 
            // RI_progressBar1
            // 
            this.RI_progressBar1.Location = new System.Drawing.Point(633, 91);
            this.RI_progressBar1.Maximum = 1;
            this.RI_progressBar1.Name = "RI_progressBar1";
            this.RI_progressBar1.Size = new System.Drawing.Size(21, 13);
            this.RI_progressBar1.TabIndex = 29;
            // 
            // CTS_progressBar2
            // 
            this.CTS_progressBar2.Location = new System.Drawing.Point(578, 175);
            this.CTS_progressBar2.Maximum = 1;
            this.CTS_progressBar2.Name = "CTS_progressBar2";
            this.CTS_progressBar2.Size = new System.Drawing.Size(21, 13);
            this.CTS_progressBar2.TabIndex = 30;
            // 
            // DSR_progressBar2
            // 
            this.DSR_progressBar2.Location = new System.Drawing.Point(578, 198);
            this.DSR_progressBar2.Maximum = 1;
            this.DSR_progressBar2.Name = "DSR_progressBar2";
            this.DSR_progressBar2.Size = new System.Drawing.Size(21, 13);
            this.DSR_progressBar2.TabIndex = 31;
            // 
            // CD_progressBar2
            // 
            this.CD_progressBar2.Location = new System.Drawing.Point(630, 175);
            this.CD_progressBar2.Maximum = 1;
            this.CD_progressBar2.Name = "CD_progressBar2";
            this.CD_progressBar2.Size = new System.Drawing.Size(21, 13);
            this.CD_progressBar2.TabIndex = 32;
            // 
            // RI_progressBar2
            // 
            this.RI_progressBar2.Location = new System.Drawing.Point(629, 199);
            this.RI_progressBar2.Maximum = 1;
            this.RI_progressBar2.Name = "RI_progressBar2";
            this.RI_progressBar2.Size = new System.Drawing.Size(21, 13);
            this.RI_progressBar2.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(581, 266);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 34;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(663, 338);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.RI_progressBar2);
            this.Controls.Add(this.CD_progressBar2);
            this.Controls.Add(this.DSR_progressBar2);
            this.Controls.Add(this.CTS_progressBar2);
            this.Controls.Add(this.RI_progressBar1);
            this.Controls.Add(this.DSR_progressBar1);
            this.Controls.Add(this.CD_progressBar1);
            this.Controls.Add(this.CTS_progressBar1);
            this.Controls.Add(this.DI_label2);
            this.Controls.Add(this.CD_label2);
            this.Controls.Add(this.DSR_label2);
            this.Controls.Add(this.CTS_label2);
            this.Controls.Add(this.RI_label1);
            this.Controls.Add(this.CD_label1);
            this.Controls.Add(this.DSR_label1);
            this.Controls.Add(this.CTS_label1);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem настройкаПортовToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem скоростьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem порт1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem порт2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem входящиеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem исходящиеToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label CTS_label1;
        private System.Windows.Forms.Label DSR_label1;
        private System.Windows.Forms.Label CD_label1;
        private System.Windows.Forms.Label RI_label1;
        private System.Windows.Forms.Label DI_label2;
        private System.Windows.Forms.Label CD_label2;
        private System.Windows.Forms.Label DSR_label2;
        private System.Windows.Forms.Label CTS_label2;
        private System.Windows.Forms.ProgressBar CD_progressBar1;
        private System.Windows.Forms.ProgressBar DSR_progressBar1;
        private System.Windows.Forms.ProgressBar RI_progressBar1;
        private System.Windows.Forms.ProgressBar CTS_progressBar2;
        private System.Windows.Forms.ProgressBar DSR_progressBar2;
        private System.Windows.Forms.ProgressBar CD_progressBar2;
        private System.Windows.Forms.ProgressBar RI_progressBar2;
        private System.Windows.Forms.ProgressBar CTS_progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
    }
}

