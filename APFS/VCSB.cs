using System;

namespace APFS
{
    public class VCSB
    {
        public UInt64 NodeID; // 0x08
        public UInt64 CheckpointID; //0x10
        public char[] Volume_Magic; //0x20
        public UInt32 VolumeNumber; //0x24
        public UInt32 CS; //0x38

        public UInt64 BlockInVolume; //0x58 volume에 사용되는 블럭개수
        public UInt64 BTOM; //0x80
        public UInt64 RootnodeNodeID; //0x88
        public UInt64 ExtentsBtree; //0x90
        public UInt64 SnapshotsList; //0x98
        public UInt64 NextCNID; //0xb0

        public UInt64 FileNum; //0xb8 volume안의 파일개수
        public UInt64 FolderNum; //0xc0 volume안의 폴더개수
        public DateTime VolumeModifyTime; //0x100
        public DateTime VolumeCreateTime; //0x130
        public DateTime CheckpointCreateTime; //0x160

        public char[] VolumeName; //0x2c0


    }
}
