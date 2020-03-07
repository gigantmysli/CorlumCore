using System;
using System.Collections.Generic;
using System.Text;

namespace Corlum.Core
{
    public class CorlumCommsManager
    {
        public const UInt32 BroadcastAdress = 0xffFFffFF;
        public class ControlStatusMessage
        {
            public byte MessageLength { get; private set; }
            public UInt32 NodeAddress { get; set; } = default;
            public byte[] rawNodeAddress
            {
                get
                {
                    return BitConverter.GetBytes(NodeAddress);
                }
            }
            public byte CommandUID { get; set; }
            public byte PacketType { get; private set; } = 0x03;
            public CommandType CommandArgument { get; set; }
            public byte Data1 = default;
            public byte[] Data2;
            public byte[] GetMessageData()
            {
                List<byte> tmsg = new List<byte>();
                tmsg.Add(0x00);
                tmsg.AddRange(rawNodeAddress);
                tmsg.Add(CommandUID);
                tmsg.Add(PacketType);
                tmsg.Add((byte)CommandArgument);
                tmsg.Add(Data1);
                tmsg.AddRange(Data2);
                tmsg[0] = (byte)tmsg.Count;
                return tmsg.ToArray();
            }
        }
        public enum CommandType : byte
        {
            SetOutputs = 0x01,
            SetPWM = 0x02,
            SetGatewayInConfigMode = 0x05,
            ToggleOutputs = 0x08,
            GetNID = 0x10,
            GetStatus = 0x11,
            GetDIDStatus = 0x12,
            GetConfigurationMemory = 0x13,
            GetCalibrationMemory = 0x14,
            ForceRouterReset = 0x15,
            GetPacketPath = 0x16
        }

        protected CorlumPort _cp;
        public CorlumCommsManager(CorlumPort cmPort)
        {
            _cp = cmPort;

        }

        public void BroadcastPWMCommand(int dutyCycle)
        {
            SendPWMCommand(BroadcastAdress, dutyCycle);
        }

        public void SendPWMCommand(UInt32 nodeAddress, int dutyCycle /* 0-100*/)
        {
            CorlumCommsManager.ControlStatusMessage tcm = new CorlumCommsManager.ControlStatusMessage();
            tcm.NodeAddress = nodeAddress;
            tcm.CommandUID = 0xfa;
            tcm.CommandArgument = CorlumCommsManager.CommandType.SetPWM;
            tcm.Data1 = (byte)dutyCycle;
            tcm.Data2 = new byte[] { 0 };
            _cp.SendData(tcm.GetMessageData());
        }
    }
}
