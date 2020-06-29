using System;
using System.IO;
using System.Collections.Generic;

namespace APFS
{
    public class Extent
    {


        // Stream에서 특정 Offset부터 Count만큼 한 Extent로 묶어 냄
        public long Offset { get; set; }

        public long Count { get; set; }

        public Stream Stream;

        public byte[] buf;


        public static void write_extent(UInt64 NodeID, byte[] buf, long count, String path)
        {
            //Console.WriteLine("         path : {0}", path);

            if (!File.Exists(path))
            {
                try
                {
             
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
