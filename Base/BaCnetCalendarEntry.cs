using System.Collections.Generic;
using System.IO.BACnet.Serialize;

namespace System.IO.BACnet
{
    public struct BACnetCalendarEntry : ASN1.IEncode, ASN1.IDecode
    {
        public List<object> Entries; // BacnetDate or BacnetDateRange or BacnetweekNDay

        public void Encode(EncodeBuffer buffer)
        {
            if (Entries == null)
                return;

            foreach (ASN1.IEncode entry in Entries)
            {
                Encode(buffer, entry);
            }
        }

        static public void Encode(EncodeBuffer buffer, ASN1.IEncode entry)
        {
            if (entry is BacnetDate)
            {
                ASN1.encode_tag(buffer, 0, true, 4);
                entry.Encode(buffer);
            }

            if (entry is BacnetDateRange)
            {
                ASN1.encode_opening_tag(buffer, 1);
                entry.Encode(buffer);
                ASN1.encode_closing_tag(buffer, 1);
            }

            if (entry is BacnetweekNDay)
            {
                ASN1.encode_tag(buffer, 2, true, 3);
                entry.Encode(buffer);
            }
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            var len = 0;

            Entries = new List<object>();

            while (true)
            {
                ASN1.IDecode entry;
                len += Decode(buffer, offset + len, count, out entry);
                if (entry == null)
                    return len;

                Entries.Add(entry);
            }
        }

        static public int Decode(byte[] buffer, int offset, uint count, out ASN1.IDecode entry)
        {
            int len = 0;
            byte tagNumber;
            len += ASN1.decode_tag_number(buffer, offset + len, out tagNumber);

            switch (tagNumber)
            {
                case 0:
                    var bdt = new BacnetDate();
                    len += bdt.Decode(buffer, offset + len, count);
                    entry = bdt;
                    break;
                case 1:
                    var bdr = new BacnetDateRange();
                    len += bdr.Decode(buffer, offset + len, count);
                    entry = bdr;
                    len++; // closing tag
                    break;
                case 2:
                    var bwd = new BacnetweekNDay();
                    len += bwd.Decode(buffer, offset + len, count);
                    entry = bwd;
                    break;
                default:
                    entry = null;
                    return len - 1; // closing Tag
            }

            return len;
        }
    }
}