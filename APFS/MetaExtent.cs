using System;
using System.IO;
using System.Collections.Generic;
namespace APFS
{
    public class MetaExtent //EDB or EIB
    {
        public static List<MetaExtent> edbList;
        //EDB
        public UInt64 block_num_start;
        public int datatype;
        public UInt64 blocks_in_extent;
        public UInt64 NodeID;

        //EIB
        public UInt64 lowest_block_start;
        public UInt64 blockNum_to_extent; //block number of edb

        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/han.dmg", FileMode.Open))
        //    {
        //        CSB.TotalSize = (UInt64)fs.Length;
        //        CSB.BlockSize = 4096;
        //        CSB.MSB_Address = 20480;

        //        // CSB csb = CSB.init_csb(fs, 0);
        //        Console.WriteLine("Start");
        //        init(fs, 96);
        //        Console.WriteLine("Fin");
        //    }

        //    //using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/noname.dmg", FileMode.Open))
        //    //{
        //    //    CSB.TotalSize = (UInt64)fs.Length;
        //    //    CSB.BlockSize = 4096;
        //    //    CSB.MSB_Address = 209735680;
        //    //    // CSB csb = CSB.init_csb(fs, 0);
        //    //    Console.WriteLine("Start");
        //    //    init(fs, 421559);
        //    //    Console.WriteLine("Fin");

        //    //}
        //}

        public static void init(FileStream stream, UInt64 block_num)
        {
            Table header = Table.get_table_header(stream, block_num);

            edbList = new List<MetaExtent>();

            //Console.WriteLine("extent block address : {0}", Utility.get_address(block_num));
            //Console.WriteLine("record_num : {0}", header.record_num);
            //Console.WriteLine("table type : {0}", header.table_type);

            if (header.table_type == 1) //EIB
            {
                MetaExtent[] eib = new MetaExtent[header.record_num];
                eib = get_eib(stream, header, block_num);
                foreach (MetaExtent e in eib)
                {
                    UInt64 edb_blockNum = e.blockNum_to_extent;
                    header = Table.get_table_header(stream, edb_blockNum);
                    var edb = get_edb(stream, header, edb_blockNum);
                    //Console.WriteLine("edb block_num: {0}, {1}", edb_blockNum, Utility.get_address(edb_blockNum));
                    //int i = 0;
                    //foreach (MetaExtent a in edb)
                    //{
                    //    Console.WriteLine(i++);
                    //    Console.WriteLine("block_num_start : {0}, {1}", a.block_num_start, Utility.get_address(a.block_num_start));
                    //    Console.WriteLine("datatype : {0}", a.datatype);
                    //    Console.WriteLine("blocks_in_extent : {0}", a.blocks_in_extent);
                    //    Console.WriteLine("NodeID : {0}\n\n", a.NodeID);

                    //}
                    if (edb != null)
                        edb.AddRange(edb);
                }
            }
            else
            {
                UInt64 edb_blockNum = block_num;
                header = Table.get_table_header(stream, edb_blockNum);
                var edb = get_edb(stream, header, edb_blockNum);
                //foreach (MetaExtent a in edb)
                //{
                //    Console.WriteLine("block_num_start : {0}, {1}", a.block_num_start, Utility.get_address(a.block_num_start));
                //    Console.WriteLine("datatype : {0}", a.datatype);
                //    Console.WriteLine("blocks_in_extent : {0}", a.blocks_in_extent);
                //    Console.WriteLine("NodeID : {0}, 0x{1}\n\n", a.NodeID, a.NodeID.ToString("X"));

                //}

                if (edb != null)
                    edb.AddRange(edb);
            }


        }


        public static List<MetaExtent> get_edb(FileStream stream, Table header, UInt64 block_num)
        {
            List<MetaExtent> edb = new List<MetaExtent>();
            TableType[] table_info = Table.save_record(stream, block_num, header);
            //    Console.WriteLine("----edb record num : {0}------", header.record_num);
            //   Console.WriteLine("----edb table_type : {0}------", header.table_type);
            for (int i = 0; i < header.record_num; i++)
            {
                MetaExtent e = new MetaExtent();
                int start, len;
                string key = table_info[i].KeySection;
                string data = table_info[i].DataSection;

                start = 0;
                len = 7;
                e.block_num_start = Utility.little_hex_to_uint64(key.Substring(start, 2 * len), len);

                start += 2 * len;
                len = 1;
                e.datatype = (int)Utility.little_hex_to_uint64(key.Substring(start, 2 * len), len);

                start = 0;
                len = 7;
                e.blocks_in_extent = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);

                start += 2 * len + 2;
                len = 8;
                e.NodeID = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);

                edb.Add(e);

            }

            return edb;

        }
        public static MetaExtent[] get_eib(FileStream stream, Table header, UInt64 block_num)
        {
            MetaExtent[] eib = new MetaExtent[header.record_num];
            TableType[] table_info = Table.save_record(stream, block_num, header);

            for (int i = 0; i < header.record_num; i++)
            {
                eib[i] = new MetaExtent();

                string key = table_info[i].IndexKey;

                int start = 0;
                int len = 6;
                eib[i].lowest_block_start = Utility.little_hex_to_uint64(key.Substring(start, 2 * len), len);
                eib[i].blockNum_to_extent = table_info[i].Node_ID;
                //Console.WriteLine(i);
                //Console.WriteLine("lowest block# start : {0}, {1}", eib[i].lowest_block_start, Utility.get_address(eib[i].lowest_block_start));
                //Console.WriteLine("blockNum to Extent : {0}, {1}", eib[i].blockNum_to_extent, Utility.get_address(eib[i].blockNum_to_extent));

            }
            return eib;
        }
    }
}