using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Data.Common;
using Newtonsoft.Json;


namespace main_application
{
    public partial class Form1 : Form
    {
        public static string AppDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string CrcLibDir = Path.Combine(AppDir, @"..\..\..\..", @"crclib_bytes.py");
        // Использется для передачти байтов по компорту
        public static Encoding WIN1251 = Encoding.GetEncoding("windows-1251");
        public static Encoding ASCII = Encoding.ASCII;
        public string database_name = "coursedb.sqlite3";

        // Начальные состояния соединений программы
        public Dictionary<string, string> Channel_status = new Dictionary<string, string> {
            { "ACK1", "undef" },
            { "ACK2", "undef" },
            { "ACK_local", "undef" }
        };
        public Mutex Channel_status_mutex = new Mutex();

        public Dictionary<string, string> Auth_status = new Dictionary<string, string> {
            { "ACK1", "undef" },
            { "ACK2", "undef" },
            { "ACK_local", "undef" }
        };
        public Mutex Auth_status_mutex = new Mutex();


        // Здесь отмечаются только сообщения информационного кадра (пока так)
        byte[] LastFrameSenttoPort1;
        byte[] LastFrameSenttoPort2;

        //Тесты visual studio 
        public Int32 Ack1_awaited = 0;
        public Int32 Ack2_awaited = 0;
        public Mutex Ack1_mutex = new Mutex();
        public Mutex Ack2_mutex = new Mutex();

        public Int32 Ack1_awaited_Auth = 0;
        public Int32 Ack2_awaited_Auth = 0;
        public Mutex Ack1_mutex_Auth = new Mutex();
        public Mutex Ack2_mutex_Auth = new Mutex();

        public bool SerialsToClose = false;
        public Mutex SerialsToClose_mutex = new Mutex();

        string SelectedPort1Name;
        Mutex SelectedPort1Name_mutex = new Mutex();
        string SelectedPort2Name;
        Mutex SelectedPort2Name_mutex = new Mutex();
        string SelectedBaudrate;
        Mutex SelectedBaudrate_mutex = new Mutex();

        //Мьютекс для согласования: чтения, записи, и изменения в списке ReceivedFrames
        public Mutex ReceivedFrames_mutex1 = new Mutex();

        //Списки принятых данных для обмена между потоками Serial1_receiving1() и FindFrame1()
        public List<byte> ReceivedFrames1 = new List<byte>();
        public byte[] FrameToSend;

        //Мьютекс для согласования: чтения, записи, и изменения в списке ReceivedFrames2
        public Mutex ReceivedFrames_mutex2 = new Mutex();

        //Списки принятых данных для обмена между потоками Serial1_receiving2() и FindFrame2()
        public List<byte> ReceivedFrames2 = new List<byte>();


        // Структура для хранения задания
        public struct One_Task
        {
            public string PortNum;
            public byte[] Frame;
            public One_Task(string name, byte[] frame)
            {
                PortNum = name;
                Frame = frame;
            }
        }
        // Список заданий используется четырмя потоками- По два на каждый порт
        // Формат записи заданий One_Task("Номер порта", Байты[] пришедшие в порт)
        //Список заданий, новое задание помещается в конец списка, выполненное удаляется из начала списка
        // Список заданий содержит задания из первого и второго порта
        public List<One_Task> TasksReceived = new List<One_Task>();

        //Мьютекс для согласования: чтения, записи, и изменения заданий
        // Использется для первого и второго порта одновременно
        public Mutex TaskReceived_mutex = new Mutex();


        public List<One_Task> TasksToSend = new List<One_Task>();

        //Мьютекс для согласования: чтения, записи, и изменения заданий
        // Использется для первого и второго порта одновременно
        public Mutex TaskToSend_mutex = new Mutex();

        // Содержит соответствие номеров машин и номер порта локальной
        // машины на котором висит нумерованная машина
        // "0" - адрес широковещания, Кадр с таким адресом принимает любая машина
        public Dictionary<string, string> PhoneBook = new Dictionary<string, string> {
            { "1", null },
            { "2", null },
            { "3", null }
        };

        public string Local_AuthName = null;
        public string Local_Address = null;
        public Dictionary<string, string> AuthData = new Dictionary<string, string>
        {
            { "Port1", null },
            { "Port2", null },
            { "local", null }
        };
        public Mutex AuthData_mutex = new Mutex();

        //Номера портов соответствуют номерам реальных компортов системы(НЕ используется)
        public Dictionary<string, string> PortAssoc = new Dictionary<string, string> {
            { "Port1", null },
            { "Port2", null },

        };


        // Заполняется при получении кадра Meeting
        // Если всё заполнено, значит физ соединение установлено
        // На данном этапе в порты приходят значения времени в кадрах Meeting
        // Локальная машина на основании этих данных принимает решение, какой индекс назначить самой себе 
        // И сопоставить адреса машин с портами, на которых они висят

        public Dictionary<string, string> Computers_Timestamps = new Dictionary<string, string> {
            { "local", null },
            { "Port1", null },
            { "Port2", null}
        };


        // Определение состояний
        public enum Connection_Status
        {
            CONNECTION_WAIT,
            CONNECTED,
            DISCONNECTION_WAIT,
            DISCONNECTED,
        };


        // portname ассоциируется с адресом машины, которая висит на этом порте (Dictionary<Portname, WsAddress>)
        // Адреса машин задаются после открытия портов(при этом машина пытается получить timestamp с двух других машин)
        // Когда все машины собрали timestamp'ы, каждая однозначно определяет свой адрес в сети и адрес двух других машин
        // WsAddress ассоциируется с именем пользователя, который сидит за машиной в  течение текущего сеанса
        // Meeting PAYLOAD
        // ts = 1234554321;  + PortName

        // Openletter  PAYLOAD
        // message_id = 123; + PortName
        class MessageToOpen_class
        {
            public string message_id { get; set; }
        }
        // InfoFrame PAYLOAD
        //модели данных для почтовых сообщений

        #region Описание Модели Данных
        class TimeStamp_class
        {
            public string timestamp { get; set; }

        }
        class inbox_class
        {
            public string id { get; set; }
            public string sender { get; set; }
            public string recepient { get; set; }
            public string re { get; set; }
            public string msg { get; set; }
            public string status { get; set; }
            public string date_received { get; set; }
            public string foreign_id { get; set; }
            public inbox_class()
            { }
            public inbox_class(inbox letter)
            {
                this.foreign_id = letter.id.ToString();
                this.sender = letter.sender.ToString();
                this.recepient = letter.recepient.ToString();
                this.re = letter.re.ToString();
                this.msg = letter.msg.ToString();
                this.status = letter.status.ToString();
                this.date_received = letter.date_received.ToString();
                this.id = "";
            }
        };
        class outbox_class
        {
            public string id { get; set; }
            public string sender { get; set; }
            public string recepient { get; set; }
            public string re { get; set; }
            public string msg { get; set; }
            public string status { get; set; }
            public string date_sent { get; set; }
            public outbox_class()
            { }
            public outbox_class(outbox letter)
            {
                this.id = letter.id.ToString();
                this.sender = letter.sender.ToString();
                this.recepient = letter.recepient.ToString();
                this.re = letter.re.ToString();
                this.msg = letter.msg.ToString();
                this.status = letter.status.ToString();

                //this.date_sent = letter.date_sent.ToString();

            }
        };
        #endregion

        /**************************************************************
                    ДЛЯ ВЗАИМОДЕЙСТВИЯ С ФОРМАМИ ПИСЕМ
        **************************************************************/

        public Mutex Inbox_update_mutex = new Mutex();
        public Mutex Outbox_update_mutex = new Mutex();

        public bool Inbox_update_needed = false;
        public bool Outbox_update_needed = false;

        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

        #region КОДИРОВАНИЕ СООБЩЕНИЙ   
        /***********************************************************
                КОДИРОВАНИЕ СООБЩЕНИЙ И СБОРКА КАДРОВ                   
        ***********************************************************/
        class Codings
        {

            public ushort SetDatalen(byte len1, byte len2)
            {
                return BitConverter.ToUInt16(new byte[] { len1, len2 }, 0);
            }

            // Таблица для определения синдрома ошибки
            public Dictionary<byte, byte> synd_table = new Dictionary<byte, byte>
            {
                { 0x01, 0x01 },
                { 0x02, 0x02 },
                { 0x04, 0x04 },
                { 0x03, 0x08 },
                { 0x06, 0x10 },
                { 0x07, 0x20 },
                { 0x05, 0x40 }
            };

