using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetExceptionSchedule : ASN1.IEncode, ASN1.IDecode
    {
        List<BacnetSpecialevent> entries;

        public BacnetExceptionSchedule()
        {
            entries = new List<BacnetSpecialevent>();
        }

        public void Encode(EncodeBuffer buffer)
        {
            foreach (BacnetSpecialevent entry in entries)
            {
                entry.Encode(buffer);
            }
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            int len = 0;
            int tlen = 0;

            while (offset + len < (count - 1))
            {
                BacnetSpecialevent entry = new BacnetSpecialevent();
                tlen = entry.Decode(buffer, offset + len, count);
                if (tlen <= 0)
                    return tlen;

                len += tlen;
                entries.Add(entry);
            }

            return len;
        }
    }
}
