using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Corlum.Core
{
    public class CorlumPort
    {
        private SerialPort _sp = new SerialPort();
        private bool _listeningActive = false;
        private int _rxBytesCounter = 0;
        private int _rxEventFiresCounter = 0;
        public CorlumPort()
        {
            OnNewFrameReceived = OnNewFrameReceivedMethod;
        }

        public Action<byte[]> OnNewFrameReceived;
        List<byte> _rxBuffer = new List<byte>();
        private void AssembleFrame(byte[] data)
        {
            _rxBuffer.AddRange(data);
            if (_rxBytesCounter >= 35)
            {
                byte[] td = _rxBuffer.GetRange(0, 35).ToArray();
                _rxBuffer.RemoveRange(0, 35);
                _rxBytesCounter -= 35;
                Console.WriteLine("Remaining data = {0}", _rxBytesCounter);
                if (OnNewFrameReceived != null)
                {
                    OnNewFrameReceived(td);
                }
            }
        }

        protected void OnNewFrameReceivedMethod(byte[] data)
        {
            Console.WriteLine("---> {0}", BitConverter.ToString(data));
        }

        public void SerialPortSetup(string portName)
        {
            _sp.PortName = portName;
            _sp.BaudRate = 19200;
            _sp.StopBits = StopBits.One;
            _sp.DataBits = 8;
            _sp.DataReceived += DataReceivedHandler;
            _sp.Open();
            if (!_sp.IsOpen)
            {
                throw new Exception("Serial port setup failed");
            }
        }
        public void SendData(byte[] data)
        {
            if (_sp.IsOpen)
            {
                _sp.Write(data, 0, data.Length);
            }
            else
            {
                throw new Exception("Data transmission failed - port closed");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            _rxBytesCounter += sp.BytesToRead;
            _rxEventFiresCounter++;
            byte[] td = new byte[sp.BytesToRead];
            ((SerialPort)sender).Read(td, 0, sp.BytesToRead);
            AssembleFrame(td);
            Console.WriteLine(BitConverter.ToString(td));
            Console.WriteLine("Fires: {0} Frames count:{1}", _rxEventFiresCounter, (float)(_rxBytesCounter / 35) + 0.1f);
        }

        public void StopSerialPortListening()
        {
            _sp.Close();
        }
    }
}
