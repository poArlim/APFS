using System;
using System.IO;

namespace APFS
{
    public class BTCS
    {
        public UInt64 BTOM; //0x30
    }
    public class BTOM
    {
        public UInt64 NodeID;
        public UInt64 ApsbNum;
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
        //        init_btom(fs, 327);
        //        Console.WriteLine("Fin");
        //    }

        //    //using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/noname.dmg", FileMode.Open))
        //    //{
        //    //    CSB.TotalSize = (UInt64)fs.Length;
        //    //    CSB.BlockSize = 4096;
        //    //    CSB.MSB_Address = 209735680;
        //    //    // CSB csb = CSB.init_csb(fs, 0);
        //    //    Console.WriteLine("Start");
        //    //    init_btom(fs, 421574);
        //    //    Console.WriteLine("Fin");

        //    //}
        //}

        public static BTOM[] init_btom(FileStream stream, UInt64 block_num)
        {

            Table header = Table.get_table_header(stream, block_num);
            BTOM[] btom = new BTOM[header.record_num];
            TableType[] table_info = Table.save_record(stream, block_num, header);
            Footer footer = Table.save_footer(stream, block_num, header);
            if (header.table_type == 5 || header.table_type == 7)
            {
                for (int i = 0; i < header.record_num; i++)
                {
                    btom[i] = new BTOM();
                    btom[i].NodeID = table_info[i].NodeID;
                    btom[i].ApsbNum = table_info[i].StructureID;
                    btom[i].BlockNum = table_info[i].BlockNum;
                    btom[i].TotalIndexNode = footer.TotalIndexNode;
                    btom[i].TotalLeafNode = footer.TotalLeafNode;
                    Console.WriteLine(i);
                    Console.WriteLine("btom -- node id : {0}", btom[i].NodeID);
                    Console.WriteLine("btom -- ApsbNum : {0}", btom[i].ApsbNum);
                    Console.WriteLine("btom -- block num: {0}, {1}", btom[i].BlockNum, Utility.get_address(btom[i].BlockNum));
                    Console.WriteLine("btom -- TotalIndexNode : {0}", btom[i].TotalIndexNode);
                    Console.WriteLine("btom -- TotalLeafNode : {0}", btom[i].TotalLeafNode);
                }
            }

            return btom;
        }

    }




    public class BTCS_Header //BTOM, BTIN, BTRN 모두 사용가 
    {
        public UInt16 BTreeLevel; //0x18  3이면 BTIN
        public UInt16 TableType; //0x20
        public UInt16 RecordNum; //0x24

        public UInt16 tableIndexSize; //0x2a
        public UInt16 tableKeyAreaSize; //0x2c
        public UInt16 tableFreeSpaceSize; //0x2e
    }

}
