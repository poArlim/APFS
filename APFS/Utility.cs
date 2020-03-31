using System;
using System.IO;

namespace APFS
{
    public struct Utility
    {
        static void Main()
        {
            using (FileStream fs = new FileStream(@"/Users/im-aron/Documents/한컴GMD/han.dmg", FileMode.Open))
            {
                //CSB.MSB_Address = 20480;
                CSB.BlockSize = 4096;
                CSB.TotalSize = (UInt64)fs.Length;
                //UInt64 block_num = 323;
                //Table t = get_table_header(fs, block_num);
                CSB.MSB_Address = Utility.get_string_address(fs, 0, "NXSB") - 32;
                Console.WriteLine($"CSB.MSB_Address = 0x{CSB.MSB_Address:X}");
                //Console.WriteLine("table_type : {0}", t.table_type);
                //Console.WriteLine("len_record_def : {0}", t.len_record_def);
                //Console.WriteLine("len_key_section : {0}", t.len_key_section);
                //Console.WriteLine("gap_key_data : {0}", t.gap_key_data);

            }

        }

        /* -little_hex_to_uint64-
         * little endian hex의 string을 그에 맞는 decimal로 바꿔준다.
         *  parameter : hex = "123", len = 3
         *   return : 786
         */
        public static UInt64 little_hex_to_uint64(string hex, int len)
        {
            string big_endian = "";
            UInt64 dec = 0;
            int start = 0;

            while (start < len)
            {

                if (len - start == 1)
                {
                    big_endian = hex.Substring(start, 1) + big_endian;
                    start += 1;
                }
                else
                {
                    big_endian = hex.Substring(start, 2) + big_endian;
                    start += 2;
                }

            }
            dec = Convert.ToUInt64(big_endian, 16);
            return dec;

        }

        /*-get_address
         * decimal의 block number가 들어오면, 그에 맞는 decimal의 주소를 반환한다.
         */
        public static UInt64 get_address(UInt64 blocknum)
        {
            UInt64 address;
            address = CSB.MSB_Address + blocknum * (UInt64)CSB.BlockSize;

            return address;
        }

        /* -str_to_hex-
         * string 을 받아서 그에 맞는 hex 코드로 바꾸어준다.
         */
        public static string str_to_hex(string toChange)
        {
            string toChange_hex = "";
            char[] values = toChange.ToCharArray();

            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                toChange_hex = string.Concat(toChange_hex, $"{value:X}");
            }

            return toChange_hex;
        }

        /* -get_string_address-
         * 일단 MSB의 위치를 찾는 데 사용
         * 원하는 string 을 찾아서 해당 string 의 address 를 반환한다.
         */
        public static UInt64 get_string_address(FileStream stream, UInt64 start_addr, string toSearch)
        {
            byte[] buf = new byte[CSB.BlockSize];
            string search_hex = str_to_hex(toSearch);
            UInt64 blocknum = 0;
            UInt64 offset = 0;
            UInt64 address = 0;
            UInt64 total_blocknum = CSB.TotalSize / CSB.BlockSize;

            stream.Seek((Int64)start_addr, SeekOrigin.Begin);
            for (UInt64 i = 0; i < total_blocknum; i++)
            {
                int readBytes = stream.Read(buf, 0, (int)CSB.BlockSize);
                string hex = BitConverter.ToString(buf).Replace("-", String.Empty);

                if (!hex.Contains(search_hex)) continue;
                else
                {
                    blocknum = i - 5; // temp initial block num 5
                    offset = (UInt64)hex.IndexOf(search_hex, 0) / 2;
                    address = i * CSB.BlockSize + offset;
                    break;
                    //Console.WriteLine($"block# = {blocknum}, offset = 0x{offset:X}, address = =0x{address:X}");
                }

            }
            return address;
        }
    }
}

