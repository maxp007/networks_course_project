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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.LetterTextBox = new System.Windows.Forms.TextBox();
            this.SendNewLetterButton = new System.Windows.Forms.Button();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.входящиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.исходящиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаПортовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скоростьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox3 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.порт1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.порт2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.открытьПортыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button3 = new System.Windows.Forms.Button();
            this.ReceiverComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.port1state_label = new System.Windows.Forms.Label();
            this.port2state_label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.ReTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataBits = 7;
            this.serialPort1.PortName = "NULL";
            this.serialPort1.RtsEnable = true;
            this.serialPort1.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort1_PinChanged);
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.Location = new System.Drawing.Point(12, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(460, 160);
            this.textBox1.TabIndex = 9;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // LetterTextBox
            // 
            this.LetterTextBox.Location = new System.Drawing.Point(12, 263);
            this.LetterTextBox.Multiline = true;
            this.LetterTextBox.Name = "LetterTextBox";
            this.LetterTextBox.Size = new System.Drawing.Size(531, 63);
            this.LetterTextBox.TabIndex = 10;
            // 
            // SendNewLetterButton
            // 
            this.SendNewLetterButton.Location = new System.Drawing.Point(563, 233);
            this.SendNewLetterButton.Name = "SendNewLetterButton";
            this.SendNewLetterButton.Size = new System.Drawing.Size(100, 26);
            this.SendNewLetterButton.TabIndex = 11;
            this.SendNewLetterButton.Text = "Отправить";
            this.SendNewLetterButton.UseVisualStyleBackColor = true;
            this.SendNewLetterButton.Click += new System.EventHandler(this.SendNewLetterButton_Click);
            // 
            // serialPort2
            // 
            this.serialPort2.DataBits = 7;
            this.serialPort2.PortName = "NULL";
            this.serialPort2.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort2_PinChanged);
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(652, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 34;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(485, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 25);
            this.button1.TabIndex = 35;
            this.button1.Text = "Отрыть порты";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.настройкаПортовToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(686, 24);
            this.menuStrip1.TabIndex = 36;
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
            // 
            // входящиеToolStripMenuItem
            // 
            this.входящиеToolStripMenuItem.Name = "входящиеToolStripMenuItem";
            this.входящиеToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.входящиеToolStripMenuItem.Text = "Входящие";
            this.входящиеToolStripMenuItem.Click += new System.EventHandler(this.входящиеToolStripMenuItem_Click);
            // 
            // исходящиеToolStripMenuItem
            // 
            this.исходящиеToolStripMenuItem.Name = "исходящиеToolStripMenuItem";
            this.исходящиеToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.исходящиеToolStripMenuItem.Text = "Отправленные";
            this.исходящиеToolStripMenuItem.Click += new System.EventHandler(this.исходящиеToolStripMenuItem_Click);
            // 
            // настройкаПортовToolStripMenuItem
            // 
            this.настройкаПортовToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.скоростьToolStripMenuItem,
            this.порт1ToolStripMenuItem,
            this.порт2ToolStripMenuItem,
            this.дополнительноToolStripMenuItem,
            this.открытьПортыToolStripMenuItem});
            this.настройкаПортовToolStripMenuItem.Name = "настройкаПортовToolStripMenuItem";
            this.настройкаПортовToolStripMenuItem.Size = new System.Drawing.Size(120, 20);
            this.настройкаПортовToolStripMenuItem.Text = "Настройка портов";
            // 
            // скоростьToolStripMenuItem
            // 
            this.скоростьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox3,
            this.toolStripSeparator1,
            this.toolStripComboBox3});
            this.скоростьToolStripMenuItem.Name = "скоростьToolStripMenuItem";
            this.скоростьToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.скоростьToolStripMenuItem.Text = "Скорость";
            // 
            // toolStripTextBox3
            // 
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Size = new System.Drawing.Size(120, 23);
            this.toolStripTextBox3.Text = "Скороть (бод/с)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // toolStripComboBox3
            // 
            this.toolStripComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox3.Items.AddRange(new object[] {
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "76800",
            "115200"});
            this.toolStripComboBox3.Name = "toolStripComboBox3";
            this.toolStripComboBox3.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox3.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox3_SelectedIndexChanged);
            // 
            // порт1ToolStripMenuItem
            // 
            this.порт1ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.порт1ToolStripMenuItem.Name = "порт1ToolStripMenuItem";
            this.порт1ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.порт1ToolStripMenuItem.Text = "Порт1";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // порт2ToolStripMenuItem
            // 
            this.порт2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox2});
            this.порт2ToolStripMenuItem.Name = "порт2ToolStripMenuItem";
            this.порт2ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.порт2ToolStripMenuItem.Text = "Порт2";
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox2.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox2_SelectedIndexChanged);
            // 
            // дополнительноToolStripMenuItem
            // 
            this.дополнительноToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // открытьПортыToolStripMenuItem
            // 
            this.открытьПортыToolStripMenuItem.Name = "открытьПортыToolStripMenuItem";
            this.открытьПортыToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.открытьПортыToolStripMenuItem.Text = "Открыть порты";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(496, 94);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 26);
            this.button3.TabIndex = 37;
            this.button3.Text = "Подключиться к сети";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ReceiverComboBox
            // 
            this.ReceiverComboBox.FormattingEnabled = true;
            this.ReceiverComboBox.Location = new System.Drawing.Point(365, 236);
            this.ReceiverComboBox.Name = "ReceiverComboBox";
            this.ReceiverComboBox.Size = new System.Drawing.Size(178, 21);
            this.ReceiverComboBox.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(482, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Адрес в сети";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(561, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "addr";
            // 
            // port1state_label
            // 
            this.port1state_label.AutoSize = true;
            this.port1state_label.Location = new System.Drawing.Point(542, 60);
            this.port1state_label.Name = "port1state_label";
            this.port1state_label.Size = new System.Drawing.Size(55, 13);
            this.port1state_label.TabIndex = 43;
            this.port1state_label.Text = "отключен";
            // 
            // port2state_label
            // 
            this.port2state_label.AutoSize = true;
            this.port2state_label.Location = new System.Drawing.Point(542, 78);
            this.port2state_label.Name = "port2state_label";
            this.port2state_label.Size = new System.Drawing.Size(55, 13);
            this.port2state_label.TabIndex = 44;
            this.port2state_label.Text = "отключен";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(482, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Абонент 1 ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(482, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Абонент 2";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(581, 34);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(96, 23);
            this.button4.TabIndex = 47;
            this.button4.Text = "Закрыть Порты";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // ReTextbox
            // 
            this.ReTextbox.Location = new System.Drawing.Point(51, 237);
            this.ReTextbox.Name = "ReTextbox";
            this.ReTextbox.Size = new System.Drawing.Size(257, 20);
            this.ReTextbox.TabIndex = 48;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 240);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "Re:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(326, 240);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 50;
            this.label7.Text = "Кому";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(485, 179);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 23);
            this.button2.TabIndex = 51;
            this.button2.Text = "SetInboxUpdate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(591, 179);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(95, 23);
            this.button5.TabIndex = 52;
            this.button5.Text = "SetOutboxUpdate";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(528, 163);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(135, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "обновление папки писем";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(563, 265);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(100, 26);
            this.button6.TabIndex = 54;
            this.button6.Text = "to bytes";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(686, 338);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ReTextbox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.port2state_label);
            this.Controls.Add(this.port1state_label);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ReceiverComboBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SendNewLetterButton);
            this.Controls.Add(this.LetterTextBox);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox LetterTextBox;
        private System.Windows.Forms.Button SendNewLetterButton;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem входящиеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem исходящиеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкаПортовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem скоростьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem порт1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem порт2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьПортыToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox ReceiverComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label port1state_label;
        private System.Windows.Forms.Label port2state_label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox ReTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}

