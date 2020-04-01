using System;
using System.IO;

namespace APFS
{

    public struct VB
    {
        public static UInt64 VCSB_BlockNum; //0x20


        public static void save_recordVB(FileStream stream, UInt64 block_num, UInt64 Table_type, UInt64 record_num, Table header)
        {
            int n;
            string hex;
            byte[] buf = new byte[64];
            UInt64 block_addr = Utility.get_address(block_num);

            switch (Table_type)
            {
                case 7:

                    TableType7[] records = new TableType7[record_num];
                    for (ulong i = 0; i < record_num; i++)
                    {
                        records[i] = new TableType7();

                        stream.Seek((Int64)block_addr + Convert.ToInt64("0x38", 16) + (Int64)i * 4, SeekOrigin.Begin);
                        n = stream.Read(buf, 0, 2);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].KeyOffset = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        //stream.Seek((Int64)block_addr + Convert.ToInt64("0x3A", 16)+ (Int64)i*4, SeekOrigin.Begin);
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
                        stream.Seek((Int64)block_addr + 4096 - 40 - (Int64)records[i].DataOffset, SeekOrigin.Begin);
                        stream.Read(buf, 0, 4);
                        n = stream.Read(buf, 0, 4);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].BlockSize = (UInt16)Utility.little_hex_to_uint64(hex, n);

                        n = stream.Read(buf, 0, 8);
                        hex = BitConverter.ToString(buf).Replace("-", String.Empty);
                        records[i].BlockNum = (UInt64)Utility.little_hex_to_uint64(hex, n);

                        if (i == 0)
                        {
                            VCSB_BlockNum = records[i].BlockNum;
                        }

                        Console.WriteLine("{0}", i+1);
                        Console.WriteLine("record key offset : {0}", records[i].KeyOffset);
                        Console.WriteLine("record data offset : {0}", records[i].DataOffset);
                        Console.WriteLine("record node id : {0}", records[i].NodeID);
                        Console.WriteLine("record structure id : {0}", records[i].StructureID);
                        Console.WriteLine("record block size: {0}", records[i].BlockSize);
                        Console.WriteLine("record block num: {0}", records[i].BlockNum);



                    }
                    break;
            }
        }
    }
}
