using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main_application
{
    public partial class AuthForm : Form
    {
        Form1 form;
        public AuthForm(Form1 form1)
        {
            form = form1;
            InitializeComponent();
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string auth_local = textBox1.Text.Trim();

            form.AuthData_mutex.WaitOne();
            form.AuthData["local"] = auth_local;
            form.AuthData_mutex.ReleaseMutex();
            this.Close();
        }
    }
}