            //Получение битов контрольной суммы из исходных 4 бит //ОК
            public byte GetChecksum(byte data)
            {
                byte temp = data;
                //Порождающий полином
                byte poly = 0x58;
                short shift_count = 0;

                byte mask = 0x40;
                short counter = 20;
                byte checksum;
                while (counter > 0)
                {
                    temp = (byte)(temp ^ poly);
                    if ((temp & mask) == mask)
                    {
                        continue;
                    }
                    else {
                        if (shift_count < 3)
                        {
                            mask = (byte)(mask >> 1);
                            poly = (byte)(poly >> 1);
                            shift_count++;
                        }
                        else { break; }
                    }
                    counter--;
                }
                checksum = temp;
                return checksum;
            }
            //OK
            public byte EncodeDataBits(byte decoded_value)
            {
                byte checksum = GetChecksum((byte)(decoded_value << 3));
                byte encoded_value = (byte)((decoded_value << 3) + checksum);
                return encoded_value;
            }
            //Принимает байт с контрольной суммой, возвращает полубайт
            public byte DecodeDataBits(byte encoded_value)
            {
                byte decoded_value;
                byte checksum = GetChecksum(encoded_value);
                if (checksum == 0)
                {
                    decoded_value = (byte)(encoded_value >> 3);
                }
                else
                {

                    decoded_value = (byte)((synd_table[checksum] ^ encoded_value) >> 3);
                }
                return decoded_value;
            }
            //Принимает массив полубайтов win1251 -> выдает строку на русском языке в utf-8
            public string ByteMessageToString(byte[] message_data)
            {
                int Message_Len = (message_data.Length);
                byte[] bytestostring = new byte[Message_Len / 2];
                for (int i = 0; i < Message_Len / 2; i++)
                {
                    // 
                    if (GetChecksum((byte)(message_data[i * 2] & 0x0F)) != (byte)(message_data[i * 2] >> 3))
                    {
                        return null;
                    }
                    if (GetChecksum((byte)(message_data[i * 2 + 1] & 0x0F)) != (byte)(message_data[i * 2 + 1] >> 3))
                    {
                        return null;
                    }
                }

                for (int i = 0; i < Message_Len / 2; i++)
                {
                    byte first = (byte)((message_data[i * 2] & 0x78) << 1);
                    byte second = (byte)((message_data[i * 2 + 1] & 0x78) >> 3);
                    byte code = (byte)(first + second);
                    bytestostring[i] = code;
                }

                byte[] utf8Bytes = Encoding.Convert(WIN1251, Encoding.UTF8, bytestostring);
                string Received_String = Encoding.UTF8.GetString(utf8Bytes);

                return Received_String;
            }
            ////Принимает строку на русском языке в utf-8 -> Выдает массив полубайтов win1251
            public byte[] StringToByteMessage(string StringToSend)
            {
                byte[] BytesToSend = new byte[(StringToSend.Length) * 2];
                byte[] utf8bytes = Encoding.UTF8.GetBytes(StringToSend);
                byte[] win1251Bytes = Encoding.Convert(Encoding.UTF8, WIN1251, utf8bytes);
                string win1251string = WIN1251.GetString(win1251Bytes);

                for (int i = 0; i < win1251Bytes.Length; i++)
                {
                    byte nibble1 = EncodeDataBits((byte)(win1251Bytes[i] >> 4));
                    byte nibble2 = EncodeDataBits((byte)(win1251Bytes[i] & 0x0F));
                    BytesToSend[i * 2] = nibble1;
                    BytesToSend[(i * 2) + 1] = nibble2;
                }
                return BytesToSend;
            }
        };

        // Определение типов кадров
        public enum FrameType : byte
        {
            MEETING = 0x01,
            DISCONNECT = 0x02,
            LOGIN = 0x03,
            LOGOUT = 0x04,
            INFORMATION = 0x06,
            OPENLETTER = 0x07,
            ACK = 0x08,
            RET = 0x09
        };

        //сборка кадра для отправки в порт
        public byte[] CreateNewFrame(FrameType type, string senderstr, string datalenstr, string receiverstr, string payload, bool encoding = false)
        {
            List<byte> framebytes = new List<byte>();
            framebytes.Add((byte)0xFF);
            framebytes.Add((byte)type);
            byte sender;
            byte receiver;
            if (byte.TryParse(senderstr, out sender) && byte.TryParse(receiverstr, out receiver))
            {
                framebytes.Add((byte)sender);
                framebytes.Add((byte)receiver);

            }
            else {
                MessageBox.Show("FormNeFrame(): Не удалось преобразовать адрес машины в байт", "Error!");
                return null;
            }

            //Если тип кадра содержит данные                                 
            if (type == FrameType.INFORMATION || type == FrameType.LOGIN || type == FrameType.OPENLETTER || type == FrameType.MEETING)
            {
                UInt16 datalen;
                if (UInt16.TryParse(datalenstr, out datalen))
                {
                    byte highbyte = (BitConverter.GetBytes(datalen))[1];
                    byte lowbyte = (BitConverter.GetBytes(datalen))[0];

                    framebytes.Add((byte)highbyte);
                    framebytes.Add((byte)lowbyte);

                    if (encoding == false)
                    {
                        byte[] utf8bytes = Encoding.UTF8.GetBytes(payload);
                        byte[] win1251Bytes = Encoding.Convert(Encoding.UTF8, WIN1251, utf8bytes);
                        //string win1251string = WIN1251.GetString(win1251Bytes);
                        //byte[] bytepayload = WIN1251.GetBytes(payload);
                        framebytes.AddRange(win1251Bytes);
                    }
                    else
                    {
                        //Кодирование payload кадра если encoding = true
                    }
                }
                else {
                    MessageBox.Show("CreateNewFrame(): Не удалось преобразовать datalen  в Uint16 ", "Error!");
                    return null;
                }

            }

            framebytes.Add((byte)0xFE);
            return framebytes.ToArray();
        }


        //разбор кадра, полученного из порта. Воврпщает структуру,заполненную принятыми данными
        public DefaultFrame ParseReceivedFrame(byte[] frame, bool encoding = false)
        {
            DefaultFrame ParsedFrame = new DefaultFrame();
            if (frame.Length < 5)
            {
                MessageBox.Show("ParseReceivedFrame(). Длина кадра меньше минимальной", "Error!");
            }
            ParsedFrame.Startbyte = frame[0];
            ParsedFrame.Frametype = frame[1];
            ParsedFrame.OriginPort = ((UInt16)(frame[2])).ToString();
            ParsedFrame.DestinationPort = ((UInt16)(frame[3])).ToString();
            ParsedFrame.Stopbyte = frame[(frame.Length - 1)];

            byte type = ParsedFrame.Frametype;
            // Кадры данных типов могут быть носителями payload'а
            if (type == (byte)FrameType.INFORMATION || type == (byte)FrameType.LOGIN || type == (byte)FrameType.OPENLETTER || type == (byte)FrameType.MEETING)
            {
                // Старший байт - 4, младший - 5.  В кадре передается в формате big-endian

                ParsedFrame.datalen = BitConverter.ToUInt16(new byte[] { frame[5], frame[4] }, 0);
                List<byte> data = new List<byte>();

                // проверка на совпадение размера данных с указанным в кадре значением
                if (frame.Length != (5 + 2 + ParsedFrame.datalen))
                {
                    MessageBox.Show("ParseReceivedFrame() Длина поля данных в кадре не совпадает с указанной", "Error!");
                    ParsedFrame.ResultOfParsing = "Fail";
                    return ParsedFrame;
                }
                else {

                    for (int i = 6; i < 6 + ParsedFrame.datalen; i++)
                    {
                        data.Add(frame[i]);
                    }

                }

                ParsedFrame.MessageData = WIN1251.GetString(data.ToArray());
            }
            ParsedFrame.ResultOfParsing = "OK";

            return ParsedFrame;
        }

        public struct DefaultFrame
        {
            public byte Startbyte;
            public byte Frametype;
            public string OriginPort;
            public string DestinationPort;
            public UInt16 datalen;
            public string MessageData;
            public byte Stopbyte;

            public string ResultOfParsing;
            public string PortName;
        }
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                         КОДИРОВАНИЕ СООБЩЕНИЙ    
        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion

