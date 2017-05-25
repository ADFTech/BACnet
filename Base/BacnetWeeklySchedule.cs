using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetWeeklySchedule : ASN1.IEncode, ASN1.IDecode
    {
        public BacnetDailySchedule[] schedules { get; }

        public BacnetWeeklySchedule()
        {
            schedules = new BacnetDailySchedule[7];

            for (int i = 0; i < 7; i++)
            {
                schedules[i] = new BacnetDailySchedule();
            }
        }

        public void Encode(EncodeBuffer buffer)
        {
            for (int i = 0; i < 7; i++)
            {
                ASN1.encode_opening_tag(buffer, 0);
                schedules[i].Encode(buffer);
                ASN1.encode_closing_tag(buffer, 0);
            }
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            int len = 0;
            int tlen = 0;

            for (int i = 0; i < 7; i++)
            {
                tlen = schedules[i].Decode(buffer, offset + len, count);
                if (tlen <= 0)
                    return -1;

                len += tlen;
            }

            return len;
        }
    }
}
