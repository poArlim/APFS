using System;
using System.IO;
namespace APFS
{
    public class RECORD
    {
        public FileFolderRecord[] ff;
        public NameAttribute[] na;
        public ExtentStatus[] es;
        public ExtentRecord[] er;
        public KeyRecord[] kr;
        public int n_ffr = 0, n_na = 0, n_es = 0, n_er = 0, n_kr = 0;

        //public static void Main()
        //{
        //    using (FileStream fs = new FileStream(@"/Users/seungbin/Downloads/han.dmg", FileMode.Open))
        //    {
        //        CSB.MSB_Address = 20480;
        //        CSB.BlockSize = 4096;

        //        init_btln(fs, 323); //148000

        //    }
        //}
        public static RECORD init_btln(FileStream stream, UInt64 block_num)
        {
            int n_ffr = 0, n_na = 0, n_es = 0, n_er = 0, n_kr = 0;

            RECORD btln = new RECORD();
            Table header = Table.get_table_header(stream, block_num);
 
            TableType[] table_info = Table.save_record(stream, block_num, header);

            btln.ff = new FileFolderRecord[header.record_num];
            btln.na = new NameAttribute[header.record_num];
            btln.es = new ExtentStatus[header.record_num];
            btln.er = new ExtentRecord[header.record_num];
            btln.kr = new KeyRecord[header.record_num];


            //Console.WriteLine("KeyOffset : {0}", table_info[0].KeyOffset);
            //Console.WriteLine("KeyLength : {0}", table_info[0].KeyLength);
            //Console.WriteLine("DataOffset  : {0}", table_info[0].DataOffset);
            //Console.WriteLine("DataLength : {0}", table_info[0].DataLength);
            //Console.WriteLine("KeySection : {0}", table_info[0].KeySection);
            //Console.WriteLine("DataSection : {0}", table_info[0].DataSection);

            for (int i = 0; i < header.record_num; i++)
            {
                char record_type = table_info[i].KeySection[14];
                Console.WriteLine("[{0}] record_type : {1}", i + 1, record_type);
                switch (record_type)
                {
                    case '3':
                        btln.ff[n_ffr] = FileFolderRecord.get(table_info[i].KeySection, table_info[i].DataSection);
                        n_ffr += 1;
                        break;

                    case '4':
                        //btln.na[n_na] = NameAttribute.get(table_info[i].KeySection, table_info[i].DataSection);
                        n_na += 1;
                        break;

                    case '6':
                        btln.es[n_es] = ExtentStatus.get(table_info[i].KeySection, table_info[i].DataSection);
                        n_es += 1;
                        break;

                    case '8':
                        btln.er[n_er] = ExtentRecord.get(table_info[i].KeySection, table_info[i].DataSection);
                        n_er += 1;
                        break;

                    case '9':
                        btln.kr[n_kr] = KeyRecord.get(table_info[i].KeySection, table_info[i].DataSection);
                        n_kr += 1;
                        break;
                        
                }
            }
            return btln;
        }


    }
 
    public class FileFolderRecord // 0x30
    {
        
        //key section

        //data section 
        public UInt64 ParentID; //0x00
        public UInt64 NodeID; //0x08
        public DateTime CreateTime; //0x10
        public DateTime ModifyTime; //0x18
        public DateTime InodeModifyTime; //0x20
        public DateTime AccessedTime; //0x28
        public UInt64 FileFolderNum; //0x38
        public UInt64 HardlinkNum; //0x40
        public UInt32 OwnerID; //0x48
        public UInt32 GroupID; //0x4c
        public UInt64 Flag; //0x50
        public UInt16 LengthMethod; //0x5c
        public UInt16 NameLengross; //0x5e
        public UInt16 DataType; //0x60
        public UInt16 FileNameLength; //0x62
        public char[] FileName; //0x68
        public UInt64 ContentLenLog; //file바로 뒤
        public UInt64 ContentLenGross; //위에 이어서

          
        public static FileFolderRecord get(string key, string data)
        {
            FileFolderRecord ffr = new FileFolderRecord();
            return ffr;
        }

    }

    public class NameAttribute // 0x40
    {
        //key section

        //data section
        //public static NameAttribute get(string key, string data)
        //{

        //}
    }

    public class ExtentStatus // 0x60
    {
        //key section

        //data section
        public UInt32 ExtentExist; //0x00

        public static ExtentStatus get(string key, string data)
        {
            string hex;
            ExtentStatus es = new ExtentStatus();
            hex = data.Substring(0, 8);
            es.ExtentExist = (UInt32)Utility.little_hex_to_uint64(hex, 4);

            Console.WriteLine("es.ExtentExist : {0}", es.ExtentExist);
            return es;
        }
    }

    public class ExtentRecord // 0x80
    {
        //key section

        //data section
        public UInt64 ExtentLength; //0x00
        public UInt64 ExtentStartBlockNum; //0x08

        public static ExtentRecord get(string key, string data)
        {
            string hex;
            ExtentRecord er = new ExtentRecord();
            hex = data.Substring(0, 16);
            er.ExtentLength = (UInt32)Utility.little_hex_to_uint64(hex, 8);
            hex = data.Substring(16, 16);
            er.ExtentStartBlockNum = (UInt32)Utility.little_hex_to_uint64(hex, 8);

            Console.WriteLine("na.ExtentExist : {0}", er.ExtentLength);
            Console.WriteLine("na.ExtentStartBlockNum : {0}", er.ExtentStartBlockNum);
            return er;
        }
    }

    public class KeyRecord // 90
    {
        //key section
        public Byte KeyLength; //0x08
        public char[] Key; //0x0c

        //data section
        public UInt64 CNID; //0x00
        public DateTime AddedDate; //0x08

        public static KeyRecord get(string key, string data)
        {
            string hex;
            KeyRecord kr = new KeyRecord();
            //key section
            hex = key.Substring(16, 2);
            kr.KeyLength = (Byte)Utility.little_hex_to_uint64(hex, 1);
            hex = key.Substring(24, kr.KeyLength*2);
            kr.Key = Utility.hex_to_charArray(hex); 

            //data section
            hex = data.Substring(0, 16);
            kr.CNID = (UInt64)Utility.little_hex_to_uint64(hex, 8);
            hex = data.Substring(16, 16);
            kr.AddedDate = Utility.hex_to_dateTime(hex);

            Console.WriteLine("kr.KeyLength : {0}", kr.KeyLength);
            Console.Write("kr.Key ");
            for (int i = 0; i < kr.KeyLength-1; i++)
            {
                Console.Write("{0}", kr.Key[i]);
            }
            Console.WriteLine();
            Console.WriteLine("kr.CNID : {0}", kr.CNID);
            Console.WriteLine("kr.AddedDate : {0}", kr.AddedDate);

            return kr;
        }
    }
}
