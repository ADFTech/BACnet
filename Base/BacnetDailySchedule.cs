using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetDailySchedule : ASN1.IEncode, ASN1.IDecode
    {
        public ObservableCollection<BacnetTimevalue> entries { get; }

        public BacnetDailySchedule()
        {
            entries = new ObservableCollection<BacnetTimevalue>();
        }

        public void Encode(EncodeBuffer buffer)
        {
            foreach (BacnetTimevalue tv in entries)
            {
                tv.Encode(buffer);
            }
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            return DecodeContext(buffer, offset, count, 0);
        }

        public int DecodeContext(byte[] buffer, int offset, uint count, byte tag)
        {
            int len = 0;
            int tlen = 0;
            byte tag_number;

            tlen = ASN1.decode_tag_number(buffer, offset + len, out tag_number);
            if (tag_number != tag)
            {
                return -1;
            }
            len += tlen;
            while (!ASN1.IS_CLOSING_TAG(buffer[offset + len]))
            {
                BacnetTimevalue tv = new BacnetTimevalue();
                tlen = tv.Decode(buffer, offset + len, count);
                if (tlen <= 0)
                {
                    return tlen;
                }

                len += tlen;
                entries.Add(tv);
            }
            len++;

            return len;
        }
    }
}
