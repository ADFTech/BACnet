using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    class BacnetTime : ASN1.IEncode, ASN1.IDecode
    {
        public byte hour;
        public byte minutes;
        public byte seconds;
        public byte hundredths;

        public void Encode(EncodeBuffer buffer)
        {
            buffer.Add(hour);
            buffer.Add(minutes);
            buffer.Add(seconds);
            buffer.Add(hundredths);
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            hour = buffer[offset];
            minutes = buffer[offset + 1];
            seconds = buffer[offset + 2];
            hundredths = buffer[offset + 3];
            return 4;
        }
    }
}