        public Form1()
        {
            InitializeComponent();
            string[] portslist = SerialPort.GetPortNames();

            /*************************************************************
                              ЗАДАНИЕ ПАРАМЕТРОВ КОМПОРТА  
             *************************************************************/
            //// Начальное задание параметров ком портов
            //Соединенные пары портов { COM3 <-> COM4, COM6 <-> COM7,COM8 <-> COM9 }
            serialPort1.Encoding = WIN1251;
            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Handshake = Handshake.RequestToSend;
            serialPort1.PortName = "COM3";

            serialPort2.Encoding = WIN1251;
            serialPort2.BaudRate = 9600;
            serialPort2.DataBits = 8;
            serialPort2.Parity = Parity.None;
            serialPort2.StopBits = StopBits.One;
            serialPort2.Handshake = Handshake.RequestToSend;

            serialPort2.PortName = "COM6";
            toolStripComboBox3.SelectedItem = "9600";
            SelectedBaudrate = "9600";
            toolStripComboBox1.Items.AddRange(SerialPort.GetPortNames());
            toolStripComboBox2.Items.AddRange(SerialPort.GetPortNames());
            //Port1 
            toolStripComboBox1.SelectedItem = "COM3";

            SelectedPort1Name = "COM3";

            //Port2
            toolStripComboBox2.SelectedItem = "COM6";
            SelectedPort2Name = "COM6";

            /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                            ЗАДАНИЕ ПАРАМЕТРОВ КОМПОРТА    
             ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Обработчики события PinChanged и DataReceived(не используются)
        private void serialPort1_PinChanged(object sender, SerialPinChangedEventArgs e)
        { }
        private void serialPort2_PinChanged(object sender, SerialPinChangedEventArgs e)
        { }

        //Обработка события не требуется
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        { }

        //Обработка события не требуется
        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        { }
        #endregion


        /****************************************************************
               ПОЛУЧЕНИЕ КАДРОВ ИЗ ПОРТОВ И ЗАПОЛНЕНИЕ СПИСКА ЗАДАНИЙ
         ****************************************************************/
        public enum ReceiveState { SOF_FOUND, EOF_FOUND, FREE }

        // Функции для считывания данных из ком порта в отдельном потоке
        public void Serial1_StartReceiving()
        {
            if (!serialPort1.IsOpen)
            {
                { MessageBox.Show("Порт Закрыт!", serialPort1.PortName); }
            }
            //Очистка буфера перед началом нового сеанса

            serialPort1.DiscardInBuffer();
            int bytestoread;
            while (serialPort1.IsOpen)
            {
                bytestoread = serialPort1.BytesToRead;
                //Буфер для чтения из порта
                byte[] ReceivedBytes = new byte[bytestoread];
                if (bytestoread > 0)
                {
                    try
                    {
                        //Чтение принятых данных из компорта в буферный массив принятых байтов
                        serialPort1.Read(ReceivedBytes, 0, bytestoread);
                        //Вход в критическую секцию
                        //разделяемый ресурс- Список байт, принятых из порта ReceivedFrames
                        ReceivedFrames_mutex1.WaitOne();
                        //Запись новых данных из компорта в глобальный Массив Принятых данных   
                        ReceivedFrames1.AddRange(ReceivedBytes);
                        ReceivedFrames_mutex1.ReleaseMutex();
                        //Выход из критической секции
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Error!");
                    }
                }
                Thread.Sleep(20);
            }
        }

        // просматривает Список ReceivedFrames и разбирает найденные кадры
        // Читает поток входящих байт и порта 1 и  заполняет список заданий в формате One_Task(Номер порта, Байты кадра)
        public void FindFrameInPort1()
        {
            int startbyte;
            int stopbyte;
            List<byte> Frame = new List<byte>();
            //Наачльное состояние функции при запуске
            ReceiveState State = ReceiveState.FREE;
            while (true)
            {
                //Вход в критическую секцию
                //разделяемый ресурс- Список байт, принятых из порта
                ReceivedFrames_mutex1.WaitOne();
                //Выполнение условия, только если новый кадр ещё не поступил, а старый уже обработан
                if (State == ReceiveState.FREE)
                {
                    startbyte = ReceivedFrames1.IndexOf(0xFF);
                    stopbyte = ReceivedFrames1.IndexOf(0xFE);
                    //Если Начало кадра найдено, то отмечаем новое состояние 
                    if (startbyte != -1)
                    {
                        State = ReceiveState.SOF_FOUND;
                        //Удаляем всё, что было до начала кадра, т.е. теперь буфер кадров содержит только начало кадра и возможно конец
                        ReceivedFrames1.RemoveRange(0, startbyte);
                        //Далее, если конец кадра не обнаружен, то вырезаем всё, что накопилось в буфере кадров в локальный контейнер для кадра
                        if (stopbyte == -1)
                        {
                            Frame.AddRange(ReceivedFrames1.GetRange(0, ReceivedFrames1.Count));
                            ReceivedFrames1.RemoveRange(0, ReceivedFrames1.Count);
                        }
                        //Иначе вырезаем лишь часть и разбираем кадр(Передача контейнера кадра на разбор )
                        else if (stopbyte != -1)
                        {
                            //Получение новых индексов для урезанного списка
                            startbyte = ReceivedFrames1.IndexOf(0xFF);
                            stopbyte = ReceivedFrames1.IndexOf(0xFE);
                            //int secondstartbyte =ReceivedFrames.F ;
                            Frame.AddRange(ReceivedFrames1.GetRange(0, stopbyte - startbyte + 1));
                            ReceivedFrames1.RemoveRange(0, stopbyte - startbyte + 1);
                            State = ReceiveState.EOF_FOUND;
                            //Запуск разбора кадра Frame, после этого перевод в состояние FREE И очистка контейнера кадра

                            //Запись найденного кадра в список заданий
                            TaskReceived_mutex.WaitOne();
                            TasksReceived.Add(new One_Task("Port1", Frame.ToArray()));
                            TaskReceived_mutex.ReleaseMutex();
                            Frame.Clear();

                            State = ReceiveState.FREE;
                        }
                    }
                    //Если Начало кадра не найдено и при этом система готова к приему нового кадра,
                    // значит в порт пришел мусор, его удаляем из буфера кадров
                    else
                    {
                        ReceivedFrames1.Clear();
                    }

                }
                //Если уже был обнаружен стартовый байт, значит принимае всё до конца буфера или до конечного байта 
                //Заход в эту область происходит, если разиер кадра оказался больше, чем буфер приема
                else if (State == ReceiveState.SOF_FOUND)
                {
                    stopbyte = ReceivedFrames1.IndexOf(0xFE);
                    //Если не найден стоповый байт, достаём из буфера кадров всё, что там есть 
                    if (stopbyte == -1)
                    {
                        startbyte = ReceivedFrames1.IndexOf(0xFF);
                        if ((startbyte != -1) && State == ReceiveState.SOF_FOUND)
                        {
                            MessageBox.Show("Функция FindFrame(), найден кадр без стопового байта", "Error!");
                            ReceivedFrames_mutex1.ReleaseMutex();
                            return;
                        }
                        Frame.AddRange(ReceivedFrames1.GetRange(0, ReceivedFrames1.Count));
                        ReceivedFrames1.RemoveRange(0, ReceivedFrames1.Count);

                    }
                    //Если стоповый байт найден, тогда вырезаем всё, что расположено до стопового байта, далее разбор кадра
                    else if (stopbyte != -1)
                    {
                        startbyte = ReceivedFrames1.IndexOf(0xFF);
                        if ((startbyte != -1) && State == ReceiveState.SOF_FOUND)
                        {
                            MessageBox.Show("Функция FindFrame(), найден кадр без стопового байта", "Error!");
                            ReceivedFrames_mutex1.ReleaseMutex();
                            return;
                        }
                        stopbyte = ReceivedFrames1.IndexOf(0xFE);
                        //Добавляем в контейнер кадра до стопового байта, остальное переносим в начало буфера
                        Frame.AddRange(ReceivedFrames1.GetRange(0, stopbyte + 1));
                        ReceivedFrames1.RemoveRange(0, stopbyte + 1);
                        State = ReceiveState.EOF_FOUND;
                        //Запуск разбора кадра Frame, после этого перевод в состояние FREE И очистка контейнера кадра

                        //Запись найденного кадра в список заданий
                        TaskReceived_mutex.WaitOne();
                        TasksReceived.Add(new One_Task("Port1", Frame.ToArray()));
                        TaskReceived_mutex.ReleaseMutex();
                        Frame.Clear();
                        State = ReceiveState.FREE;
                    }
                }
                // Переход в эту область не должен происходить
                else if (State == ReceiveState.EOF_FOUND)
                {
                    MessageBox.Show(" Функция FindFrame.\r\n Состояние осталось EOF_FOUND в начале прохода цикла", "Error!");
                    return;
                }

                //Выход из критической секции 
                ReceivedFrames_mutex1.ReleaseMutex();
                Thread.Sleep(10);


            }
        }

        //(Здесь больше ничего писать не надо)
        //Всё тоже самое, что и для первого порта(Serial1_StartReceiving).
        public void Serial2_StartReceiving()
        {
            if (!serialPort2.IsOpen)
            {
                MessageBox.Show("Порт Закрыт!", serialPort2.PortName);
            }
            //Очистка буфера перед началом нового сеанса
            serialPort2.DiscardInBuffer();
            int bytestoread;
            while (serialPort2.IsOpen)
            {
                bytestoread = serialPort2.BytesToRead;
                //Буфер для чтения из порта
                byte[] ReceivedBytes = new byte[bytestoread];
                if (bytestoread > 0)
                {
                    try
                    {
                        //Чтение принятых данных из компорта в буферный массив принятых байтов
                        serialPort2.Read(ReceivedBytes, 0, bytestoread);
                        //Вход в критическую секцию
                        //разделяемый ресурс- Список байт, принятых из порта ReceivedFrames
                        ReceivedFrames_mutex2.WaitOne();
                        //Запись новых данных из компорта в глобальный Массив Принятых данных   
                        ReceivedFrames2.AddRange(ReceivedBytes);
                        ReceivedFrames_mutex2.ReleaseMutex();
                        //Выход из критической секции
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Error!");
                    }
                }
                Thread.Sleep(20);
            }
        }

        //Всё тоже самое, что и для первого порта(FindFrameInPort1).
        public void FindFrameInPort2()
        {
            int startbyte;
            int stopbyte;
            List<byte> Frame = new List<byte>();
            //Наачльное состояние функции при запуске
            ReceiveState State = ReceiveState.FREE;
            while (true)
            {
                //Вход в критическую секцию
                //разделяемый ресурс- Список байт, принятых из порта
                ReceivedFrames_mutex2.WaitOne();
                //Выполнение условия, только если новый кадр ещё не поступил, а старый уже обработан
                if (State == ReceiveState.FREE)
                {
                    startbyte = ReceivedFrames2.IndexOf(0xFF);
                    stopbyte = ReceivedFrames2.IndexOf(0xFE);
                    //Если Начало кадра найдено, то отмечаем новое состояние 
                    if (startbyte != -1)
                    {
                        State = ReceiveState.SOF_FOUND;
                        //Удаляем всё, что было до начала кадра, т.е. теперь буфер кадров содержит только начало кадра и возможно конец
                        ReceivedFrames2.RemoveRange(0, startbyte);
                        //Далее, если конец кадра не обнаружен, то вырезаем всё, что накопилось в буфере кадров в локальный контейнер для кадра
                        if (stopbyte == -1)
                        {
                            Frame.AddRange(ReceivedFrames2.GetRange(0, ReceivedFrames2.Count));
                            ReceivedFrames2.RemoveRange(0, ReceivedFrames2.Count);
                        }
                        //Иначе вырезаем лишь часть и разбираем кадр(Передача контейнера кадра на разбор )
                        else if (stopbyte != -1)
                        {
                            //Получение новых индексов для урезанного списка
                            startbyte = ReceivedFrames2.IndexOf(0xFF);
                            stopbyte = ReceivedFrames2.IndexOf(0xFE);
                            //int secondstartbyte =ReceivedFrames.F ;
                            Frame.AddRange(ReceivedFrames2.GetRange(0, stopbyte - startbyte + 1));
                            ReceivedFrames2.RemoveRange(0, stopbyte - startbyte + 1);
                            State = ReceiveState.EOF_FOUND;
                            //Запуск разбора кадра Frame, после этого перевод в состояние FREE И очистка контейнера кадра
                            /*
                            string fileName = "X:\\out.txt";
                            FileStream aFile = new FileStream(fileName, FileMode.Append);
                            StreamWriter sw = new StreamWriter(aFile);
                            aFile.Seek(0, SeekOrigin.End);
                            byte[] bytearray = Frame.ToArray();
                            string Received_String = WIN1251.GetString(bytearray);
                            sw.WriteLine(Received_String);
                            sw.Close();
                            */
                            //Запись найденного кадра в список заданий
                            TaskReceived_mutex.WaitOne();
                            TasksReceived.Add(new One_Task("Port2", Frame.ToArray()));
                            TaskReceived_mutex.ReleaseMutex();
                            Frame.Clear();

                            State = ReceiveState.FREE;
                        }
                    }
                    //Если Начало кадра не найдено и при этом система готова к приему нового кадра,
                    // значит в порт пришел мусор, его удаляем из буфера кадров
                    else
                    {
                        ReceivedFrames2.Clear();
                    }

                }
                //Если уже был обнаружен стартовый байт, значит принимае всё до конца буфера или до конечного байта 
                //Заход в эту область происходит, если разиер кадра оказался больше, чем буфер приема
                else if (State == ReceiveState.SOF_FOUND)
                {
                    stopbyte = ReceivedFrames2.IndexOf(0xFE);
                    //Если не найден стоповый байт, достаём из буфера кадров всё, что там есть 
                    if (stopbyte == -1)
                    {
                        startbyte = ReceivedFrames2.IndexOf(0xFF);
                        if ((startbyte != -1) && State == ReceiveState.SOF_FOUND)
                        {
                            MessageBox.Show("Функция FindFrame(), найден кадр без стопового байта", "Error!");
                            ReceivedFrames_mutex2.ReleaseMutex();
                            return;
                        }
                        Frame.AddRange(ReceivedFrames2.GetRange(0, ReceivedFrames2.Count));
                        ReceivedFrames2.RemoveRange(0, ReceivedFrames2.Count);

                    }
                    //Если стоповый байт найден, тогда вырезаем всё, что расположено до стопового байта, далее разбор кадра
                    else if (stopbyte != -1)
                    {
                        startbyte = ReceivedFrames2.IndexOf(0xFF);
                        if ((startbyte != -1) && State == ReceiveState.SOF_FOUND)
                        {
                            MessageBox.Show("Функция FindFrame(), найден кадр без стопового байта", "Error!");
                            ReceivedFrames_mutex2.ReleaseMutex();
                            return;
                        }
                        stopbyte = ReceivedFrames2.IndexOf(0xFE);
                        //Добавляем в контейнер кадра до стопового байта, остальное переносим в начало буфера
                        Frame.AddRange(ReceivedFrames2.GetRange(0, stopbyte + 1));
                        ReceivedFrames2.RemoveRange(0, stopbyte + 1);
                        State = ReceiveState.EOF_FOUND;
                        //Запуск разбора кадра Frame, после этого перевод в состояние FREE И очистка контейнера кадра
                        /*
                        string fileName = "X:\\out.txt";
                        FileStream aFile = new FileStream(fileName, FileMode.Append);
                        StreamWriter sw = new StreamWriter(aFile);
                        aFile.Seek(0, SeekOrigin.End);
                        byte[] bytearray = Frame.ToArray();
                        string Received_String = WIN1251.GetString(bytearray);
                        sw.WriteLine(Received_String);
                        sw.Close();
                        */
                        //Запись найденного кадра в список заданий
                        TaskReceived_mutex.WaitOne();
                        TasksReceived.Add(new One_Task("Port2", Frame.ToArray()));
                        TaskReceived_mutex.ReleaseMutex();
                        Frame.Clear();
                        State = ReceiveState.FREE;
                    }
                }
                // Переход в эту область не должен происходить
                else if (State == ReceiveState.EOF_FOUND)
                {
                    MessageBox.Show(" Функция FindFrame.\r\n Состояние осталось EOF_FOUND в начале прохода цикла", "Error!");
                    return;
                }

                //Выход из критической секции 
                ReceivedFrames_mutex2.ReleaseMutex();
                Thread.Sleep(10);


            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
             Получение кадров из ком портов и внесение их в список заданий
           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /***************************************************************************
                             ВЫПОЛНЕНИЕ ЗАДАНИЙ НА ПРИЕМ 
         ***************************************************************************/

        // Новые задания появляются в списке TasksReceived из функции FindFrame1 и FindFrame2

        // Выводит адрес текущей машины
        public void settextlabel2(string text)
        {
            label2.Text = text;
        }
        public void settextlabel4(string text)
        {
            label4.Text = text;
        }
        public void TaskHandler()
        {
            while (true)
            {
                //Получение доступа к списку заданий
                TaskReceived_mutex.WaitOne();
                //Достаем Задание
                if (TasksReceived.Count != 0)
                {
                    One_Task task = TasksReceived[0];
                    //Удаление кадра и списка заданий
                    TasksReceived.RemoveAt(0);
                    TaskReceived_mutex.ReleaseMutex();

                    byte[] frame = task.Frame;
                    // не Number,  а Name
                    //имена = {"Port1","Port2"} 
                    string PortNumber = task.PortNum;

                    DefaultFrame ReceivedFrameStruct = ParseReceivedFrame(frame);

                    if (ReceivedFrameStruct.ResultOfParsing == "OK")
                    {
                        ReceivedFrameStruct.PortName = PortNumber;
                        byte frametype = ReceivedFrameStruct.Frametype;

                        //ОПРЕДЕЛЕНИЕ НЕОБХОДИМОГО ОБРАБОТЧИКА
                        if (frametype == (byte)FrameType.ACK || frametype == (byte)FrameType.RET)
                        {
                            if (frametype == (byte)FrameType.ACK)
                            {
                                string status_1;
                                string status_2;
                                string status_local;
                                Channel_status_mutex.WaitOne();
                                status_1 = Channel_status["ACK1"];
                                status_2 = Channel_status["ACK2"];
                                status_local = Channel_status["ACK_local"];
                                Channel_status_mutex.ReleaseMutex();
                                if (status_1 == "undef" || status_2 == "undef" || status_local == "undef")
                                {
                                    if (ReceivedFrameStruct.PortName == "Port1")
                                    {
                                        Ack1_mutex.WaitOne();
                                        Ack1_awaited = 0;
                                        Ack1_mutex.ReleaseMutex();
                                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[]
                                        { "Получен отчет о приеме Meeting из Port 1 \r\n" });
                                        Channel_status_mutex.WaitOne();
                                        Channel_status["ACK1"] = "Received";
                                        Channel_status_mutex.ReleaseMutex();

                                    }
                                    if (ReceivedFrameStruct.PortName == "Port2")
                                    {
                                        Ack2_mutex.WaitOne();
                                        Ack2_awaited = 0;
                                        Ack2_mutex.ReleaseMutex();
                                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[]
                                        { "Получен отчет о приеме Meeting из Port 2 \r\n" });
                                        Channel_status_mutex.WaitOne();
                                        Channel_status["ACK2"] = "Received";
                                        Channel_status_mutex.ReleaseMutex();
                                    }
                                }
                                else {
                                    string status_1_auth;
                                    string status_2_auth;

                                    Auth_status_mutex.WaitOne();
                                    status_1_auth = Auth_status["ACK1"];
                                    status_2_auth = Auth_status["ACK2"];
                                    Auth_status_mutex.ReleaseMutex();

                                    if (status_1_auth == "undef" || status_2_auth == "undef")
                                    {
                                        if (ReceivedFrameStruct.PortName == "Port1")
                                        {
                                            Ack1_mutex_Auth.WaitOne();
                                            Ack1_awaited_Auth = 0;
                                            Ack1_mutex_Auth.ReleaseMutex();
                                            BeginInvoke(new SetTextDeleg(addtotextbox1), new object[]
                                            { "Получен отчет о приеме Login из Port 1 \r\n" });
                                            Auth_status_mutex.WaitOne();
                                            Auth_status["ACK1"] = "Received";
                                            Auth_status_mutex.ReleaseMutex();

                                        }
                                        if (ReceivedFrameStruct.PortName == "Port2")
                                        {
                                            Ack2_mutex_Auth.WaitOne();
                                            Ack2_awaited_Auth = 0;
                                            Ack2_mutex_Auth.ReleaseMutex();
                                            BeginInvoke(new SetTextDeleg(addtotextbox1), new object[]
                                            { "Получен отчет о приеме Login из Port 2 \r\n" });
                                            Auth_status_mutex.WaitOne();
                                            Auth_status["ACK2"] = "Received";
                                            Auth_status_mutex.ReleaseMutex();
                                        }
                                    }
                                    else
                                    {

                                    }
                                }

                                // Отметка отправленного сообщения как доставленного 
                                // Далее, открыть возможность оправлять новые сообщения

                            }
                            //Если пришел кадр Ret, тогда повторная передача недавно переданного кадра с информацией
                            else {

                            }
                        }
                        if (frametype == (byte)FrameType.MEETING || frametype == (byte)FrameType.DISCONNECT)
                        {
                            if (frametype == (byte)FrameType.MEETING)
                            {
                                FrameReceivedMeeting(ReceivedFrameStruct);

                            }
                            else {

                            }
                        }
                        if (frametype == (byte)FrameType.LOGIN || frametype == (byte)FrameType.LOGOUT)
                        {
                            if (frametype == (byte)FrameType.LOGIN)
                            {
                                FrameReceivedLogin(ReceivedFrameStruct);

                            }
                            else {
                                //Сообщение увеломляет об отключении пользователя, далее выводится сообщение об этом, и состема останавливает работу
                                // Перевод системы в состояние логического, но не физического соединения
                            }
                        }
                        //ГОТОВО
                        if (frametype == (byte)FrameType.OPENLETTER || frametype == (byte)FrameType.INFORMATION)
                        {
                            if (frametype == (byte)FrameType.OPENLETTER)
                            {
                                FrameReceivedOpenLetter(ReceivedFrameStruct);
                            }
                            else {
                                FrameReceivedInformation(ReceivedFrameStruct);
                            }
                        }

                    }
                    else {

                        TaskToSend_mutex.WaitOne();
                        TasksToSend.Add(new One_Task(PortNumber, new byte[] { 0xFF, (byte)FrameType.RET, 0x00, 0x00, 0xFE }));
                        ////Отправка RET кадра в PortName или машине OriginPort
                        TaskToSend_mutex.ReleaseMutex();

                    }

                }
                else {
                    TaskReceived_mutex.ReleaseMutex();
                }
                Thread.Sleep(20);
            }
        }

        /*
        TODO:
         + Обработка получения Information кадра
         + Обработка получения OpenLetter кадра
         +/- Обработка получения ACK Кадра
         + Обработка получения RET Кадра 
         + Обработка получения MEETING Кадра
         - Обработка получения DISCONNECT Кадра
         ? Обработка получения LOGIN Кадра
         - Обработка получения LOGOUT Кадра
        */

        /*******************************************************
                       ОБРАБОТЧИКИ СОБЫТИЙ
        ********************************************************/
        //Готово
        public void FrameReceivedMeeting(DefaultFrame ReceivedFrame)
        {
            TaskToSend_mutex.WaitOne();
            TasksToSend.Add(new One_Task(ReceivedFrame.PortName, new byte[] { 0xFF, (byte)FrameType.ACK, 0x00, 0x00, 0xFE }));
            ////Отправка ACK кадра в PortName или машине OriginPort
            TaskToSend_mutex.ReleaseMutex();

            if (ReceivedFrame.PortName == "Port1")
            {
                Computers_Timestamps["Port1"] = ReceivedFrame.MessageData;

            }

            if (ReceivedFrame.PortName == "Port2")
            {
                Computers_Timestamps["Port2"] = ReceivedFrame.MessageData;


            }

            //Если все timestamp'ы были получены , тогда сравниваем их , затем вычисляем  адреса машин
            if (Computers_Timestamps["local"] != null && Computers_Timestamps["Port1"] != null && Computers_Timestamps["Port2"] != null)
            {

                Channel_status_mutex.WaitOne();
                Channel_status["ACK_local"] = "Received";
                Channel_status_mutex.ReleaseMutex();

                long localTime;
                long Port1Time;
                long Port2Time;
                List<long> order = new List<long>();

                bool res1 = long.TryParse(Computers_Timestamps["local"], out localTime);
                bool res2 = long.TryParse(Computers_Timestamps["Port1"], out Port1Time);
                bool res3 = long.TryParse(Computers_Timestamps["Port2"], out Port2Time);
                if (!res1 || !res2 || !res3)
                {
                    MessageBox.Show("FrameReceivedMeeting, ошибка при получении числа из Computers_timestamp", "Error!");
                }

                order.Add(localTime);
                order.Add(Port1Time);
                order.Add(Port2Time);
                order.Sort();
                PhoneBook[(order.IndexOf(localTime) + 1).ToString()] = "local";
                PhoneBook[(order.IndexOf(Port1Time) + 1).ToString()] = "Port1";
                PhoneBook[(order.IndexOf(Port2Time) + 1).ToString()] = "Port2";
                Local_Address = (order.IndexOf(localTime) + 1).ToString();

                BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Адреса абонентов получены\r\n" });
                BeginInvoke(new SetTextDeleg(settextlabel2), new object[] { Local_Address });

            }

        }

        public void FrameReceivedLogin(DefaultFrame ReceivedFrame)
        {
            TaskToSend_mutex.WaitOne();
            TasksToSend.Add(new One_Task(ReceivedFrame.PortName, new byte[] { 0xFF, (byte)FrameType.ACK, 0x00, 0x00, 0xFE }));
            ////Отправка ACK кадра в PortName или машине OriginPort
            TaskToSend_mutex.ReleaseMutex();

            if (ReceivedFrame.PortName == "Port1")
            {
                AuthData_mutex.WaitOne();
                AuthData["Port1"] = ReceivedFrame.MessageData;
                AuthData_mutex.ReleaseMutex();


            }

            if (ReceivedFrame.PortName == "Port2")
            {
                AuthData_mutex.WaitOne();
                AuthData["Port2"] = ReceivedFrame.MessageData;
                AuthData_mutex.ReleaseMutex();


            }

            string port1_auth_data, port2_auth_data;
            AuthData_mutex.WaitOne();

            port1_auth_data = AuthData["Port1"];
            port2_auth_data = AuthData["Port2"];
            AuthData_mutex.ReleaseMutex();

            //Если все login'ы были получены
            if (port1_auth_data != null && port2_auth_data != null)
            {
                Auth_status_mutex.WaitOne();
                Auth_status["ACK_local"] = "Received";
                Auth_status_mutex.ReleaseMutex();
                BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { "Логины абонентов получены\r\n" });
            }

        }

