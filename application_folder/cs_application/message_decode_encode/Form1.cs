using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace message_decode_encode
{
    public partial class Form1 : Form
    {
        public static string AppDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string CrcLibDir = Path.Combine(AppDir, @"..\..\..\..", @"crclib_bytes.py");
        public static Encoding Win1251 = Encoding.GetEncoding("windows-1251");

        public Form1()
        { InitializeComponent(); }

        //On Comport1 opening
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(serialPort1.IsOpen))
                {
                    serialPort1.DtrEnable = checkBox2.Checked;
                    serialPort1.RtsEnable = checkBox1.Checked;
                    serialPort1.Open();
                    button1.Text = "Закрыть порт1";
                    try
                    {
                        if (serialPort1.CDHolding) CD_progressBar1.Value = 1;
                        else { CD_progressBar1.Value = 0; }

                        if (serialPort1.CtsHolding) CTS_progressBar1.Value = 1;
                        else { CTS_progressBar1.Value = 0; }

                        if (serialPort1.DsrHolding) DSR_progressBar1.Value = 1;
                        else { DSR_progressBar1.Value = 0; }
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    button1.Text = "Открыть порт1";
                }
            }
            catch (Exception ex)
            {
                button1.Text = "Открыть порт1";
                MessageBox.Show(ex.Message);
            }
        }

        //On Comport2 opening(Is not used yet)
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(serialPort2.IsOpen))
                {
                    serialPort2.DtrEnable = checkBox3.Checked;
                    serialPort2.RtsEnable = checkBox4.Checked;
                    serialPort2.Open();
                    button3.Text = "Закрыть порт2";
                    try
                    {
                        if (serialPort2.CDHolding) CD_progressBar2.Value = 1; else { CD_progressBar2.Value = 0; }
                        if (serialPort2.CtsHolding) CTS_progressBar2.Value = 1; else { CTS_progressBar2.Value = 0; }
                        if (serialPort2.DsrHolding) DSR_progressBar2.Value = 1; else { DSR_progressBar2.Value = 0; }
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (serialPort2.IsOpen)
                {
                    try
                    {
                        serialPort2.Close();
                        button3.Text = "Открыть порт2";
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                button3.Text = "Открыть порт2";
                MessageBox.Show(ex.Message);
            }
        }

        //Methods are not used yet
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        { }
        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        { }
        private void настройкаПортовToolStripMenuItem_Click(object sender, EventArgs e)
        { }
        private void Form1_Load(object sender, EventArgs e)
        { }
        private void порт1ToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        { }

        private void processMeunItem1(object sender, EventArgs e)
        {
            string selectedMenuItemName = ((ToolStripMenuItem)sender).Text;
            try
            {
                this.serialPort1.PortName = selectedMenuItemName;
                label1.Text = selectedMenuItemName;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void порт2ToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        { }
        private void processMeunItem2(object sender, EventArgs e)
        {
            string selectedMenuItemName = ((ToolStripMenuItem)sender).Text;
            try
            {
                this.serialPort2.PortName = selectedMenuItemName;
                label2.Text = selectedMenuItemName;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void порт1ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ToolStripMenuItem tsParent = (ToolStripMenuItem)sender;
            if (tsParent.DropDownItems.Count == 0)
            {
                string[] ports = SerialPort.GetPortNames();
                ToolStripMenuItem portMenuItem;
                for (int i = 0; i < ports.Length; i++)
                {
                    portMenuItem = new ToolStripMenuItem();
                    portMenuItem.Text = ports[i];
                    portMenuItem.Tag = i;
                    portMenuItem.Click += new EventHandler(processMeunItem1);
                    //portMenuItem.CheckOnClick = true;
                    tsParent.DropDownItems.Add(portMenuItem);
                }
            }
        }

        private void порт2ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ToolStripMenuItem tsParent = (ToolStripMenuItem)sender;
            if (tsParent.DropDownItems.Count == 0)
            {
                string[] ports = SerialPort.GetPortNames();
                //define new ToolStripMenuItem to use as the new child
                ToolStripMenuItem portMenuItem;
                for (int i = 0; i < ports.Length; i++)
                {
                    portMenuItem = new ToolStripMenuItem();
                    portMenuItem.Text = ports[i];
                    portMenuItem.Tag = i;
                    portMenuItem.Click += new EventHandler(processMeunItem2);
                    //portMenuItem.CheckOnClick = true;
                    tsParent.DropDownItems.Add(portMenuItem);
                }
            }
        }

        private void скоростьToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ToolStripMenuItem tsParent = (ToolStripMenuItem)sender;
            if (tsParent.DropDownItems.Count == 0)
            {
                string[] baudrates = {"600","1200", "2400", "4800", "9600", "14400", "19200",
                    "28800", "38400", "56000","115200","128000","256000" };
                ToolStripMenuItem baudrateMenuItem;
                for (int i = 0; i < baudrates.Length; i++)
                {
                    baudrateMenuItem = new ToolStripMenuItem();
                    baudrateMenuItem.Text = baudrates[i];
                    baudrateMenuItem.Tag = i;
                    baudrateMenuItem.Click += new EventHandler(processbaudrateMenuItem);
                    // baudrateMenuItem.CheckOnClick = true;
                    tsParent.DropDownItems.Add(baudrateMenuItem);
                }
            }
        }
        private void processbaudrateMenuItem(object sender, EventArgs e)
        {
            string selectedMenuItemName = ((ToolStripMenuItem)sender).Text;
            this.serialPort2.BaudRate = Convert.ToInt32(selectedMenuItemName);
            this.serialPort1.BaudRate = Convert.ToInt32(selectedMenuItemName);
            label3.Text = selectedMenuItemName;
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        { serialPort1.DtrEnable = ((CheckBox)sender).Checked; }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        { serialPort1.RtsEnable = ((CheckBox)sender).Checked; }

        private void checkBox4_CheckStateChanged(object sender, EventArgs e)
        { serialPort2.RtsEnable = ((CheckBox)sender).Checked; }

        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        { serialPort2.DtrEnable = ((CheckBox)sender).Checked; }

        private void serialPort1_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Thread.Sleep(50);
            bool[] serial1state = { serialPort1.CDHolding, serialPort1.CtsHolding, serialPort1.DsrHolding };
            this.BeginInvoke(new SetStateDeleg1(setstate1), new object[] { serial1state });
        }
        private void serialPort2_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Thread.Sleep(50);
            bool[] serial1state = { serialPort2.CDHolding, serialPort2.CtsHolding, serialPort2.DsrHolding };
            this.BeginInvoke(new SetStateDeleg2(setstate2), new object[] { serial1state });
        }

        private delegate void SetStateDeleg1(bool[] serialstate);
        private delegate void SetStateDeleg2(bool[] serialstate);
        private void setstate1(bool[] data)
        {
            bool CD = data[0];
            bool Cts = data[1];
            bool Dsr = data[2];
            if (CD) CD_progressBar1.Value = 1; else { CD_progressBar1.Value = 0; }
            if (Cts) CTS_progressBar1.Value = 1; else { CTS_progressBar1.Value = 0; }
            if (Dsr) DSR_progressBar1.Value = 1; else { DSR_progressBar1.Value = 0; }
        }
        private void setstate2(bool[] data)
        {
            bool CD = data[0];
            bool Cts = data[1];
            bool Dsr = data[2];

            if (CD) CD_progressBar2.Value = 1; else { CD_progressBar2.Value = 0; }
            if (Cts) CTS_progressBar2.Value = 1; else { CTS_progressBar2.Value = 0; }
            if (Dsr) DSR_progressBar2.Value = 1; else { DSR_progressBar2.Value = 0; }

        }
        

        // Делегат используется для записи в UI control из потока не-UI
        private delegate void SetTextDeleg(string text);
        private delegate void SetStatusDeleg(string text);

        private void si_DataReceived(string data)
        { textBox1.Text = textBox1.Text + "\n" + data.Trim(); }

        private void setstatus_DataReceived(string data)
        {
            label4.Text = data;
        }


        /* 
            Serial port basic frame structure
             -----------------------------------------------------------------------------------------------
            | startbyte | destination address | sender address | frame type | data length | Data | StopByte |
             -----------------------------------------------------------------------------------------------
            start/stop byte = 0xFF
            destination address ={0,..,0x7F}
            sender address ={0,..,0x7F}
            frame type ={0,..,0x7F}
            data length ={0,..,0x7F}
            Data ={0,..,..,n} n <=0x7F-1 //Not more than 128 bytes
            StopByte = 0xFF

        */

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(50);
            serialPort1.Encoding = Encoding.ASCII;
            serialPort1.DataBits = 8;

            string decoded_message = "";
            string asciidata;
            string Win1251DecodedStringFromPython = "";

            ScriptEngine engine = Python.CreateEngine();
            var paths = engine.GetSearchPaths();
            paths.Add(@"C:\Python27\Lib");
            engine.SetSearchPaths(paths);
            ScriptScope scope = engine.CreateScope();
            engine.ExecuteFile(CrcLibDir, scope);

            try
            {
                BeginInvoke(new SetStatusDeleg(setstatus_DataReceived), new object[] { "Data Receiving" });

                asciidata = serialPort1.ReadExisting();
                byte[] asciiByteArray = Encoding.ASCII.GetBytes(asciidata);

                dynamic function = scope.GetVariable("decode_data");
                dynamic utf8result = function(asciiByteArray);

                decoded_message = utf8result;

                byte[] win1251DecodedBytesFromPython = Win1251.GetBytes(decoded_message);
                Win1251DecodedStringFromPython = Win1251.GetString(win1251DecodedBytesFromPython);

                BeginInvoke(new SetStatusDeleg(setstatus_DataReceived), new object[] { "Ready" });

            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }

            BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { Win1251DecodedStringFromPython });
        }


        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Encoding = Encoding.ASCII;
            serialPort1.DataBits = 8;

            // Launch Python Script 
            ScriptEngine engine = Python.CreateEngine();
            var paths = engine.GetSearchPaths();
            paths.Add(@"C:\Python27\Lib");
            engine.SetSearchPaths(paths);
            ScriptScope scope = engine.CreateScope();
            engine.ExecuteFile(CrcLibDir, scope);

            label4.Text = "Sending";
            string utf8message = textBox2.Text;

            byte[] utf8Bytearray = Encoding.UTF8.GetBytes(utf8message);
            //Array of win1251 bytes from utf8 string
            byte[] win1251ByteArray = Encoding.Convert(Encoding.UTF8, Win1251, utf8Bytearray);

            /*
                string utf8string = UTF8Encoding.UTF8.GetString(utf8Bytearray);
                byte[] asciiBytearray = Encoding.ASCII.GetBytes(utf8message);
                string asciistring = ASCIIEncoding.ASCII.GetString(asciiBytearray);
            */

            //Win1251 bytes Coding to doubled ascii( 7-bit byte format)
            dynamic function = scope.GetVariable("encode_data");
            dynamic result = function(win1251ByteArray);
            var encoded_message = result;
            //Get win1252Bytes from returned Symbols
            byte[] asciiByteArrayFromPython = Encoding.ASCII.GetBytes(encoded_message);

            string asciistring = ASCIIEncoding.ASCII.GetString(asciiByteArrayFromPython);
            //byte[] asciiByteArray = Encoding.Convert(Win1251, Encoding.ASCII, asciiByteArrayFromPython);

            //Should send raw, 7bit ascii strings(already encoded with 7bit crc, 2x7bit==1Byte of decoded char)
            try { serialPort1.Write(asciistring); }
            catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Error!"); }

            //Decoding the received messages is here. Just for instant tests.
            //Execute Decoding received ascii doubled array, get win1251 char array
            function = scope.GetVariable("decode_data");
            result = function(asciiByteArrayFromPython);
            var decoded_message = result;
            byte[] win1251DecodedBytesFromPython = Win1251.GetBytes(decoded_message);
            string Win1251DecodedStringFromPython = Win1251.GetString(win1251DecodedBytesFromPython);
            textBox1.Text = Win1251DecodedStringFromPython;

            label4.Text = "Ready";

        }


    }
}


