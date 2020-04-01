using System;
using System.IO;

namespace APFS
{
    public class CSBD
    {
        public UInt64 CSBD_Address;
        public UInt16 TableType; //0x20
        public UInt16 RecordNum; //0x24
        public CSBD_recode[] records;
        //static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/im-aron/Documents/한컴GMD/han.dmg", FileMode.Open))
        //    {
        //        CSBD csbd = init_csbd(fs, 0);
        //        csbd.records = new CSBD_recode[csbd.RecordNum];
        //        UInt64 start_addr = csbd.CSBD_Address + 40; //40 = 0x28
        //        for (UInt16 i = 0; i < csbd.RecordNum; i++)
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine($"record[{i}]");
        //            csbd.records[i] = CSBD_recode.init_csbd_recode(fs, start_addr);
        //            start_addr += 40; // 40 = 0x28
        //        }



        //    }

        //}
        public static CSBD init_csbd(FileStream fs, UInt64 start_addr)   //CSB_Address 받아서 바로 계산하고 싶은데 -> 추후 수정하자
        {
            CSBD csbd = new CSBD();
            UInt64 block_addr;
            int n;
            string hex;
            byte[] buf = new byte[64];
            csbd.CSBD_Address = 49152;//Utility.get_string_address(fs, start_addr, "NXSB") - 32 - CSB.BlockSize;
            Console.WriteLine("CSBD_Address = {0}", csbd.CSBD_Address);
            block_addr = csbd.CSBD_Address;

            fs.Seek((Int64)block_addr + Convert.ToInt64("0x20", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd.TableType = (UInt16)Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 2); // Unknown 비워주ㅣ
            n = fs.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd.RecordNum = (UInt16)Utility.little_hex_to_uint64(hex, n);

            Console.WriteLine("CSBD_Address : {0}", csbd.CSBD_Address);
            Console.WriteLine("TableType : {0}", csbd.TableType);
            Console.WriteLine("RecordNum : {0}", csbd.RecordNum);

            return csbd;
        }
    }
    public class CSBD_recode
    {
        public UInt64 Record_Address;
        public UInt16 RecordID; //0x00
        public UInt16 RecordSize; //0x02
        public UInt64 BlockSize; //0x08
        public UInt64 ObjectID; //0x18
        public UInt64 BMD_BlockNum; //0x20

        public static CSBD_recode init_csbd_recode(FileStream fs, UInt64 start_addr)
        {
            CSBD_recode csbd_recode = new CSBD_recode();
            int n;
            string hex;
            byte[] buf = new byte[64];
            csbd_recode.Record_Address = start_addr;

            fs.Seek((Int64)csbd_recode.Record_Address + Convert.ToInt64("0x00", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd_recode.RecordID = (UInt16)Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 2);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd_recode.RecordSize = (UInt16)Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)csbd_recode.Record_Address + Convert.ToInt64("0x08", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd_recode.BlockSize = (UInt16)Utility.little_hex_to_uint64(hex, n);

            fs.Seek((Int64)csbd_recode.Record_Address + Convert.ToInt64("0x18", 16), SeekOrigin.Begin);
            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd_recode.ObjectID = (UInt16)Utility.little_hex_to_uint64(hex, n);

            n = fs.Read(buf, 0, 8);
            hex = BitConverter.ToString(buf).Replace("-", String.Empty);
            csbd_recode.BMD_BlockNum = (UInt16)Utility.little_hex_to_uint64(hex, n);

            Console.WriteLine("Record_Address : {0}", csbd_recode.Record_Address);
            Console.WriteLine("RecordID : {0}", csbd_recode.RecordID);
            Console.WriteLine("RecordSize : {0}", csbd_recode.RecordSize);
            Console.WriteLine("BlockSize : {0}", csbd_recode.BlockSize);
            Console.WriteLine("ObjectID : {0}", csbd_recode.ObjectID);
            Console.WriteLine("BlockNum : {0}", csbd_recode.BMD_BlockNum);

            return csbd_recode;
        }
    }

    //맨처음이 bitmap area의 location(BMD)

}
