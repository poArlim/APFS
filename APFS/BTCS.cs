using System;
using System.IO;

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
        //    //using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/han.dmg", FileMode.Open))
        //    //{
        //    //    CSB.TotalSize = (UInt64)fs.Length;
        //    //    CSB.BlockSize = 4096;
        //    //    CSB.MSB_Address = 20480;

        //    //    // CSB csb = CSB.init_csb(fs, 0);
        //    //    Console.WriteLine("Start");
        //    //    init_btom(fs, 327);
        //    //    Console.WriteLine("Fin");
        //    //}

        //    using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/noname.dmg", FileMode.Open))
        //    {
        //        CSB.TotalSize = (UInt64)fs.Length;
        //        CSB.BlockSize = 4096;
        //        CSB.MSB_Address = 209735680;
        //        // CSB csb = CSB.init_csb(fs, 0);
        //        Console.WriteLine("Start");
        //        init_btin(fs, 421577);
        //        Console.WriteLine("Fin");

        //    }
        //}

        public static  BTCS[] init_btin(FileStream stream, UInt64 block_num)
        {

            Table header = Table.get_table_header(stream, block_num);
            BTCS[] btin = new  BTCS[header.record_num];
            TableType[] table_info = Table.save_record(stream, block_num, header);

            if (header.table_type == 6)
            {
                for (int i = 0; i < header.record_num; i++)
                {
                    btin[i] = new  BTCS();
                    btin[i].NodeID = table_info[i].NodeID;
                    btin[i].Checkpoint = table_info[i].StructureID;
                    btin[i].BlockNum = table_info[i].BlockNum;

                    //Console.WriteLine(i);
                    //Console.WriteLine("btin -- node id : {0}", btin[i].NodeID);
                    //Console.WriteLine("btin -- Checkpoint : {0}", btin[i].Checkpoint);
                    //Console.WriteLine("btin -- block num: {0}, {1}", btin[i].BlockNum, Utility.get_address(btin[i].BlockNum));

                }
            }

            return btin;
        }

        public static  BTCS[] init_btom(FileStream stream, UInt64 block_num)
        {

            Table header = Table.get_table_header(stream, block_num);
             BTCS[] btom = new  BTCS[header.record_num];
            TableType[] table_info = Table.save_record(stream, block_num, header);
            Footer footer = Table.save_footer(stream, block_num, header);
            if (header.table_type == 5 || header.table_type == 7)
            {
                for (int i = 0; i < header.record_num; i++)
                {
                    btom[i] = new  BTCS();
                    btom[i].NodeID = table_info[i].NodeID;
                    btom[i].Checkpoint = table_info[i].StructureID;
                    btom[i].BlockNum = table_info[i].BlockNum;
                    btom[i].TotalIndexNode = footer.TotalIndexNode;
                    btom[i].TotalLeafNode = footer.TotalLeafNode;
                    //Console.WriteLine(i);
                    //Console.WriteLine("btom -- node id : {0}", btom[i].NodeID);
                    //Console.WriteLine("btom -- Checkpoint : {0}", btom[i].Checkpoint);
                    //Console.WriteLine("btom -- block num: {0}, {1}", btom[i].BlockNum, Utility.get_address(btom[i].BlockNum));
                    //Console.WriteLine("btom -- TotalIndexNode : {0}", btom[i].TotalIndexNode);
                    //Console.WriteLine("btom -- TotalLeafNode : {0}", btom[i].TotalLeafNode);
                }
            }

            return btom;
        }

    }



}
