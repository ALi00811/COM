using ComProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using WpfApp1.Json;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        ClassComConnection com;
        public string SerialPort { get; set; }
        public int Value { get; set; }
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnAnalize_Click(object sender, RoutedEventArgs e)
        {
            // Set Defult Value
            if (tbSerialPort.Text == "") tbSerialPort.Text = "45812"; 

            if (!string.IsNullOrEmpty(tbValue.Text) && !string.IsNullOrEmpty(tbSerialPort.Text))
            {
                SerialPort = tbSerialPort.Text;
                Value = Convert.ToInt16(tbValue.Text);

                //Conection
                com = new ClassComConnection();
                com.SetSerialPort(SerialPort);
                com.SendData(opCode.P10, Value, 1);

                //Create Json File
                SerializeJson jsonfile = new SerializeJson(com, Value, opCode.P10.ToString());

                //Read Json File
                var jsonfile2 = new DeserializeJson();
                var result = jsonfile2.Deserialize();

                // Set Value as Json File
                lblHashCode.Content = result.HashCode;
                lblHeader.Content = result.Header;
                lblFrameCounter.Content = result.FrameCounter;
                lblOpCode.Content = result.OpCode;
                lblData.Content = result.Data;
                lblCrc.Content = result.Crc;
                lblPatch.Content = $"Patch Json File : {jsonfile2.Result}";
                
            }
        }
    }
}
