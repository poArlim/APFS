using System;
using System.IO;
namespace APFS
{
    public class Table
    {
        public uint check_point;
        public uint btree_level;
        public uint table_type;
        public uint record_num;
        public uint len_record_def;
        public uint len_key_section;
        public uint gap_key_data;


        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/seungbin/Downloads/han.dmg", FileMode.Open))
        //    {
        //        CSB.MSB_Address = 20480;
        //        CSB.BlockSize = 4096;
        //        //UInt64 block_num = 323;
        //        UInt64 block_num = 0;
        //        Table t = get_table_header(fs, block_num);
        //        //Console.WriteLine("table_type : {0}", t.table_type);
        //        //Console.WriteLine("len_record_def : {0}", t.len_record_def);
        //        //Console.WriteLine("len_key_section : {0}", t.len_key_section);
        //        //Console.WriteLine("gap_key_data : {0}", t.gap_key_data);

        //        UInt64 VRB_addr = get_block_address(fs, block_num, "0xA0");
        //        Console.WriteLine("VRB address : {0}", VRB_addr);

        //        UInt64 VB_addr = get_block_address(fs, VRB_addr, "0x30");
        //        Console.WriteLine("VB address : {0}", VB_addr);

        //        t = get_table_header(fs, VB_addr);
        //        Console.WriteLine("VB check_point : {0}", t.check_point);
        //        Console.WriteLine("VB table_type : {0}", t.table_type);
        //        Console.WriteLine("VB record_num : {0}", t.record_num);
        //        Console.WriteLine("VB len_record_def : {0}", t.len_record_def);
        //        Console.WriteLine("VB len_key_section : {0}", t.len_key_section);
        //        Console.WriteLine("VB gap_key_data : {0}", t.gap_key_data);


        //        VB.save_recordVB(fs, VB_addr, t.table_type, t.record_num, t);



        //    }

        //}


        public static UInt64 get_block_address(FileStream stream, UInt64 blocknum, string address)
        {
            UInt64 VRB_addr;
            int n;
            string hex;
            byte[] buf = new byte[64];
            UInt64 block_addr = Utility.get_address(blocknum);

            stream.Seek((Int64)block_addr + Convert.ToInt64(address, 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 4);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            VRB_addr = (UInt64)Utility.little_hex_to_uint64(hex, n);
            //VRB_addr = Utility.get_address(VRB_addr);

            return VRB_addr;
        }


        public static Table get_table_header(FileStream stream, UInt64 blocknum)
        {

            Table t = new Table();
            int n;
            string hex;
            byte[] buf = new byte[32];
            UInt64 block_addr = Utility.get_address(blocknum);

            stream.Seek((Int64)block_addr + Convert.ToInt64("0x10", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.check_point = (uint)Utility.little_hex_to_uint64(hex, n);

            stream.Seek((Int64)block_addr + Convert.ToInt64("0x18", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.btree_level = (uint)Utility.little_hex_to_uint64(hex, n);

            stream.Seek((Int64)block_addr + Convert.ToInt64("0x20", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.table_type = (uint)Utility.little_hex_to_uint64(hex, n);

            stream.Seek((Int64)block_addr + Convert.ToInt64("0x24", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.record_num = (uint)Utility.little_hex_to_uint64(hex, n);

            // 바꾼거 
            stream.Seek((Int64)block_addr + Convert.ToInt64("0x2A", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.len_record_def = (uint)Utility.little_hex_to_uint64(hex, n);

            //stream.Seek((Int64)block_addr + Convert.ToInt64("0x2C", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.len_key_section = (uint)Utility.little_hex_to_uint64(hex, n);

            //stream.Seek((Int64)block_addr + Convert.ToInt64("0x2E", 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            t.gap_key_data = (uint)Utility.little_hex_to_uint64(hex, n);

            return t;
        }


    }

    public class TableType1
    {
        public UInt16 KeyOffset;
        public UInt16 KeyLength;
        public UInt16 DataOffset;
        public UInt16 DataLength;

        //key section
        public char[] IndexKey;

        //data section
        public UInt64 NodeID;

    }

    public class TableType2
    {
        public UInt16 KeyOffset;
        public UInt16 KeyLength;
        public UInt16 DataOffset;
        public UInt16 DataLength;

        //Record Type에 따라 달라짐.
        public UInt64 ParentID_and_RecordType; // 0x00

    }




    public class TableType5
    {
        public UInt16 KeyOffset;
        public UInt16 DataOffset;

        //key section
        public UInt64 NodeID; //0x00 
        public UInt64 StructureID; //0x08  

        //data section
        public UInt64 BlockNum; //0x20  
    }

    public class TableType6
    {
        public UInt16 KeyOffset;
        public UInt16 DataOffset;

        //keysection
        public UInt64 NodeID; //0x00 
        public UInt64 StructureID; //0x08  

        //data section
        public UInt16 BlockSize; //0x04 
        public UInt64 BlockNum; //0x20
    }


    public class TableType7
    {
        public UInt16 KeyOffset;
        public UInt16 DataOffset;

        //keysection
        public UInt64 NodeID; //0x00 
        public UInt64 StructureID; //0x08  

        //data section
        public UInt16 BlockSize; //0x04 
        public UInt64 BlockNum; //0x20

        //footer
        public UInt16 TotalLeafNode; //0x18
        public UInt16 TotalIndexNode; //0x20
    }
}
