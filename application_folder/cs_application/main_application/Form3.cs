﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main_application
{
    public partial class Form3 : Form
    {
        public Form1 form1;

        public bool UpdateThread_to_close = false;

        public Form3(Form1 form)
        {
            UpdateThread_to_close = false;
            this.form1 = form;
            InitializeComponent();
            Thread LookForOutboxUpdatesthr = new Thread(LookForOutboxUpdates);
            LookForOutboxUpdatesthr.IsBackground = true;

            LookForOutboxUpdatesthr.Start();
        }

        public void Show_Outbox()
        {
            using (CourseDB db = new CourseDB())
            {
                dataGridView1.DataSource = db.outbox.ToList();
            }
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[0].Width = 20;
            dataGridView1.Columns[1].Width = 50;
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[5].Width = 190;
            dataGridView1.Columns[7].Width = 90;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[5].Value.ToString() == "Принято")
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;

                }
                if (row.Cells[5].Value.ToString() == "Прочитано")
                {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
            dataGridView1.Update();
            dataGridView1.Refresh();

        }


        private delegate void SetTextDeleg(string text);
        public void setlabel4state(string text)
        {
            label5.Text = text;
        }

        private delegate void UpdateDataGridDeleg(List<outbox> list);
        public void UpdateDataGrid1(List<outbox> list)
        {
            dataGridView1.DataSource = list;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[5].Value.ToString() == "Принято")
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;

                }
                if (row.Cells[5].Value.ToString() == "Прочитано")
                {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
        public void LookForOutboxUpdates()
        {
            while (true)
            {
                form1.Outbox_update_mutex.WaitOne();

                if (form1.Outbox_update_needed)
                {
                    form1.Outbox_update_needed = false;
                    form1.Outbox_update_mutex.ReleaseMutex();

                    BeginInvoke(new SetTextDeleg(setlabel4state), new object[] { "обновляется" });
                    using (CourseDB db = new CourseDB())
                    {
                        BeginInvoke(new UpdateDataGridDeleg(UpdateDataGrid1), new object[] { db.outbox.ToList() });
                    }
                }
                else
                {
                    form1.Outbox_update_mutex.ReleaseMutex();
                }

                Thread.Sleep(1000);
                BeginInvoke(new SetTextDeleg(setlabel4state), new object[] { "ожидание" });

                if (UpdateThread_to_close)
                { 
                    break;
                }
            }
        }

        //Обработка клика на письмо 
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

                //Отображение письма в текстБоксы 

                try { textBox1.Text = row.Cells[2].Value.ToString(); }
                catch (Exception ex) { textBox1.Text = ""; }

                try { textBox2.Text = row.Cells[3].Value.ToString(); }
                catch (Exception ex) { textBox2.Text = ""; }

                try { textBox3.Text = row.Cells[4].Value.ToString(); }
                catch (Exception ex) { textBox3.Text = ""; }

                try { textBox4.Text = row.Cells[7].Value.ToString(); }
                catch (Exception ex) { textBox4.Text = ""; }
                try
                { textBox5.Text = row.Cells[5].Value.ToString(); }
                catch (Exception ex) { textBox5.Text = ""; }

            }

        }

        private void Form3_Load_1(object sender, EventArgs e)
        {
            Show_Outbox();
        }

        private void Form3_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            UpdateThread_to_close = true;
            Thread.Sleep(2500);
        }
    }

}
