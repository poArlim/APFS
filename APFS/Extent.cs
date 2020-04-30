using System;
using System.IO;

namespace APFS
{
    public class Extent
    {
        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/seungbin/Downloads/han.dmg", FileMode.Open))
        //    {
        //        long extent_address = 1208320;
        //        long extent_count = 8192;

        //        Extent new_extent = read_extent(fs, extent_address, extent_count);

        //        Console.WriteLine("extent_address = {0}", new_extent.Offset);
        //        Console.WriteLine("extent_count = {0}", new_extent.Count);
        //        Console.WriteLine("extent stream = {0}", new_extent.Stream);


        //        byte[] buf = new byte[new_extent.Count];

        //        new_extent.Stream.Seek(0, SeekOrigin.Begin);
        //        new_extent.Stream.Read(buf, 0, (int)new_extent.Count);
        //        string hex = BitConverter.ToString(buf).Replace("-", String.Empty);

        //        Console.WriteLine(hex);

        //    }

        //}

        // Stream에서 특정 Offset부터 Count만큼 한 Extent로 묶어 냄
        public long Offset { get; set; }

        public long Count { get; set; }

        public Stream Stream;

        public static Extent read_extent(FileStream fs, long start_addr, long length)
        {
            Extent new_extent = new Extent();

            new_extent.Offset = start_addr;
            new_extent.Count = length;

            //Stream new_extent = new FileStream();
            new_extent.Stream = new FileStream("outStream", FileMode.Create);

            int n;
            byte[] buf = new byte[length];

            fs.Seek(start_addr, SeekOrigin.Begin);
            n = fs.Read(buf, 0, (int)length);

            new_extent.Stream.Write(buf, 0, n);

            new_extent.Stream.Close();

            return new_extent;
        }
    }
}
