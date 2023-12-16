using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.View;
namespace ComProject
{
    public enum ComTitle
    {
        Connect,Disconnect,
        P1_Valid, P1_Unvalid,
        P2_Valid, P2_Unvalid,
        P3_Valid, P3_Unvalid,
        P4_Valid, P4_Unvalid,
    }
    public class ComEvent:EventArgs
    {
        public ComTitle Title = ComTitle.Connect;
        public byte OpCode = 0;
        public double Value;
        public ComEvent(ComTitle title)
        {
            Title = title;
        }
        public ComEvent(ComTitle title, double value)
        {
            Title = title;
            Value = value;
        }
        public ComEvent(byte title, double value)
        {
            OpCode = title;
            Value = value;
        }

    }
    public class ClassComConnection
    {
        private System.IO.Ports.SerialPort serialPort1;
        Thread thGetDataPort;
        Thread thInterpreter;
        public bool flagGetData = false;
        List<byte> dataBuf = new List<byte>();
        public event EventHandler<ComEvent> ComEventHandler;
        public byte DestAddress;
        public uint FrameCounter = 0;
        public List<byte> GetInffo;
        public void SetSerialPort(string com)
        {
            serialPort1 = new System.IO.Ports.SerialPort(com);
        }
        public void Connect(string com)
        {
            try
            {
                serialPort1 = new System.IO.Ports.SerialPort(com);
               // thGetDataPort = new Thread(new ThreadStart(fun_GetDataSerial));
                thInterpreter = new Thread(new ThreadStart(fun_GetData));
                serialPort1.BaudRate = 115200;
                serialPort1.Open();
                flagGetData = true;
               // thGetDataPort.Start();
                thInterpreter.Start();
                DestAddress = 0x12;
            }
            catch
            {
                MessageBox.Show("Selected port isnot valid.   ", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //object _lockGetData = new object();
        private void fun_GetDataSerial()
        {
            while (flagGetData)
            {
                byte[] data = new byte[20];
                int count = serialPort1.Read(data, 0, 20);
                //Monitor.Enter(_lockGetData);
                for (int i = 0; i < count; i++)
                    dataBuf.Add(data[i]);
                //Monitor.Exit(_lockGetData);

                Thread.Sleep(10);
            }
        }
        private void fun_GetData()
        {
            while (flagGetData)
            {
                byte[] data = new byte[1024];
                int count = serialPort1.Read(data, 0, 1024);
                for (int i = 0; i < count; i++)
                    dataBuf.Add(data[i]);

                int ind = 0;
                while (dataBuf.Count >= 20)
                {
                    try
                    {
                        if (dataBuf[ind] == 0x48 &&
                        dataBuf[ind + 1] == 0x45 &&
                        dataBuf[ind + 2] == 0x41 &&
                        dataBuf[ind + 3] == 0x44)
                        {
                            try
                            {
                                ind += 4;//header
                                ind += 2;//frame counter
                                ind += 1;//data type
                                ind += 1;//from
                                ind += 1;//des

                                byte opCode = dataBuf[ind]; ind++;
                                ind++;
                                ind++;
                                ind++;
                                int size = dataBuf[ind]; ind++;


                                byte[] tByte = new byte[size];
                                for (int i = 0; i < size; i++)
                                    tByte[(size - 1) - i] = dataBuf[ind + i];

                                ind += size;
                                ind += 4;

                                short _val = BitConverter.ToInt16(tByte, 0);
                                ComEventHandler(this, new ComEvent(opCode, _val));

                                dataBuf.RemoveRange(0, ind);
                                ind = 0;
                                
                            }
                            catch { }
                        }
                        else
                            ind++;
                    }
                    catch
                    {
                        dataBuf.RemoveAt(0);
                    }
                }
                Thread.Sleep(50);
            }
        }
        public void ClosePort()
        {
            try
            {
                if (serialPort1 != null)
                {
                    SendData(opCode.LowRefreshTime, 0, 0);
                    Thread.Sleep(100);
                    SendData(opCode.PAutoRefresh, 0, 0);
                }
                ComEventHandler(this, new ComEvent(ComTitle.Disconnect));
                flagGetData = false;
                if (serialPort1 != null)
                    serialPort1.Close();
                serialPort1 = null;
                // thGetDataPort.Abort();
                if (thInterpreter != null)
                    thInterpreter.Abort();
                dataBuf.Clear();
            }
            catch { }
        }
        public void SendData(opCode opcode, int value, byte rw)
        {
            if (serialPort1 == null)
                MessageBox.Show("Device isnot connected");
            else
            {

                List<byte> sData = new List<byte>();
                sData.Add(0x48);
                sData.Add(0x45);
                sData.Add(0x41);
                sData.Add(0x44);

                byte[] temp1 = BitConverter.GetBytes(FrameCounter);
                sData.Add(temp1[0]);
                sData.Add(temp1[1]);

                sData.Add(0x04);
                sData.Add(0x10);
                sData.Add(0x12);
                sData.Add((byte)opcode);
                sData.Add(0);
                sData.Add(0);
                sData.Add(0);
                sData.Add(2);
                byte[] temp2 = BitConverter.GetBytes(value);
                sData.Add(temp2[1]);
                sData.Add(temp2[0]);

                sData.Add(0xAA);
                sData.Add(0xBB);
                sData.Add(0xCC);
                sData.Add(0xDD);
                //serialPort1.Write(sData.ToArray(), 0, sData.Count);
                Thread.Sleep(5);
                GetInffo = sData;
            }
        }
    }
    public enum opCode
    {
        Null,

        //P0 = 0,
        //P1 = 1,
        //P3 = 3,
        //P4 = 4,
        //P5 = 5,
        //P6 = 6,
        //P7 = 7,
        //P8 = 8,
        //P9 = 9,

        P10 = 0x10,
        P11 = 0x11,
        P12 = 0x12,
        P13 = 0x13,
        P14 = 0x14,
        P15 = 0x15,
        P16 = 0x16,
        P17 = 0x17,

        //P18 = 18,
        //P19 = 19,
        //P20 = 20,
        //P21 = 21,
        //P22 = 22,
        //P23 = 23,

        P72 = 0x72,
        P73 = 0x73,
        P74 = 0x74,
        P75 = 0x75,
        P76 = 0x76,
        P77 = 0x77,
        P78 = 0x78,
        P79 = 0x79,

        P8A = 0x8A,
        P8B = 0x8B,
        P8C = 0x8C,
        P8D = 0x8D,
        P8E = 0x8E,

        P149 = 149,
        P150 = 150,

        LowRefreshTime = 240,
        PAutoRefresh = 241,
    }

}