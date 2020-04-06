﻿using System;
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

        public static TableType[] save_record(FileStream stream, UInt64 block_num, Table header)
        {
            int n;
            string hex;
            byte[] buf = new byte[4096];
            UInt64 block_addr = Utility.get_address(block_num);
            Int64 footer_length = 0;
            if (header.table_type % 2 == 1)
            {
                //footer
                Footer footer = new Footer();
                stream.Seek((Int64)block_addr + 4096 - 16, SeekOrigin.Begin);
                n = stream.Read(buf, 0, 8);
                hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                footer.TotalLeafNode = (UInt16)Utility.little_hex_to_uint64(hex, n);
                n = stream.Read(buf, 0, 8);
                hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                footer.TotalIndexNode = (UInt16)Utility.little_hex_to_uint64(hex, n);
                footer_length = 40;
            }

            TableType[] records = new TableType[header.record_num];

            switch (header.table_type)
            {
                case 1:
                    for (ulong i = 0; i < header.record_num; i++)
                    {
                        records[i] = new TableType();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 8, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records[i].KeyLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        hex = hex.Substring(0, records[i].KeyLength);
                        records[i].IndexKey = hex;

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records[i].DataLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].NodeID = (UInt16)Utility.little_hex_to_uint64(hex, n);
                    }
                    break;

                case 2:
                case 3:
                    for (ulong i = 0; i < header.record_num; i++)
                    {
                        records[i] = new TableType();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 8, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records[i].KeyLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeySection = hex;
                        records[i].KeySection = records[i].KeySection.Substring(0, records[i].KeyLength);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records[i].DataLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataSection = hex;
                        records[i].DataSection = records[i].DataSection.Substring(0, records[i].DataLength);

                        Console.WriteLine("{0}", i + 1);
                        Console.WriteLine("record key offset : {0}", records[i].KeyOffset);
                        Console.WriteLine("record key length : {0}", records[i].KeyLength);
                        Console.WriteLine("record data offset : {0}", records[i].DataOffset);
                        Console.WriteLine("record data length : {0}", records[i].DataLength);
                        Console.WriteLine("record key section: {0}", records[i].KeySection);
                        Console.WriteLine("record data section: {0}", records[i].DataSection);
                    }
                    break;
                case 4:
                case 5:
                    for (ulong i = 0; i < header.record_num; i++)
                    {
                        records[i] = new TableType();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].NodeID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].StructureID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].BlockNum = (UInt64)Utility.little_hex_to_uint64(hex, n);
                    }
                    break;


                case 6:
                case 7:
                    for (ulong i = 0; i < header.record_num; i++)
                    {
                        records[i] = new TableType();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].NodeID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].StructureID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records[i].DataOffset, SeekOrigin.Begin);
                        stream.Read(buf, 0, 4);
                        n = stream.Read(buf, 0, 4);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].BlockSize = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].BlockNum = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //if (i == 0) //VB확인을 위한 임시적인 것. 함수 밖으로 빼도 됨
                        //{
                        //    VCSB_BlockNum = records_67[i].BlockNum;
                        //}

                        Console.WriteLine("{0}", i + 1);
                        Console.WriteLine("record key offset : {0}", records[i].KeyOffset);
                        Console.WriteLine("record data offset : {0}", records[i].DataOffset);
                        Console.WriteLine("record node id : {0}", records[i].NodeID);
                        Console.WriteLine("record structure id : {0}", records[i].StructureID);
                        Console.WriteLine("record block size: {0}", records[i].BlockSize);
                        Console.WriteLine("record block num: {0}", records[i].BlockNum);
                    }
                    break;
            }
            return records;
        }

        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/seungbin/Downloads/han.dmg", FileMode.Open))
        //    {
        //        CSB.MSB_Address = 20480;
        //        CSB.BlockSize = 4096;
        //        //UInt64 block_num = 0;
        //        UInt64 VCSB_addr = 332;
        //        Table t;

        //        //volume structure
        //        //UInt64 VRB_addr = get_block_address(fs, block_num, "0xA0");
        //        //Console.WriteLine("VRB address : {0}", VRB_addr);

        //        //UInt64 VB_addr = get_block_address(fs, VRB_addr, "0x30");
        //        //Console.WriteLine("VB address : {0}", VB_addr);

        //        //t = get_table_header(fs, VB_addr);
        //        //Console.WriteLine("VB check_point : {0}", t.check_point);
        //        //Console.WriteLine("VB table_type : {0}", t.table_type);
        //        //Console.WriteLine("VB record_num : {0}", t.record_num);
        //        //Console.WriteLine("VB len_record_def : {0}", t.len_record_def);
        //        //Console.WriteLine("VB len_key_section : {0}", t.len_key_section);
        //        //Console.WriteLine("VB gap_key_data : {0}", t.gap_key_data);


        //        //VB.save_record(fs, VB_addr, t.table_type, t.record_num, t);

        //        //BTCS
        //        UInt64 BTCS_addr = get_block_address(fs, VCSB_addr, "0x80");
        //        Console.WriteLine("BTCS address : {0}", BTCS_addr);

        //        UInt64 BTOM_addr = get_block_address(fs, BTCS_addr, "0x30");
        //        Console.WriteLine("BTOM address : {0}", BTOM_addr);

        //        t = get_table_header(fs, BTOM_addr);
        //        Console.WriteLine("BTOM check_point : {0}", t.check_point);
        //        Console.WriteLine("BTOM btree num : {0}", t.btree_level);
        //        Console.WriteLine("BTOM table_type : {0}", t.table_type);
        //        Console.WriteLine("BTOM record_num : {0}", t.record_num);
        //        Console.WriteLine("BTOM len_record_def : {0}", t.len_record_def);
        //        Console.WriteLine("BTOM len_key_section : {0}", t.len_key_section);
        //        Console.WriteLine("BTOM gap_key_data : {0}", t.gap_key_data);

        //        t = get_table_header(fs, 331);

        //        Table.save_record(fs, 331, t);
        //    }

        //}


        public static UInt64 get_block_address(FileStream stream, UInt64 blocknum, string address)
        {
            UInt64 sought_block;
            int n;
            string hex;
            byte[] buf = new byte[64];
            UInt64 block_addr = Utility.get_address(blocknum);

            stream.Seek((Int64)block_addr + Convert.ToInt64(address, 16), SeekOrigin.Begin);
            n = stream.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            sought_block = (UInt64)Utility.little_hex_to_uint64(hex, n);
            //VRB_addr = Utility.get_address(VRB_addr);

            return sought_block;
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

    public class TableType
    {
        public UInt16 KeyOffset;
        public UInt16 KeyLength;
        public UInt16 DataOffset;
        public UInt16 DataLength;

        //1
        //key section
        public string IndexKey;
        //data section
        public UInt64 Node_ID;

        //23
        public string KeySection;
        public string DataSection;

        //45
        //key section
        public UInt64 NodeID; //0x00 
        public UInt64 StructureID; //0x08

        //DATA section + 67
        public UInt64 BlockNum; //0x20  
        public UInt16 BlockSize; //0x04 
    }

    public class Footer
    {
        public UInt64 TotalLeafNode; //0x18
        public UInt64 TotalIndexNode; //0x20
    }

}
