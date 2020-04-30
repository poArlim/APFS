using System;
using System.IO;
using System.Collections.Generic;
namespace APFS
{

    public class  BTCS
    {
        public UInt64 NodeID;
        public UInt64 Checkpoint;
        public UInt64 BlockNum;

        public UInt64 TotalLeafNode;
        public UInt64 TotalIndexNode;

        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/han.dmg", FileMode.Open))
        //    {
        //        CSB.TotalSize = (UInt64)fs.Length;
        //        CSB.BlockSize = 4096;
        //        CSB.MSB_Address = 20480;

        //        // CSB csb = CSB.init_csb(fs, 0);
        //        Console.WriteLine("Start");
        //        List<BTCS> btrn_btln = init_btom(fs, 327);
        //        foreach (BTCS b in btrn_btln)
        //        {
        //            Console.WriteLine("node id : {0}", b.NodeID);
        //            Console.WriteLine("Checkpoint : {0}", b.Checkpoint);
        //            Console.WriteLine("block num: {0}, {1}\n", b.BlockNum, Utility.get_address(b.BlockNum));
        //        }
        //        Console.WriteLine("Fin");
        //    }

        //    //using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/noname.dmg", FileMode.Open))
        //    //{
        //    //    CSB.TotalSize = (UInt64) fs.Length;
        //    //    CSB.BlockSize = 4096;
        //    //    CSB.MSB_Address = 209735680;
        //    //    // CSB csb = CSB.init_csb(fs, 0);
        //    //    Console.WriteLine("Start");
        //    //    List<UInt64> btin_block_num = new List<UInt64>();
        //    //    List<BTCS> btrn_btln = new List<BTCS>() ; 
        //    //    btin_block_num.Add(421577);
        //    //    btin_block_num.Add(421494);
        //    //    btin_block_num.Add(421575);
        //    //    btrn_btln = init_btin_all(fs, btin_block_num);
        //    //    foreach(BTCS b in btrn_btln)
        //    //    {
        //    //        Console.WriteLine("node id : {0}", b.NodeID);
        //    //        Console.WriteLine("Checkpoint : {0}", b.Checkpoint);
        //    //        Console.WriteLine("block num: {0}, {1}\n", b.BlockNum, Utility.get_address(b.BlockNum));
        //    //    }
        //    //    Console.WriteLine("Fin");
        //    //}
        //}

        public static List<BTCS> init_btin_all(FileStream stream, List<UInt64> btin_block_num)
        {
            List<BTCS> btin = new List<BTCS>();
            foreach (UInt64 block_num in btin_block_num)
            {
                var b_list = init_btin(stream, block_num);
                btin.AddRange(b_list); 
            }
            return btin; 
        }

        public static List<BTCS> init_btin(FileStream stream, UInt64 block_num)
        {

            Table header = Table.get_table_header(stream, block_num);
            List<BTCS> btin = new List<BTCS>();
            TableType[] table_info = Table.save_record(stream, block_num, header);
         //   Console.WriteLine("--------Block Num {0}, # of record {1}------", block_num, header.record_num) ;
            if (header.table_type == 6)
            {
                for (int i = 0; i < header.record_num; i++)
                {
                    BTCS b = new BTCS();
                    b.NodeID = table_info[i].NodeID;
                    b.Checkpoint = table_info[i].StructureID;
                    b.BlockNum = table_info[i].BlockNum;
                    btin.Add(b); 
                }
            }

            return btin;
        }

        //public static  BTCS[] init_btin(FileStream stream, UInt64 block_num)
        //{

        //    Table header = Table.get_table_header(stream, block_num);
        //    BTCS[] btin = new  BTCS[header.record_num];
        //    TableType[] table_info = Table.save_record(stream, block_num, header);

        //    if (header.table_type == 6)
        //    {
        //        for (int i = 0; i < header.record_num; i++)
        //        {
        //            btin[i] = new  BTCS();
        //            btin[i].NodeID = table_info[i].NodeID;
        //            btin[i].Checkpoint = table_info[i].StructureID;
        //            btin[i].BlockNum = table_info[i].BlockNum;

        //            //Console.WriteLine(i);
        //            //Console.WriteLine("btin -- node id : {0}", btin[i].NodeID);
        //            //Console.WriteLine("btin -- Checkpoint : {0}", btin[i].Checkpoint);
        //            //Console.WriteLine("btin -- block num: {0}, {1}", btin[i].BlockNum, Utility.get_address(btin[i].BlockNum));

        //        }
        //    }

        //    return btin;
        //}

        public static  List<BTCS> init_btom(FileStream stream, UInt64 block_num)
        {

            Table header = Table.get_table_header(stream, block_num);
            List<BTCS> btom = new List<BTCS>();
            TableType[] table_info = Table.save_record(stream, block_num, header);
            Footer footer = Table.save_footer(stream, block_num, header);
            if (header.table_type == 5 || header.table_type == 7)
            {
                for (int i = 0; i < header.record_num; i++)
                {
                    BTCS b = new BTCS(); 
                    b.NodeID = table_info[i].NodeID;
                    b.Checkpoint = table_info[i].StructureID;
                    b.BlockNum = table_info[i].BlockNum;
                    b.TotalIndexNode = footer.TotalIndexNode;
                    b.TotalLeafNode = footer.TotalLeafNode;
                    btom.Add(b); 
                }
            }

            return btom;
        }

    }



}
