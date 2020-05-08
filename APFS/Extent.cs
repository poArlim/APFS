using System;
using System.IO;
using System.Collections.Generic;

namespace APFS
{
    public class Extent
    {
        //public static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/han.dmg", FileMode.Open))
        //    {
        //        CSB.TotalSize = (UInt64)fs.Length;
        //        CSB.BlockSize = 4096;
        //        CSB.MSB_Address = 20480;

        //        // CSB csb = CSB.init_csb(fs, 0);
        //        Console.WriteLine("Start");
        //        List<BTCS> btrn_btln = BTCS.init_btom(fs, 327);
        //        int n = 0;
        //        foreach (BTCS b in btrn_btln)
        //        {
        //            n++;
        //            //Console.WriteLine("node id : {0}", b.NodeID);
        //            //Console.WriteLine("Checkpoint : {0}", b.Checkpoint);
        //            //Console.WriteLine("block num: {0}, {1}\n", b.BlockNum, Utility.get_address(b.BlockNum));
        //            if (n > 1)
        //            {

        //                RECORD.init_btln(fs, b.BlockNum);
        //            }
        //        }

        //        foreach (FileFolderRecord f in RECORD.ffr_list)
        //        {
        //            int idx = RECORD.NodeID_ffrIdx_dic[f.NodeID];
        //            Console.WriteLine("NodeID :{0} == {1} ", f.NodeID, RECORD.ffr_list[idx].NodeID);
        //        }
        //        Console.WriteLine();

        //        //Extent
        //        Console.WriteLine();
        //        UInt64 Extent_addr = 314;
        //        Console.WriteLine("Extent : {0}", Extent_addr);

        //        MetaExtent.init(fs, Extent_addr);
        //        /*
        //         * RECORD.ffr_list[idx].Flag[0] == 8 -> file
        //         * RECORD.ffr_list[idx].Flag[0] == 4 -> folder
        //         */
        //        n = 0;
        //        foreach (MetaExtent a in MetaExtent.edbList)
        //        {
        //            Console.WriteLine("\n\n{0}", n++);
        //            int idx = RECORD.NodeID_ffrIdx_dic[a.NodeID];
        //            Console.WriteLine("NodeID : {0}", a.NodeID);
        //            Console.WriteLine("Flag : {0}", RECORD.ffr_list[idx].Flag[0]);
        //            Console.WriteLine("block_num_start : {0}, {1}", a.block_num_start, Utility.get_address(a.block_num_start));
        //            Console.WriteLine("datatype : {0}", a.datatype);
        //            Console.WriteLine("blocks_in_extent : {0}", a.blocks_in_extent);
        //            String fname = new string(RECORD.ffr_list[idx].FileName, 0, RECORD.ffr_list[idx].FileName.Length-1); 
        //            Console.WriteLine("Filename : {0}", fname);
        //            Extent new_extent = Extent.read_extent(fs, (long)Utility.get_address(a.block_num_start), (long)a.blocks_in_extent * CSB.BlockSize);
        //            write_extent(a, new_extent.buf, new_extent.Count, ""); 
        //        }

        //        Console.WriteLine("Fin");
        //    }
        //}

        // Stream에서 특정 Offset부터 Count만큼 한 Extent로 묶어 냄
        public long Offset { get; set; }

        public long Count { get; set; }

        public Stream Stream;

        public byte[] buf;

        public static void write_extent(MetaExtent file, byte[] buf, long count, String path)
        {
            int idx = RECORD.NodeID_ffrIdx_dic[file.NodeID];
            String file_name = new string(RECORD.ffr_list[idx].FileName, 0, RECORD.ffr_list[idx].FileName.Length - 1);
            path = Path.Combine(path, file_name);
            Console.WriteLine("path : {0}", path);
            if (!File.Exists(path))
            {
                try
                {
                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(buf, 0, (int)count);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message.ToString()); 
                }
            }

        }
        public static Extent read_extent(FileStream fs, long start_addr, long length)
        {
            Extent new_extent = new Extent();

            new_extent.Offset = start_addr;
            new_extent.Count = length;
            new_extent.buf = new byte[length]; 

            fs.Seek(start_addr, SeekOrigin.Begin);
            fs.Read(new_extent.buf, 0, (int)length);

            return new_extent;
        }

    }
}
