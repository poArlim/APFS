using System;

namespace APFS
{
    public class CSBD
    {
        public UInt16 TableType; //0x20
        public UInt16 RecordNum; //0x24
    }
    public class CSBD_recode
    {
        public UInt16 RecordID; //0x00
        public UInt32 RecordSize; //0x02
        public UInt64 BlockSize; //0x08
        public UInt64 ObjectID; //0x18
        public UInt64 BlockNum; //0x20
    }

    //맨처음이 bitmap area의 location(BMD)

}
