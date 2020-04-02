using System;
using System.IO;

namespace APFS
{
    public struct Utility
    {
        public static Int64 stdTime = new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
        // APFS standard : 1970/01/01/00:00:00 (nano second), DateTime structure standard : 0001/01/01/00:00:00 (100 nano second)

        /* -hex_to_dateTime-
         * hex 로 된 little endian 을 받아서 시간으로 바꾸어준다.
         */
        public static DateTime hex_to_dateTime(string nanosec)
        {
            Int64 modTime = (Int64)Utility.little_hex_to_uint64(nanosec, 8) / 100;
            DateTime newTime = new DateTime(modTime + stdTime);

            return newTime;
        }

        /* -little_hex_to_uint64-
         * little endian hex의 string을 그에 맞는 decimal로 바꿔준다.
         *  len : byte count
         *  parameter : hex = "1203", len = 2
         *   return : 786
         */
        public static UInt64 little_hex_to_uint64(string hex, int len)
        {
            string big_endian = "";
            UInt64 dec = 0;
            int start = 0;
            len *= 2;
            while (start < len)
            {
                big_endian = hex.Substring(start, 2) + big_endian;
                start += 2;
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

        public static char[] hex_to_charArray(string hex)
        {
            /*
             *  hex를 char array로 변환.
             */
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return System.Text.Encoding.ASCII.GetString(raw).ToCharArray();
        }
    }
}

