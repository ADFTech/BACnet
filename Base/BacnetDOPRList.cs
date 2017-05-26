using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.BACnet.Serialize;
using System.Linq;
using System.Text;

namespace System.IO.BACnet
{
    public class BacnetDOPRList : ASN1.IEncode, ASN1.IDecode
    {
        public ObservableCollection<BacnetDOPR> doprs { get; set; }

        public BacnetDOPRList()
        {
            doprs = new ObservableCollection<BacnetDOPR>();
        }

        public class BacnetDOPR : ASN1.IEncode, ASN1.IDecode
        {
            public BacnetObjectId obj_id { get; set; }
            public BacnetPropertyIds prop_id { get; set; }
            public uint array_index { get; set; }
            public BacnetObjectId dev_id { get; set; }

            public BacnetDOPR()
            {
                array_index = ASN1.BACNET_ARRAY_ALL;

                dev_id = new BacnetObjectId(BacnetObjectTypes.MAX_BACNET_OBJECT_TYPE, 0);
            }

            public void Encode(EncodeBuffer buffer)
            {
                ASN1.encode_context_object_id(buffer, 0, obj_id.type, obj_id.instance);
                ASN1.encode_context_enumerated(buffer, 1, (uint)prop_id);

                /* Array index is optional so check if needed before inserting */
                if (array_index != ASN1.BACNET_ARRAY_ALL)
                    ASN1.encode_context_unsigned(buffer, 2, array_index);

                /* Likewise, device id is optional so see if needed
                 * (set type to non device to omit */
                if (dev_id.type == BacnetObjectTypes.OBJECT_DEVICE)
                    ASN1.encode_context_object_id(buffer, 3, dev_id.type, dev_id.instance);
            }

            public int Decode(byte[] buffer, int offset, uint count)
            {
                int len = 0;
                int tlen = 0;

                BacnetObjectId tobj_id;
                ushort obj_type;
                tlen = ASN1.decode_context_object_id(buffer, offset + len, 0, out obj_type, out tobj_id.instance);
                if (tlen <= 0)
                    return tlen;

                tobj_id.type = (BacnetObjectTypes)obj_type;
                obj_id = tobj_id;
                len += tlen;

                uint tprop_id;
                tlen = ASN1.decode_context_unsigned(buffer, offset + len, 1, out tprop_id);
                if (tlen <= 0)
                    return tlen;

                prop_id = (BacnetPropertyIds)tprop_id;
                len += tlen;

                if (ASN1.decode_is_context_tag(buffer, offset + len, 2))
                {
                    // Optional array index
                    uint tarray_index;
                    tlen = ASN1.decode_context_unsigned(buffer, offset + len, 2, out tarray_index);
                    if (tlen <= 0)
                        return tlen;

                    array_index = tarray_index;
                    len += tlen;
                }

                if (offset + len < (count - 1) && ASN1.decode_is_context_tag(buffer, offset + len, 3))
                {
                    // Optional device id
                    tlen = ASN1.decode_context_object_id(buffer, offset + len, 3, out obj_type, out tobj_id.instance);
                    if (tlen <= 0)
                        return tlen;

                    tobj_id.type = (BacnetObjectTypes)obj_type;
                    dev_id = tobj_id;
                    len += tlen;
                }

                return len;
            }
        }

        public void Encode(EncodeBuffer buffer)
        {
            foreach (BacnetDOPR dopr in doprs)
            {
                dopr.Encode(buffer);
            }
        }

        public int Decode(byte[] buffer, int offset, uint count)
        {
            int len = 0;
            int tlen = 0;

            while (offset + len < (count - 1))
            {
                BacnetDOPR dopr = new BacnetDOPR();
                tlen = dopr.Decode(buffer, offset + len, count);
                if (tlen <= 0)
                    return tlen;

                len += tlen;
                doprs.Add(dopr);
            }

            return len;
        }
    }
}
