using System;
using System.IO;
using System.Collections.Generic;
namespace APFS
{

    public class CSB
    {

        public static UInt64 MSB_Address;
        public static UInt64 TotalSize;
        public UInt64 CSB_Address;
        public UInt64 BlockChecksum;
        public UInt64 BlockID;
        public UInt64 CSB_Checkpoint;
        public char[] CSB_Magic;
        public static UInt32 BlockSize;
        public UInt64 TotalBlocks;

        public UInt64 NextCSB_ID;
        public UInt32 BaseBlock;
        public UInt32 NextCSBD;
        public UInt32 OriginalCSBD;
        public UInt32 OldestCSBD;

        public UInt64 VolumesIndexblock;
        public UInt32 VolumesMaxNumber;
        public UInt64 VolumeID_List;

        //public static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/newhan.dmg", FileMode.Open))
        //    {
        //        List<CSB> csb_list = get_csb_list(fs);
        //        foreach (CSB c in csb_list)
        //        {
        //            Console.WriteLine("Chckpoint : {0}", c.CSB_Checkpoint);
        //        }
        //        Console.WriteLine("Fin");
        //    }
        //}

        public static List<CSB> get_csb_list(FileStream fs)
        {
            List<CSB> csb_list = new List<CSB>(); 
            CSB.TotalSize = (UInt64)fs.Length;
            CSB.BlockSize = 4096;
            CSB msb = CSB.init_csb(fs, 0);
            //Console.WriteLine("msb - checkpoint : {0}", msb.CSB_Checkpoint);
            //Console.WriteLine("msb - OldestCSBD : {0}", msb.OldestCSBD);
            //Console.WriteLine("msb - NextCSBD : {0}", msb.NextCSBD);
            UInt64 next_csb_addr = Utility.get_address(msb.OldestCSBD) + CSB.BlockSize;
            while (true) {
                
                CSB csb = init_csb(fs, next_csb_addr);
                //Console.WriteLine("=======csb Address : {0}", csb.CSB_Address);
                //Console.WriteLine("checkpoint : {0}", csb.CSB_Checkpoint);
                //Console.WriteLine(" OldestCSBD : {0}", csb.OldestCSBD);
                //Console.WriteLine(" NextCSBD : {0}", csb.NextCSBD);
                csb_list.Add(csb);
                next_csb_addr = Utility.get_address(csb.NextCSBD) + CSB.BlockSize;
                if (csb.NextCSBD == msb.OriginalCSBD) break; 
            }
            csb_list.Add(msb);
            return csb_list;
        }

        public static CSB init_csb(FileStream fs, UInt64 start_addr)
        {
            
            CSB csb = new CSB();
            UInt64 block_addr;
            int n;
            string hex;
            byte[] buf = new byte[64];
            csb.CSB_Address = Utility.get_string_address(fs, start_addr, "NXSB") - 32;
            block_addr = csb.CSB_Address;
            if (start_addr == 0)
            {
                CSB.MSB_Address = csb.CSB_Address;
            }


            fs.Seek((Int64)block_addr + Convert.ToInt64("0x00", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.BlockChecksum = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
   
            csb.BlockID = Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.CSB_Checkpoint = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x20", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.CSB_Magic = Utility.hex_to_charArray(hex);

            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            CSB.BlockSize = (UInt32)Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.TotalBlocks = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x60", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.NextCSB_ID = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x70", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.BaseBlock = (UInt32)Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x80", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.NextCSBD = (UInt32)Utility.little_hex_to_uint64(hex, n) + csb.BaseBlock;

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x88", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.OriginalCSBD = (UInt32)Utility.little_hex_to_uint64(hex, n) + csb.BaseBlock;

            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.OldestCSBD = (UInt32)Utility.little_hex_to_uint64(hex, n) - 1; 

            fs.Seek((Int64)block_addr + Convert.ToInt64("0xA0", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.VolumesIndexblock = Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)block_addr + Convert.ToInt64("0xB4", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.VolumesMaxNumber = (UInt32)Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csb.VolumeID_List = Utility.little_hex_to_uint64(hex, n);



            //Console.WriteLine("CSB_Address :{0}", csb.CSB_Address);
            //Console.WriteLine("BlockChecksum :{0}", csb.BlockChecksum);
            //Console.WriteLine("BlockID :{0}", csb.BlockID);
            //Console.WriteLine("CSB_Checkpoint :{0}", csb.CSB_Checkpoint);
            //Console.WriteLine("BlockSize :{0} ", CSB.BlockSize);
            ////string s = new string(csb.CSB_Magic);
            ////Console.WriteLine("-->MAGIC : {0}", s);

            //Console.WriteLine("TotalBlocks :{0}", csb.TotalBlocks);
            //Console.WriteLine("NextCSB_ID :{0}", csb.NextCSB_ID);
            //Console.WriteLine("BaseBlock :{0}", csb.BaseBlock);
            //Console.WriteLine("NextCSBD :{0}", csb.NextCSBD);
            //Console.WriteLine("OriginalCSBD :{0} ", csb.OriginalCSBD);
            //Console.WriteLine("OldestCSBD :{0} ", csb.OldestCSBD);

            //Console.WriteLine("VolumesIndexblock :{0}", csb.VolumesIndexblock);
            //Console.WriteLine("VolumesMaxNumber :{0} ", csb.VolumesMaxNumber);
            //Console.WriteLine("VolumeID_List :{0} ", csb.VolumeID_List);




            return csb;

        }
    }
}