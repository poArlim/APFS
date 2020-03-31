using System;

namespace APFS
{
    public class FileFolderRecord // 0x30
    {
        //key section

        //data section
        public UInt64 ParentID; //0x00
        public UInt64 NodeID; //0x08
        public DateTime CreateTime; //0x10
        public DateTime ModifyTime; //0x18
        public DateTime InodeModifyTime; //0x20
        public DateTime AccessedTime; //0x28
        public UInt64 FileFolderNum; //0x38
        public UInt64 HardlinkNum; //0x40
        public UInt32 OwnerID; //0x48
        public UInt32 GroupID; //0x4c
        public UInt64 Flag; //0x50
        public UInt16 LengthMethod; //0x5c
        public UInt16 NameLengross; //0x5e
        public UInt16 DataType; //0x60
        public UInt16 FileNameLength; //0x62
        public char[] FileName; //0x68
        public UInt64 ContentLenLog; //file바로 뒤
        public UInt64 ContentLenGross; //위에 이어서
    }

    public class NameAttribute // 0x40
    {
        //key section

        //data section
    }

    public class ExtentStatus // 0x60
    {
        //key section

        //data section
        public UInt32 ExtentExist; //0x00
    }

    public class ExtentRecord // 0x80
    {
        //key section

        //data section
        public UInt64 ExtentLength; //0x00
        public UInt64 ExtentStartBlockNum; //0x08
    }

    public class KeyRecord // 90
    {
        //key section
        public Byte KeyLength; //0x08
        public char[] Key; //0x0c

        //data section
        public UInt64 CNID; //0x00
        public DateTime AddedDate; //0x08

    }
}
