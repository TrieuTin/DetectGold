using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.IO.Ports.SerialPort serialPort;
        int sleep = 50;
        public MainWindow()
        {
            InitializeComponent();

            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
            if (serialPort != null)
                serialPort.Dispose();

            serialPort = Sys.PortData.NewPort.ConnectPort();
            serialPort.DataReceived += SerialPort_DataReceived;
           

        }

        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string txt = "";
            if (!Dispatcher.CheckAccess())  // Checks if this code is running on the UI thread
            {
                Dispatcher.Invoke(() => { SerialPort_DataReceived(sender, e); });  // Invokes the method on the UI thread
                return;
            }
            else
            {
                int dataLength = serialPort.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = serialPort.Read(data, 0, dataLength);

                if (nbrDataRead == 0)
                {
                    // txt = "0.00";
                    return;
                }

                string str = System.Text.Encoding.UTF8.GetString(data);
                txt += str;

             
                System.Threading.Thread.Sleep(sleep);

                string finaltxt = txt.Trim();
                string oneLine = "";

                for (int i = 0; i < finaltxt.Length; i++)
                {
                    char ch = Convert.ToChar(finaltxt.Substring(i, 1));
                    if (ch == 45 || ch == 46 || (ch >= 48 && ch <= 57))  // checks for '-', '.' and numbers
                    {
                        oneLine += ch;
                    }
                }

                oneLine = oneLine.Replace("..", "-");
                int count = 0;

                for (int i = oneLine.Length - 1; i > 0; i--)
                {
                    char ch = oneLine[i];
                    if (ch == 45) // Minus sign
                    {
                        count = i + 1;
                        break;
                    }
                }

                lbl_weight.Text = CheckInput(oneLine.Substring(count, oneLine.Length - count)) == "0" ?
                    "0.0" : CheckInput(oneLine.Substring(count, oneLine.Length - count));
            }

            string CheckInput(string text)
            {
                string pase = "";//text.Contains(".") ? text[^5..] : text[^3..];
                if (text.Contains("."))
                {
                    
                    pase = text.Substring(Math.Max(0, text.Length - 5));
                }
                else
                {                    
                    pase = text.Substring(Math.Max(0, text.Length - 3));
                }


                if (!float.TryParse(pase, out float fl))
                {
                    return "";
                }
              
                return fl.ToString();
            }
        }

        double _m_air = 0;
        double _m_water = 0;

        const float P_bronze= 8.96f;
        const float P_gold= 19.32f;
        void Calculator()
        {
            if (double.Parse(lbl_Air.Text) != 0 && double.Parse(lbl_Water.Text) != 0)
            {
                lbl_rateGold.Text = "0";

                lbl_rateBronze.Text = "0";

              

                double F = 0;
                double P_hk = 0;
                double x = 0;
                double y = 0;

                _m_air = double.Parse(lbl_Air.Text);

                _m_water = double.Parse(lbl_Water.Text);

                F = (_m_air - _m_water);
                P_hk = (_m_air / F);

                x = ((P_hk - P_bronze) / (P_gold - P_bronze)) * 100;

                y = 100 - x;


                lbl_rateBronze.Text = y.ToString();

                lbl_rateGold.Text = x.ToString();

                lbl_Water.Text = "0";

                lbl_Air.Text = "0";



            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lbl_Air.Text == "0") lbl_Air.Text = lbl_weight.Text;

            if (lbl_Water.Text == "0") lbl_Water.Text = lbl_weight.Text;

            Calculator();
        }
    }
}
