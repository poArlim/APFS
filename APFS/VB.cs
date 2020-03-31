using System;

namespace APFS
{
    public class VRB
    {
        public UInt64 VB; //0x30
    }

    public class VB
    {
        public UInt16 TableType; //0x20
        public UInt16 RecordNum; //0x24
        public UInt16 tableIndexSize; //0x2a
        public UInt16 tableKeyAreaSize; //0x2c
        public UInt16 tableFreeSpaceSize; //0x2e

    }
}
