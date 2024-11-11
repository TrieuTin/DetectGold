using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys
{
    class PortData
    {

        private static PortData _pd;

        SerialPort port;


        public static List<string> PortsName
        {

            get
            {
                return GetPort();
            }
        }


        public static PortData NewPort { get => _pd == null ? _pd = new PortData() : _pd; }

        PortData()
        {

        }
        void DisconnectPort()
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
        }
        public SerialPort ConnectPort(string portName = "COM1", int baudrate = 9600, Parity parity = Parity.None, int dataBit = 8, StopBits stopBit = StopBits.One)
        {
            if (port != null && !port.IsOpen)
            {
                port = new SerialPort(portName, baudrate, parity, dataBit, stopBit);

                port.Open();

            }
            else
            {
                DisconnectPort();

                port = new SerialPort(portName, baudrate, parity, dataBit, stopBit);

                port.Open();
            }
            return port;
        }
        public static List<string> GetPort()
        {
            var portList = new List<string>();

            // Retrieve the list of all COM ports on your Computer
            string[] ports = SerialPort.GetPortNames();
            foreach (string _port in ports)
            {
                portList.Add(_port);

            }
            return portList;
        }
    }
    
}
