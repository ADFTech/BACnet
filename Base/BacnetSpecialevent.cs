using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetSpecialevent : ASN1.IEncode, ASN1.IDecode
    {
        public enum BacnetSchedulePeriods
        {
            CALENDAR_ENTRY,
            CALENDAR_REF,
        }

        public BacnetSchedulePeriods period_type;
        public object period_entry;
        public BacnetObjectId period_ref;
        public BacnetDailySchedule schedule;
        public uint priority;

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
                BACnetCalendarEntry.Encode(buffer, period_entry as ASN1.IEncode);
            }
            else if (period_type == BacnetSchedulePeriods.CALENDAR_REF)
            {
                ASN1.encode_context_object_id(buffer, 1, period_ref.type, period_ref.instance);
            }
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

            tlen = ASN1.decode_context_unsigned(buffer, offset + len, 3, out priority);
            if (tlen <= 0)
                return -1;

            len += tlen;

            return len;
        }
    }
}
