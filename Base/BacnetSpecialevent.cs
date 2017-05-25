using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetSpecialevent : ASN1.IEncode, ASN1.IDecode
    {
        public enum BacnetSchedulePeriods : byte
        {
            CALENDAR_ENTRY,
            CALENDAR_REF,
        }

        public BacnetSchedulePeriods period_type;
        public object period_entry;
        public BacnetObjectId period_ref;
        public BacnetDailySchedule schedule { get; set; }
        public uint priority { get; set; }

        public BacnetSpecialevent()
        {
            period_type = BacnetSchedulePeriods.CALENDAR_ENTRY;
            period_entry = new BacnetDate();
            period_ref = new BacnetObjectId(BacnetObjectTypes.OBJECT_CALENDAR, 1);
            schedule = new BacnetDailySchedule();
        }

        public void Encode(EncodeBuffer buffer)
        {
            if (period_type == BacnetSchedulePeriods.CALENDAR_ENTRY)
            {
                ASN1.encode_opening_tag(buffer, 0);
                BACnetCalendarEntry.Encode(buffer, period_entry as ASN1.IEncode);
                ASN1.encode_closing_tag(buffer, 0);
            }
            else if (period_type == BacnetSchedulePeriods.CALENDAR_REF)
            {
                ASN1.encode_context_object_id(buffer, (byte)BacnetSchedulePeriods.CALENDAR_REF, period_ref.type, period_ref.instance);
            }

            ASN1.encode_opening_tag(buffer, 2);
            schedule.Encode(buffer);
            ASN1.encode_closing_tag(buffer, 2);

            ASN1.encode_context_unsigned(buffer, 3, priority);
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            int len = 0;
            int tlen = 0;

            if (ASN1.decode_is_opening_tag_number(buffer, offset + len, 0))
            {
                len++;
                ASN1.IDecode entry;
                tlen = BACnetCalendarEntry.Decode(buffer, offset + len, count, out entry);

                if (tlen <= 0)
                    return tlen;

                period_entry = entry;
                period_type = BacnetSchedulePeriods.CALENDAR_ENTRY;
                len += tlen;
            }
            else if (ASN1.decode_is_context_tag(buffer, offset + len, 1))
            {
                ushort type;
                tlen = ASN1.decode_context_object_id(buffer, offset + len, 1, out type, out period_ref.instance);
                period_ref.type = (BacnetObjectTypes)type;
                if (tlen <= 0)
                {
                    return tlen;
                }
                period_type = BacnetSchedulePeriods.CALENDAR_REF;
                len += tlen;
            }

            tlen = schedule.DecodeContext(buffer, offset + len, count, 2);
            if (tlen <= 0)
                return -1;

            len += tlen;

            uint tpriority;
            tlen = ASN1.decode_context_unsigned(buffer, offset + len, 3, out tpriority);
            if (tlen <= 0)
                return -1;

            priority = tpriority;

            len += tlen;

            return len;
        }
    }
}
