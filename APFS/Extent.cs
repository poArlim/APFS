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

        //        create_dir(17, "");
        //        create_dir(18, "1/");
                
        //        Console.WriteLine("Fin");
        //    }
        //}

        // Stream에서 특정 Offset부터 Count만큼 한 Extent로 묶어 냄
        public long Offset { get; set; }

        public long Count { get; set; }

        public Stream Stream;

        public byte[] buf;

        public static void create_dir(UInt64 NodeID, String path)
        {
            int idx = RECORD.NodeID_ffrIdx_dic[NodeID];
            String folder_name = new string(RECORD.ffr_list[idx].FileName, 0, RECORD.ffr_list[idx].FileName.Length - 1);
            path = Path.Combine(path, folder_name);
            Console.WriteLine("dir-path : {0}", path);
            Directory.CreateDirectory(path); 
        }

        public static void write_extent(UInt64 NodeID, byte[] buf, long count, String path)
        {
            int idx = RECORD.NodeID_ffrIdx_dic[NodeID];
            String file_name = new string(RECORD.ffr_list[idx].FileName, 0, RECORD.ffr_list[idx].FileName.Length - 1);
            path = Path.Combine(path, file_name);
            Console.WriteLine("path : {0}", path);
            if (!File.Exists(path))
            {
                try
                {
                    //TODO : file mode에 맞춰서 만들기
                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(buf, 0, (int)count);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
            }
            else
            {
                Console.WriteLine("*****Already Exist Files, path : {0}", path);
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
