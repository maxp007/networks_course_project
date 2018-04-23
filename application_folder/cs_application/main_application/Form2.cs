using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace main_application
{
    public partial class Form2 : Form
    {
        public Form1 form1;
        
        public bool UpdateThread_to_close = false;
        public Form2(Form1 form)
        {
            UpdateThread_to_close = false;
            this.form1 = form;
            InitializeComponent();
            Thread LookForInboxUpdatesthr = new Thread(LookForInboxUpdates);
            LookForInboxUpdatesthr.IsBackground = true;
            LookForInboxUpdatesthr.Start();

        }

        public void Show_Inbox() {

            using (CourseDB db = new CourseDB())
            {
                dataGridView1.DataSource = db.inbox.ToList();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            Show_Inbox();
        }

        private delegate void SetTextDeleg(string text);
        public void setlabel4state(string text)
        {
            label4.Text = text;
        }
        private delegate void UpdateDataGridDeleg(List<inbox> list);
        public void UpdateDataGrid1(List<inbox> list)
        {
            dataGridView1.DataSource  = list;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
        public void LookForInboxUpdates(){
            while(true){
                form1.Inbox_update_mutex.WaitOne();

                if (form1.Inbox_update_needed)
                {
                    form1.Inbox_update_needed = false;
                    form1.Inbox_update_mutex.ReleaseMutex();

                    
                    BeginInvoke(new SetTextDeleg(setlabel4state), new object[] { "обновляется" });
                    using (CourseDB db = new CourseDB())
                    {
                        
                        BeginInvoke(new UpdateDataGridDeleg(UpdateDataGrid1), new object[] { db.inbox.ToList() });
                    }
                    

                }
                else
                {
                    form1.Inbox_update_mutex.ReleaseMutex();
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
                try { textBox1.Text = row.Cells[1].Value.ToString(); }
                catch (Exception ex) { textBox1.Text = ""; }

                try { textBox2.Text = row.Cells[3].Value.ToString(); }
                catch (Exception ex) { textBox2.Text = ""; }

                try { textBox3.Text = row.Cells[4].Value.ToString(); }
                catch (Exception ex) { textBox3.Text = ""; }

                try { textBox4.Text = row.Cells[7].Value.ToString();}
                catch (Exception ex){textBox4.Text = "";}
                try
                { textBox5.Text = row.Cells[5].Value.ToString(); }
                catch (Exception ex) { textBox5.Text = ""; }

                //Попытка отправить уведомление о прочтении
                bool foreign_id_exists = false;
                string foreign_id_string="";
                string sender_string = "";
                string status_string = "";
                try {
                    foreign_id_string = row.Cells[6].Value.ToString();
                    sender_string = row.Cells[1].Value.ToString();
                    status_string = row.Cells[5].Value.ToString();
                    foreign_id_exists = true;
                }
                catch(Exception ex)
                {
                    foreign_id_exists = false;
                }
                if (foreign_id_exists)
                {
                    if (form1.PhoneBook["3"] != null && form1.PhoneBook["1"] != null && form1.PhoneBook["2"] != null)
                    {
                        if (form1.AuthData["Port1"] != null && form1.AuthData["Port2"] != null && form1.AuthData["local"] != null)
                        {
                            if (status_string != "Прочитано")
                            {
                                int len = foreign_id_string.Length;
                                // Здесь ожидаем получения мьютекса на создание заданий
                                // После получения, используя методы Form1 создается кадр об открытии письма
                                // Необходимо получить значение "Port1/2" по ключу "sender",
                                // а также номер машины, за которой сидит этот sender
                                var Receiver_Port = form1.AuthData.FirstOrDefault(x => x.Value == sender_string).Key;
                                //either "Port1" or "Port2"
                                string Port = Receiver_Port.ToString();
                                var pc_address = form1.PhoneBook.FirstOrDefault(x => x.Value == Receiver_Port.ToString()).Key;
                                var local_pc_address = form1.PhoneBook.FirstOrDefault(x => x.Value == "local").Key;
                                //either "1" or "2" or "3"
                                string pc_addr = pc_address.ToString();
                               
                                string local_pc_addr = local_pc_address.ToString();

                                //Конвертирование из utf-8 в win1251 уже предусмотрено в функции
                                byte[] frame = form1.CreateNewFrame(Form1.FrameType.OPENLETTER, local_pc_addr, len.ToString(), pc_addr, foreign_id_string, false);

                                Form1.One_Task openletterframe = new Form1.One_Task();

                                form1.TaskToSend_mutex.WaitOne();
                                form1.TasksToSend.Add(new Form1.One_Task(Receiver_Port, frame));
                                form1.TaskToSend_mutex.ReleaseMutex();

                                using (CourseDB db = new CourseDB())
                                {
                                    var foreign_id_num = long.Parse(foreign_id_string);
                                   var result = db.inbox.SingleOrDefault(x => x.foreign_id == foreign_id_num);
                                   if (result != null)
                                   {
                                        result.status = "Прочитано";
                                        db.SaveChanges();
                                        form1.Inbox_update_mutex.WaitOne();
                                        form1.Inbox_update_needed = true;
                                        form1.Inbox_update_mutex.ReleaseMutex();
                                   }                                
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }


            }

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateThread_to_close = true;
            Thread.Sleep(2500);

        }
    }
}