        //Готово
        public void FrameReceivedRet(DefaultFrame frame)
        {
            // Здесь получаем значение last send frame о отправляем его вкомпорт
            TaskToSend_mutex.WaitOne();
            if (frame.PortName == "Port1")
            {
                TasksToSend.Add(new One_Task("Port1", LastFrameSenttoPort1));
            }
            if (frame.PortName == "Port2")
            {
                TasksToSend.Add(new One_Task("Port1", LastFrameSenttoPort1));
            }

            TaskToSend_mutex.ReleaseMutex();

        }

        // Не используется
        public void FrameReceivedDisconnect(string PortName, byte[] frame)
        {


            //Если пришел Disconnect Кадр, то
            // меняем состояние системы на physical Disconnect
            // И очищаем словари соответствия портов, номеров машин и прочего
            PhoneBook["0"] = null;
            PhoneBook["1"] = null;
            PhoneBook["2"] = null;

            PortAssoc["Port1"] = null;
            PortAssoc["Port2"] = null;

            Computers_Timestamps["Port1"] = null;
            Computers_Timestamps["Port2"] = null;
            Computers_Timestamps["local"] = null;


            if (serialPort1.IsOpen)
            { serialPort1.Close(); }
            if (serialPort2.IsOpen)
            { serialPort2.Close(); }


        }
        public void FrameReceivedLogout(string PortNmae, byte[] frame)
        {

            //Здесь очистка таблицы логинов (соответствие номера машины и имени пользователя)

            // Здесь нужно запустить выход из состояния авторизованности
        }

