using System;
using System.IO;

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
        public UInt64 BTCS; //0x80
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

        //static void Main(String[] args)
        //{
        //    using (FileStream fs = new FileStream(@"/Users/im-aron/Documents/한컴GMD/han.dmg", FileMode.Open))
        //    {
        //        CSB.TotalSize = (UInt64)fs.Length;
        //        CSB.BlockSize = 4096;
        //        CSB csb = CSB.init_csb(fs, 0);

        //        init_vcsb(fs, 332);

        //        Console.WriteLine("FIN");


        //    }
        //}

        public static VCSB init_vcsb(FileStream fs, UInt64 block_num)
        {
            VCSB vcsb = new VCSB();
            UInt64 block_addr = Utility.get_address(block_num);
            int n;
            string hex;
            byte[] buf = new byte[128];

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x08", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.NodeID = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.CheckpointID = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x20", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.Volume_Magic = Utility.hex_to_charArray(hex);

            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.VolumeNumber = (UInt32)Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.CS = (UInt32)Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x58", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.BlockInVolume = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x80", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.BTCS = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.RootnodeNodeID = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.ExtentsBtree = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.SnapshotsList = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0xB0", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.NextCNID = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.FileNum = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.FolderNum = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x100", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.VolumeModifyTime = Utility.hex_to_dateTime(hex);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x130", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.VolumeCreateTime = Utility.hex_to_dateTime(hex);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x160", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.CheckpointCreateTime = Utility.hex_to_dateTime(hex);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x2c0", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 48);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            vcsb.VolumeName = Utility.hex_to_charArray(hex);





            //Console.WriteLine("VCSB_Address :{0}", block_addr);
            //Console.WriteLine("NodeID :{0}", vcsb.NodeID);
            //Console.WriteLine("CheckpointID :{0}", vcsb.CheckpointID);
            ////string magic = new string(vcsb.Volume_Magic);
            ////Console.WriteLine("Volume_Magic :{0}",magic);
            //Console.WriteLine("\nVolumeNumber :{0}", vcsb.VolumeNumber);
            //Console.WriteLine("CS :{0}", vcsb.CS);
            //Console.WriteLine("BlockInVolume :{0}", vcsb.BlockInVolume);
            //Console.WriteLine("\nBTCS :{0}", vcsb.BTCS);
            //Console.WriteLine("RootnodeNodeID :{0}", vcsb.RootnodeNodeID);
            //Console.WriteLine("ExtentsBtree :{0}", vcsb.ExtentsBtree);
            //Console.WriteLine("\nSnapshotsList :{0}", vcsb.SnapshotsList);
            //Console.WriteLine("NextCNID :{0}", vcsb.NextCNID);
            //Console.WriteLine("FileNum :{0}", vcsb.FileNum);
            //Console.WriteLine("FolderNum :{0}", vcsb.FolderNum);
            //Console.WriteLine("VolumeName :{0}", new string(vcsb.VolumeName) );
            Console.WriteLine("VolumeModifyTime : {0} ", vcsb.VolumeModifyTime);
            Console.WriteLine("VolumeCreateTime : {0} ", vcsb.VolumeCreateTime);
            Console.WriteLine("CheckpointCreateTime : {0} ", vcsb.CheckpointCreateTime);


            return vcsb;
        }





    }
}
