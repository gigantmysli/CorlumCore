using System;
using System.Threading.Tasks;
using Corlum.Core;
using System.Collections.Concurrent;


namespace CorlumCore
{
    class Program
    {

        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            CorlumCommsManager.ControlStatusMessage tcm = new CorlumCommsManager.ControlStatusMessage();
            tcm.NodeAddress = CorlumCommsManager.BroadcastAdress;
            tcm.CommandUID = 0xfa;
            tcm.CommandArgument = CorlumCommsManager.CommandType.SetPWM;
            tcm.Data1 = 100;
            tcm.Data2 = new byte[]{ 0 };
            Console.WriteLine(BitConverter.ToString(tcm.GetMessageData()));
            CorlumPort _cp = new CorlumPort();
            _cp.SerialPortSetup("COM8");                   
            CorlumCommsManager cms = new CorlumCommsManager(_cp);
            cms.BroadcastPWMCommand(95);
            while (Console.ReadKey(true).Key != ConsoleKey.Q) ;
            Console.WriteLine("Requesting serial port stop");
            _cp.StopSerialPortListening();
        }
    }
}