        // Обработчик открытия письма (готов)
        // В БД ищется письмо, затем отмечается как прочитанное
        public void FrameReceivedOpenLetter(DefaultFrame frame)
        {
            //Подключение к бд, поиск письма с тем id, который указан в принятом пакете

            long LetterId = long.Parse(frame.MessageData);

            using (CourseDB db = new CourseDB())
            {
                var result = db.outbox.SingleOrDefault(x => x.id == LetterId);
                if (result != null)
                {
                    if (result.status == "Прочитано")
                    {

                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { $" Сообщение : письмо с id = {LetterId} уже было прочитано\r\n" });
                    }
                    else {
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { $" Сообщение : письмо с id = {LetterId} было прочитано\r\n" });
                        result.status = "Прочитано";
                        db.SaveChanges();
                        //Отметка для обновления формы отправленных сообщений
                        Outbox_update_mutex.WaitOne();
                        Outbox_update_needed = true;
                        Outbox_update_mutex.ReleaseMutex();
                    }
                }
            }


            //После этого обновление таблицы принятых сообщений на ui                       

        }

        //Готов
        public UInt16 GetDatalen(byte len1, byte len2)
        {
            return BitConverter.ToUInt16(new byte[] { len1, len2 }, 0);
        }

        //Готов
        public void FrameReceivedInformation(DefaultFrame local_frame)
        {
            byte[] framedata = WIN1251.GetBytes(local_frame.MessageData);
            string framestr = WIN1251.GetString(framedata);
            inbox_class message_data = JsonConvert.DeserializeObject<inbox_class>(local_frame.MessageData);
            if (true)
            {
                BeginInvoke
                    (new SetTextDeleg(addtotextbox1),
                    new object[] { $"Получено письмо от {message_data.sender}" + "\r\n" });

                long foreign_id = long.Parse(message_data.id);
                inbox received_letter = new inbox();
                received_letter.foreign_id = foreign_id;
                received_letter.re = message_data.re;
                received_letter.msg = message_data.msg;
                received_letter.recepient = message_data.recepient;
                received_letter.sender = message_data.sender;
                received_letter.status = "Принято";
                using (CourseDB db = new CourseDB())
                {
                    db.inbox.Add(received_letter);
                    db.SaveChanges();
                    db.Dispose();
                }
                Inbox_update_mutex.WaitOne();
                Inbox_update_needed = true;
                Inbox_update_mutex.ReleaseMutex();

                TaskToSend_mutex.WaitOne();
                TasksToSend.Add(new One_Task(local_frame.PortName,
                                CreateNewFrame(FrameType.ACK, "0", null, "0", null, false))
                                );
                TaskToSend_mutex.ReleaseMutex();
            }
            else
            {
                // Если сообщение кривое, то отправляем RET в порт из которого он был принят
                TaskToSend_mutex.WaitOne();
                TasksToSend.Add(new One_Task(local_frame.PortName,
                    CreateNewFrame(FrameType.RET, "0", null, "0", null, false)
                    ));
                TaskToSend_mutex.ReleaseMutex();
            }
            //Декодирование содержимого посылки. Если всё ок, отправляем ACK отправителю,
            //письмо помещается в БД( таблицу Inbox) со статусом доставлено
            //Если Содержит ошибку, отправляем RET отправителю

        }

        /* 
        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         функции для исполнения заданий, полученных из списка с заданиями
        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        */

        //Обработка ACK кадра необходима для подтверждения приема 
        // Посылка MEETING или LOGIN или INFORMATION кадра адресату,
        // Этот кадр отмечается, как переданный, но пока не распознанный
        // затем ожидание ответа - ACK или RET.
        // Если ACK тогда отмеченный кадр помечается как доставленный(если это письмо)
        // Если это Login или Meeting кадр, тогда система переводится в состояние "логически соединен" или "физически соединен"

        /*
        **********************************************************
                          ОТПРАВКА НОВОГО ПИСЬМА 
        **********************************************************   
        */
        private volatile Type _dependency;

        public void MyClass()
        {
            _dependency = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }
        public void SendNewLetterButton_Click(object sender, EventArgs e)
        {
            string Re_string = ReTextbox.Text;
            string Receiver_name = ReceiverComboBox.SelectedItem.ToString();
            string Letter_Message = LetterTextBox.Text;

            if (PhoneBook["1"] != null && PhoneBook["2"] != null && PhoneBook["3"] != null)
            {
                if (AuthData["Port1"] != null && AuthData["Port2"] != null && AuthData["local"] != null)
                {
                    using (CourseDB db = new CourseDB())
                    {
                        outbox letter = new outbox();
                        letter.re = Re_string;
                        letter.sender = AuthData["local"];
                        letter.recepient = Receiver_name;
                        letter.status = "Отправлено";
                        letter.msg = Letter_Message;
                        db.outbox.Add(letter);
                        db.SaveChanges();
                    }
                    long max;
                    outbox a;
                    using (CourseDB db = new CourseDB())
                    {
                        max = db.outbox.Max(x => x.id);
                        a = db.outbox.FirstOrDefault(x => x.id == max);
                    }
                    string letter_local_id = a.id.ToString();
                    string sender_addr = PhoneBook.FirstOrDefault(x => x.Value == "local").Key;

                    string receiver_port = AuthData.FirstOrDefault(x => x.Value == Receiver_name).Key;
                    string receiver_addr = PhoneBook.FirstOrDefault(x => x.Value == receiver_port).Key;
                    outbox_class letter_payload_obj = new outbox_class(a);
                    string letter_payload_string = JsonConvert.SerializeObject(letter_payload_obj);
                    string letter_len = letter_payload_string.Length.ToString();
                    byte[] Letter_frame_to_send = CreateNewFrame(FrameType.INFORMATION, sender_addr,
                        letter_len, receiver_addr, letter_payload_string, false);
                    TaskToSend_mutex.WaitOne();
                    TasksToSend.Add(new One_Task(receiver_port, Letter_frame_to_send));
                    TaskToSend_mutex.ReleaseMutex();

                    Outbox_update_mutex.WaitOne();
                    Outbox_update_needed = true;
                    Outbox_update_mutex.ReleaseMutex();
                }
                else
                {
                    MessageBox.Show("SendNewLetterButton_Click(): login'ы не определены", "Error!");
                }
            }
            else
            {
                MessageBox.Show("SendNewLetterButton_Click(): Локальная машина не подключена к сети", "Error!");
            }


        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        // Делегат используется для записи в UI control из потока не-UI
        private delegate void SetTextDeleg(string text);
        private delegate void FillComboBoxDeleg();
        public void FillReceiverComboBox()
        {
            AuthData_mutex.WaitOne();
            string[] values = AuthData.Values.ToArray();
            ReceiverComboBox.Items.AddRange(values);
            AuthData_mutex.ReleaseMutex();
        }
        public void settextbox1(string text)
        {
            textBox1.Text = text;
        }

        public void addtotextbox1(string text)
        {
            textBox1.Text = textBox1.Text + text;
        }


        #region Открытие портов
        public void OpenSerial1()
        {
            SelectedPort1Name_mutex.WaitOne();

            try
            {
                serialPort1.PortName = SelectedPort1Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, serialPort1.PortName);

            }


            SelectedPort1Name_mutex.ReleaseMutex();
            //serialPort2.PortName = "COM3";

            SelectedBaudrate_mutex.WaitOne();
            serialPort1.BaudRate = int.Parse(SelectedBaudrate);
            SelectedBaudrate_mutex.ReleaseMutex();

            try
            {


                serialPort1.Open();

                serialPort1.DiscardOutBuffer();
                serialPort1.DiscardInBuffer();
                serialPort1.DtrEnable = true;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, serialPort1.PortName);
                return;
            }

        }

        public void OpenSerial2()
        {
            SelectedPort2Name_mutex.WaitOne();
            try
            {
                serialPort2.PortName = SelectedPort2Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, serialPort1.PortName);

            };
            SelectedPort2Name_mutex.ReleaseMutex();
            // serialPort2.PortName = "COM6";
            SelectedBaudrate_mutex.WaitOne();
            serialPort2.BaudRate = int.Parse(SelectedBaudrate);
            SelectedBaudrate_mutex.ReleaseMutex();
            try
            {

                serialPort2.Open();

                serialPort2.DiscardOutBuffer();
                serialPort2.DiscardInBuffer();
                serialPort2.DtrEnable = true;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, serialPort2.PortName);
                return;
            }

        }
        #endregion

        /************************************************************
                       ПРОВЕРКА СОСТОЯНИЯ ЛИНИЙ CD и DSR
        ************************************************************/
        // Делегат используется для записи в UI control из потока не-UI
        private delegate void SetPortState(string text);
        public void setport1state(string text)
        {
            port1state_label.Text = text;
        }
        public void setport2state(string text)
        {
            port2state_label.Text = text;
        }

        public void serial1_monitor()
        {
            while (true)
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.CDHolding || serialPort1.DsrHolding)
                    {
                        BeginInvoke(new SetTextDeleg(setport1state), new object[] { "Подключен" });
                    }
                    else { BeginInvoke(new SetTextDeleg(setport1state), new object[] { "Отключен" }); }
                }

                Thread.Sleep(200);
            }

        }
        public void serial2_monitor()
        {
            while (true)
            {
                if (serialPort2.IsOpen)
                {
                    if (serialPort2.CDHolding || serialPort2.DsrHolding)
                    {
                        BeginInvoke(new SetTextDeleg(setport2state), new object[] { "Подключен" });
                    }
                    else { BeginInvoke(new SetTextDeleg(setport2state), new object[] { "Отключен" }); }
                }

                Thread.Sleep(200);
            }

        }

        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//




        /************************************************************
                         ЗДЕСЬ НАЧАЛО ВСЕГО
        ************************************************************/

        private void button1_Click(object sender, EventArgs e)
        {

            Thread SerOpenthread1 = new Thread(OpenSerial1);
            Thread SerOpenthread2 = new Thread(OpenSerial2);
            SerOpenthread1.IsBackground = true;
            SerOpenthread1.Start();
            SerOpenthread2.IsBackground = true;
            SerOpenthread2.Start();
            Thread.Sleep(300);

            if (!serialPort1.IsOpen || !serialPort2.IsOpen)
            {
                MessageBox.Show("Не удалось открыть порты", "Warning");
                return;
            }
            else
            {

                DateTime foo = DateTime.UtcNow;
                long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                string timestamp = unixTime.ToString();
                //Отметка локального времени
                Computers_Timestamps["local"] = timestamp;
            }


            Thread Channel_Trackerthr = new Thread(Channel_Tracker);
            Channel_Trackerthr.IsBackground = true;
            Channel_Trackerthr.Start();

            //После запуска потока, метод StartReceiving Осуществляет прием из входного буфера в программный буфер принятых байтов
            Thread ReceiverThr1 = new Thread(Serial1_StartReceiving);
            ReceiverThr1.IsBackground = true;
            ReceiverThr1.Start();

            //FindFrame просматривает програмный буфер принятых байтов и составляет кадры, зате помещает  в список кадров(заданий)
            Thread ParseFrameThr1 = new Thread(FindFrameInPort1);
            ParseFrameThr1.IsBackground = true;
            ParseFrameThr1.Start();

            //После запуска потока, метод StartReceiving Осуществляет прием из входного буфера в программный буфер принятых байтов
            Thread ReceiverThr2 = new Thread(Serial2_StartReceiving);
            ReceiverThr2.IsBackground = true;
            ReceiverThr2.Start();

            //FindFrame просматривает програмный буфер принятых байтов и составляет кадры, зате помещает  в список кадров(заданий)
            Thread ParseFrameThr2 = new Thread(FindFrameInPort2);
            ParseFrameThr2.IsBackground = true;
            ParseFrameThr2.Start();

            Thread serial1_mon_thr = new Thread(serial1_monitor);
            serial1_mon_thr.IsBackground = true;
            serial1_mon_thr.Start();

            Thread serial2_mon_thr = new Thread(serial2_monitor);
            serial2_mon_thr.IsBackground = true;
            serial2_mon_thr.Start();

            Thread TaskHandlerThr = new Thread(TaskHandler);
            TaskHandlerThr.IsBackground = true;
            TaskHandlerThr.Start();

            Thread TaskToSendHandlerThr = new Thread(TaskToSendHandler);
            TaskToSendHandlerThr.IsBackground = true;
            TaskToSendHandlerThr.Start();

            /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                Запуск потоков для приема кадров в систему
              ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            //Здесь запуск функции выполнения заданий
            //Метод просматривает очередь заданий, вырезает самое старое задание, затем выполняет его, мспользуя отдельные функции

            //После открытия портов необходимо 10 раз отправить 
            //кадр для установления логического соединения "MEETING",
            //Ожидая ответа от соседней машины

            // Thread thrinfo = Thread.CurrentThread;
            // string thr = thrinfo.IsAlive.ToString();
            // BeginInvoke(new SetTextDeleg(settextbox1), new object[] { thr });
        }
        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            { serialPort1.Close(); }
            if (serialPort2.IsOpen)
            { serialPort2.Close(); }
        }




        /***************************************************************************
                            ВЫПОЛНЕНИЕ ЗАДАНИЙ НА ОТПРАВКУ 
        ***************************************************************************/

        // Больше ничего писать не надо
        // Внимание. При попытке передать данные в отключенный порт принимающей стороны,
        // поток не будет продолжен, пока локальная машина не передаст данные
        // мб следует отключить flow control = dtr/cts -> flow control = None
        public void TaskToSendHandler()
        {
            while (true)
            {
                //Получение задания из очереди на отправку кадра в порт
                TaskToSend_mutex.WaitOne();
                if (TasksToSend.Count != 0)
                {
                    One_Task TaskToSend = TasksToSend[0];
                    TasksToSend.RemoveAt(0);
                    TaskToSend_mutex.ReleaseMutex();

                    string PortName = TaskToSend.PortNum;
                    byte[] frametosend = TaskToSend.Frame;

                    if (PortName == "Port1")
                    {
                        try
                        { serialPort1.Write(WIN1251.GetString(frametosend)); }
                        catch (Exception ex)
                        { MessageBox.Show(ex.ToString(), "Error!"); }
                    }
                    else if (PortName == "Port2")

                    {
                        try
                        { serialPort2.Write(WIN1251.GetString(frametosend)); }
                        catch (Exception ex)
                        { MessageBox.Show(ex.ToString(), "Error!"); }
                    }

                    else { MessageBox.Show("TaskToSendHandler() Нет такого порта", "Error!"); }

                }
                else {
                    TaskToSend_mutex.ReleaseMutex();
                }

                Thread.Sleep(20);
            }
        }

        /*
         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                             ВЫПОЛНЕНИЕ ЗАДАНИЙ НА ОТПРАВКУ 
         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        */

        //назначение функции- отсылать timestamp локальной машины в два других порта, до тех пор, пока не будет получена квитанция 1,2
        public void Establish_physical()
        {

            string timestamp = Computers_Timestamps["local"];
            Ack1_mutex.WaitOne();
            Ack1_awaited = 0;
            Ack1_mutex.ReleaseMutex();

            Ack2_mutex.WaitOne();
            Ack2_awaited = 0;
            Ack2_mutex.ReleaseMutex();
            int ack1;
            int ack2;
            int counter = 0;
            while (true)
            {
                //Начальная попытка отправить meeting
                if (counter == 0)
                {
                    Ack1_mutex.WaitOne();
                    ack1 = Ack1_awaited;
                    Ack1_mutex.ReleaseMutex();

                    Ack2_mutex.WaitOne();
                    ack2 = Ack2_awaited;
                    Ack2_mutex.ReleaseMutex();
                    if (ack1 == 0 && ack2 == 0)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame1 = CreateNewFrame(FrameType.MEETING, "0", (timestamp.Length).ToString(), "0", timestamp);
                        TasksToSend.Add(new One_Task("Port1", frame1));

                        Ack1_mutex.WaitOne();
                        Ack1_awaited = 1;
                        Ack1_mutex.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток соединения (пк на порт1) : " + counter.ToString() + "\r\n" });


                        TaskToSend_mutex.WaitOne();
                        byte[] frame2 = CreateNewFrame(FrameType.MEETING, "0", (timestamp.Length).ToString(), "0", timestamp);
                        TasksToSend.Add(new One_Task("Port2", frame2));

                        Ack2_mutex.WaitOne();
                        Ack2_awaited = 1;
                        Ack2_mutex.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток соединения (пк на порт2) : " + counter.ToString() + "\r\n" });
                        Thread.Sleep(4000);
                        continue;
                    }

                }
                if (counter < 10 && counter != 0)
                {
                    Ack1_mutex.WaitOne();
                    ack1 = Ack1_awaited;
                    Ack1_mutex.ReleaseMutex();

                    Ack2_mutex.WaitOne();
                    ack2 = Ack2_awaited;
                    Ack2_mutex.ReleaseMutex();
                    if (ack1 == 0 && ack2 == 0)
                    {
                        Channel_status_mutex.WaitOne();
                        Channel_status["ACK1"] = "Received";
                        Channel_status_mutex.ReleaseMutex();

                        Channel_status_mutex.WaitOne();
                        Channel_status["ACK2"] = "Received";
                        Channel_status_mutex.ReleaseMutex();

                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { "Переданные данные доставлены  \r\n" });
                        return;
                    }
                    if (ack1 == 1)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame1 = CreateNewFrame(FrameType.MEETING, "0", (timestamp.Length).ToString(), "0", timestamp);
                        TasksToSend.Add(new One_Task("Port1", frame1));

                        Ack1_mutex.WaitOne();
                        Ack1_awaited = 1;
                        Ack1_mutex.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток соединения (пк на порт1) : " + counter.ToString() + "\r\n" });

                    }
                    if (ack2 == 1)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame2 = CreateNewFrame(FrameType.MEETING, "0", (timestamp.Length).ToString(), "0", timestamp);
                        TasksToSend.Add(new One_Task("Port2", frame2));

                        Ack2_mutex.WaitOne();
                        Ack2_awaited = 1;
                        Ack2_mutex.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток соединения (пк на порт2) : " + counter.ToString() + "\r\n" });

                    }
                }
                else if (counter != 0)
                {
                    BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Соединение не удалось. Попыток: " + counter.ToString() + "\r\n" });
                    return;
                }
                Thread.Sleep(4000);
            }
        }

        //Отсылает логины, ждет ack
        public void Establish_Logical()
        {
            AuthData_mutex.WaitOne();
            string local_auth_name = AuthData["local"];
            AuthData_mutex.ReleaseMutex();

            Ack1_mutex_Auth.WaitOne();
            Ack1_awaited_Auth = 0;
            Ack1_mutex_Auth.ReleaseMutex();

            Ack2_mutex_Auth.WaitOne();
            Ack2_awaited_Auth = 0;
            Ack2_mutex_Auth.ReleaseMutex();

            int ack1;
            int ack2;
            int counter = 0;

            while (true)
            {
                Auth_status_mutex.WaitOne();
                string s1, s2, sl;
                s1 = Auth_status["ACK1"];
                s2 = Auth_status["ACK2"];

                Auth_status_mutex.ReleaseMutex();
                if (s1 != "undef" && s2 != "undef")
                {
                    return;
                }
                //Начальная попытка отправить LOGIN
                if (counter == 0)
                {
                    Ack1_mutex_Auth.WaitOne();
                    ack1 = Ack1_awaited_Auth;
                    Ack1_mutex_Auth.ReleaseMutex();

                    Ack2_mutex_Auth.WaitOne();
                    ack2 = Ack2_awaited_Auth;
                    Ack2_mutex_Auth.ReleaseMutex();

                    if (ack1 == 0 && ack2 == 0)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame1 = CreateNewFrame(FrameType.LOGIN, "0", (local_auth_name.Length).ToString(), "0", local_auth_name);
                        TasksToSend.Add(new One_Task("Port1", frame1));

                        Ack1_mutex_Auth.WaitOne();
                        Ack1_awaited_Auth = 1;
                        Ack1_mutex_Auth.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток логин соединения (пк на порт1) : " + counter.ToString() + "\r\n" });


                        TaskToSend_mutex.WaitOne();
                        byte[] frame2 = CreateNewFrame(FrameType.LOGIN, "0", (local_auth_name.Length).ToString(), "0", local_auth_name);
                        TasksToSend.Add(new One_Task("Port2", frame2));

                        Ack2_mutex_Auth.WaitOne();
                        Ack2_awaited_Auth = 1;
                        Ack2_mutex_Auth.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток логин соединения (пк на порт2) : " + counter.ToString() + "\r\n" });
                        Thread.Sleep(4000);
                        continue;
                    }

                }
                if (counter < 10 && counter != 0)
                {
                    Ack1_mutex_Auth.WaitOne();
                    ack1 = Ack1_awaited_Auth;
                    Ack1_mutex_Auth.ReleaseMutex();

                    Ack2_mutex_Auth.WaitOne();
                    ack2 = Ack2_awaited_Auth;
                    Ack2_mutex_Auth.ReleaseMutex();
                    if (ack1 == 0 && ack2 == 0)
                    {
                        Auth_status_mutex.WaitOne();
                        Auth_status["ACK1"] = "Received";
                        Auth_status_mutex.ReleaseMutex();

                        Auth_status_mutex.WaitOne();
                        Auth_status["ACK2"] = "Received";
                        Auth_status_mutex.ReleaseMutex();
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { "Переданные логины доставлены  \r\n" });

                        return;
                    }
                    if (ack1 == 1)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame1 = CreateNewFrame(FrameType.LOGIN, "0", (local_auth_name.Length).ToString(), "0", local_auth_name);
                        TasksToSend.Add(new One_Task("Port1", frame1));

                        Ack1_mutex_Auth.WaitOne();
                        Ack1_awaited_Auth = 1;
                        Ack1_mutex_Auth.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток передать логин (пк на порт1) : " + counter.ToString() + "\r\n" });

                    }
                    if (ack2 == 1)
                    {
                        TaskToSend_mutex.WaitOne();
                        byte[] frame2 = CreateNewFrame(FrameType.LOGIN, "0", (local_auth_name.Length).ToString(), "0", local_auth_name);
                        TasksToSend.Add(new One_Task("Port2", frame2));

                        Ack2_mutex_Auth.WaitOne();
                        Ack2_awaited_Auth = 1;
                        Ack2_mutex_Auth.ReleaseMutex();

                        TaskToSend_mutex.ReleaseMutex();
                        counter++;
                        BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Попыток передать логин (пк на порт2) : " + counter.ToString() + "\r\n" });

                    }
                }
                else if (counter != 0)
                {
                    BeginInvoke(new SetTextDeleg(addtotextbox1), new object[] { " Передать логин не удалось. Попыток: " + counter.ToString() + "\r\n" });
                    return;
                }
                Thread.Sleep(4000);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Thread connect_physthr = new Thread(Establish_physical);
            connect_physthr.Start();

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = toolStripComboBox1.SelectedItem.ToString();
            SelectedPort1Name_mutex.WaitOne();
            SelectedPort1Name = selectedState;
            SelectedPort1Name_mutex.ReleaseMutex();
        }

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = toolStripComboBox3.SelectedItem.ToString();
            SelectedBaudrate_mutex.WaitOne();
            SelectedBaudrate = selectedState;
            SelectedBaudrate_mutex.ReleaseMutex();
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = toolStripComboBox2.SelectedItem.ToString();
            SelectedPort2Name_mutex.WaitOne();
            SelectedPort2Name = selectedState;
            SelectedPort2Name_mutex.ReleaseMutex();
        }

        //Закрытие портов
        private void button4_Click(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Handshake = Handshake.None;
                    Thread.Sleep(300);
                    serialPort1.Close();
                    MessageBox.Show("Порт 1 был Закрыт", "Error!");
                    serialPort1.Handshake = Handshake.RequestToSend;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error!");
                }
            }

            if (serialPort2.IsOpen)
            {
                try
                {

                    serialPort2.Handshake = Handshake.None;
                    Thread.Sleep(300);
                    serialPort2.Close();
                    MessageBox.Show("Порт 2 был Закрыт", "Error!");
                    serialPort2.Handshake = Handshake.RequestToSend;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error!");
                }
            }

        }

        /*************************************************************************
                            ОТКРЫТИЕ ПАПОК С ПИСЬМАМИ
        **************************************************************************/

        public void Channel_Tracker()
        {
            bool stat = false;
            bool local_auth_ready = false;
            bool form_was_shown = false;
            while (true)
            {
                Channel_status_mutex.WaitOne();
                string st1 = Channel_status["ACK1"];
                string st2 = Channel_status["ACK2"];
                string st_loc = Channel_status["ACK_local"];
                Channel_status_mutex.ReleaseMutex();


                if ((st1 != "undef" && st2 != "undef" && st_loc != "undef") && local_auth_ready == false && form_was_shown == false)
                {
                    AuthForm form = new AuthForm(this);
                    form.ShowDialog();
                    stat = true;
                    form_was_shown = true;
                }

                AuthData_mutex.WaitOne();
                string local_auth = AuthData["local"];
                AuthData_mutex.ReleaseMutex();

                if ((local_auth != null) && (stat == true))
                {
                    local_auth_ready = true;
                }

                if (stat == true && local_auth_ready == true && form_was_shown == true)
                {
                    //Запуск попыток переслать свой логин 
                    Thread Establish_Logicalthr = new Thread(Establish_Logical);
                    Establish_Logicalthr.IsBackground = true;
                    Establish_Logicalthr.Start();

                    Thread Auth_Trackerthr = new Thread(Auth_Tracker);
                    Auth_Trackerthr.IsBackground = true;
                    Auth_Trackerthr.Start();

                    BeginInvoke(new SetTextDeleg(settextlabel4), new object[] { local_auth });

                    return;
                }
                Thread.Sleep(200);
            }

        }

        //Поток, следящий за Авторизацией
        public void Auth_Tracker()
        {
            bool stat = false;
            while (true)
            {
                Auth_status_mutex.WaitOne();
                if (Auth_status["ACK1"] != "undef" && Auth_status["ACK_local"] != "undef" && Auth_status["ACK2"] != "undef")
                {
                    stat = true;
                }
                Auth_status_mutex.ReleaseMutex();
                if (stat == true)
                {

                    BeginInvoke(new SetTextDeleg(addtotextbox1), new object[]
                                       { "Авторизация всех пользователей прошла успешно \r\n" });
                    BeginInvoke(new FillComboBoxDeleg(FillReceiverComboBox), new object[] { });

                    return;
                }
                Thread.Sleep(200);
            }

        }

        //Открытие папки входящие
        private void входящиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 inbox_folder = new Form2(this);
            inbox_folder.Show();
        }

        //Инициация обновления формы inbox
        private void button2_Click(object sender, EventArgs e)
        {
            Inbox_update_mutex.WaitOne();
            Inbox_update_needed = true;
            Inbox_update_mutex.ReleaseMutex();
        }

        //Открытие папки исходящие
        private void исходящиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 outbox_folder = new Form3(this);
            outbox_folder.Show();
        }

        //Инициация обновления формы outbox
        private void button5_Click(object sender, EventArgs e)
        {
            Outbox_update_mutex.WaitOne();
            Outbox_update_needed = true;
            Outbox_update_mutex.ReleaseMutex();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //{"id":"1","re":"Shalom, jay son","msg":"Json desialize test","status":"Отправлено"}
            string str = LetterTextBox.Text;
            //outbox_class outbox_letter;
            //outbox_class json_letter = JsonConvert.DeserializeObject<outbox_class>(str);
            byte[] utf8bytes = Encoding.UTF8.GetBytes(str);
            byte[] win1251Bytes = Encoding.Convert(Encoding.UTF8, WIN1251, utf8bytes);
            string win1251string = WIN1251.GetString(win1251Bytes);


            List<string> a = new List<string>();
            for (int i = 0; i < win1251Bytes.Length; i++)
            {
                a.Add((win1251Bytes[i]).ToString("X2"));
            }
            textBox1.Text = "$" + String.Join("$", a.ToArray());
            string len = LetterTextBox.Text.Length.ToString();
        }
    }
}


