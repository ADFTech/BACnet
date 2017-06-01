﻿using System;
using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetTimevalue : ASN1.IEncode, ASN1.IDecode
    {
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public BacnetValue Val
        {
            get { return val; }
            set { val = value; }
        }

        public DateTime time;
        public BacnetValue val;

        public BacnetTimevalue()
        {
            time = new DateTime();
            val = new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, 0u);
        }

        public void Encode(EncodeBuffer buffer)
        {
            ASN1.encode_application_time(buffer, time);
            ASN1.bacapp_encode_application_data(buffer, val);
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            int len = 0;
            int tlen = 0;

            DateTime ttime;
            BacnetValue tval;

            tlen = ASN1.decode_application_time(buffer, offset + len, out ttime);
            if (tlen <= 0)
                return tlen;
            len += tlen;

            uint lenValueType;
            byte tagNumber;
            var tagLen = ASN1.decode_tag_number_and_value(buffer, offset + len, out tagNumber, out lenValueType);
            if (tagLen > 0)
            {
                len += tagLen;
                var decodeLen = ASN1.bacapp_decode_data(buffer, offset + len, (int)count, (BacnetApplicationTags)tagNumber,
                    lenValueType, out tval);
                if (decodeLen < 0) return decodeLen;
                len += decodeLen;

                time = ttime;
                val = tval;
            }

            return len;
        }
    }
}
