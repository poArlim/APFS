using System;
using System.IO;

namespace APFS
{

    public struct VB
    {
        public static UInt64 VCSB_BlockNum; //0x20

        public static void save_record(FileStream stream, UInt64 block_num, UInt64 Table_type, UInt64 record_num, Table header)
        {
            int n;
            string hex;
            byte[] buf = new byte[4096];
            UInt64 block_addr = Utility.get_address(block_num);
            Int64 footer_length = 0; 
            if (Table_type % 2 == 1)
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

            switch (Table_type)
            {
                case 1:
                    TableType1[] records1 = new TableType1[record_num];
                    for (ulong i = 0; i < record_num; i++)
                    {
                        records1[i] = new TableType1();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records1[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records1[i].KeyLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records1[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records1[i].DataLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records1[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records1[i].KeyLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        hex = hex.Substring(0, records1[i].KeyLength);
                        records1[i].IndexKey = hex;

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - 40 - (Int64)records1[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records1[i].DataLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records1[i].NodeID = (UInt16)Utility.little_hex_to_uint64(hex, n);


                        Console.WriteLine("{0}", i + 1);
                        Console.WriteLine("record key offset : {0}", records1[i].KeyOffset);
                    }
                    break;

                case 2:
                case 3:

                    TableType_23[] records_23 = new TableType_23[record_num];
                    for (ulong i = 0; i < record_num; i++)
                    {
                        records_23[i] = new TableType_23();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].KeyLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].DataLength = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records_23[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records_23[i].KeyLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].KeySection = hex;
                        records_23[i].KeySection = records_23[i].KeySection.Substring(0, records_23[i].KeyLength);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records_23[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, (int)(records_23[i].DataLength));
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_23[i].DataSection = hex;
                        records_23[i].DataSection = records_23[i].DataSection.Substring(0, records_23[i].DataLength);

                        Console.WriteLine("{0}", i + 1);
                        Console.WriteLine("record key offset : {0}", records_23[i].KeyOffset);
                        Console.WriteLine("record key length : {0}", records_23[i].KeyLength);
                        Console.WriteLine("record data offset : {0}", records_23[i].DataOffset);
                        Console.WriteLine("record data length : {0}", records_23[i].DataLength);
                        Console.WriteLine("record key section: {0}", records_23[i].KeySection);
                        Console.WriteLine("record data section: {0}", records_23[i].DataSection);
                    }
                    break;
                case 4:
                case 5:

                    TableType_45[] records_45 = new TableType_45[record_num];
                    for (ulong i = 0; i < record_num; i++)
                    {
                        records_45[i] = new TableType_45();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_45[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_45[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records_45[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_45[i].NodeID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_45[i].StructureID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records_45[i].DataOffset, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_45[i].BlockNum = (UInt64)Utility.little_hex_to_uint64(hex, n);
                    }
                    break;


                case 6:
                case 7:

                    TableType_67[] records_67 = new TableType_67[record_num];
                    for (ulong i = 0; i < record_num; i++)
                    {
                        records_67[i] = new TableType_67();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].DataOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //key section
                        stream.Seek((Int64)block_addr + (Int64)header.len_record_def + (Int64)records_67[i].KeyOffset + Convert.ToInt64("0x38", 16), SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].NodeID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].StructureID = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //data section
                        stream.Seek((Int64)block_addr + 4096 - footer_length - (Int64)records_67[i].DataOffset, SeekOrigin.Begin);
                        stream.Read(buf, 0, 4);
                        n = stream.Read(buf, 0, 4);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].BlockSize = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records_67[i].BlockNum = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        //if (i == 0) //VB확인을 위한 임시적인 것. 함수 밖으로 빼도 됨
                        //{
                        //    VCSB_BlockNum = records_67[i].BlockNum;
                        //}

                        //Console.WriteLine("{0}", i + 1);
                        //Console.WriteLine("record key offset : {0}", records_67[i].KeyOffset);
                        //Console.WriteLine("record data offset : {0}", records_67[i].DataOffset);
                        //Console.WriteLine("record node id : {0}", records_67[i].NodeID);
                        //Console.WriteLine("record structure id : {0}", records_67[i].StructureID);
                        //Console.WriteLine("record block size: {0}", records_67[i].BlockSize);
                        //Console.WriteLine("record block num: {0}", records_67[i].BlockNum);
                    }
                    break;
            }
            
        }
    }
}
