using System;

namespace APFS
{
    public class CSB
    {
        public static UInt64 MSB_Address;
        public UInt64 BlockCheksum;
        public UInt64 BlockID;
        public UInt64 CSB_Checkpoint;
        public char[] CSB_Magic;
        public static UInt32 BlockSize;
        public static UInt64 TotalSize;
        public static UInt64 TotalBlocks;

        public UInt64 NextCSB_ID;
        public UInt32 BaseBlock;
        public UInt32 PreviousCSBD;
        public UInt32 OriginalCSBD;
        public UInt32 OldestCSBD;

        public UInt64 VolumesIndexblock;
        public UInt32 VolumesMaxNumber;
        public UInt64 VolumeID_List;
    }
}