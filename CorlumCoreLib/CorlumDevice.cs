using System;
using System.Collections.Generic;
using System.Text;

namespace Corlum.Core
{
    public class CorlumDevice
    {
        protected CorlumCommsManager _commsMngr = null;
        public CorlumDevice(CorlumCommsManager commsMngr)
        {
            _commsMngr = commsMngr;
        }

        public UInt32 NodeAddress { get; set; } = default;

        public void SetPWMOutput(int dutyCycle)
        {
            _commsMngr.SendPWMCommand(NodeAddress, dutyCycle);
        }

    }
}
