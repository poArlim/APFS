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

        public static void Main()
        {
            using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/han.dmg", FileMode.Open))
            {
                CSB.MSB_Address = 20480;
                CSB.BlockSize = 4096;

                init_btln(fs, 323); //148000

            }
        }
        public static RECORD init_btln(FileStream stream, UInt64 block_num)
        {
            int n_ffr = 0, n_na = 0, n_es = 0, n_er = 0, n_kr = 0;

            RECORD btln = new RECORD();
            Table header = Table.get_table_header(stream, block_num);
 
            TableType[] table_info = Table.save_record(stream, block_num, header);
            

            //Console.WriteLine("KeyOffset : {0}", table_info[0].KeyOffset);
            //Console.WriteLine("KeyLength : {0}", table_info[0].KeyLength);
            //Console.WriteLine("DataOffset : {0}", table_info[0].DataOffset);
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

                        btln.ff[n_ffr] = FileFolderRecord.get();
                        n_ffr += 1; 
                        break;

                    case '4':
                        btln.na = NameAttribute.get();
                        btln.ff[n_na] = FileFolderRecord.get();
                        n_na += 1;
                        break;

                    case '6':
                        btln.es = ExtentStatus.get();
                        btln.ff[n_es] = FileFolderRecord.get();
                        n_es += 1;
                        break;

                    case '8':
                        btln.er = ExtentRecord.get();
                        btln.ff[n_er] = FileFolderRecord.get();
                        n_er += 1;
                        break;

                    case '9':
                        btln.kr = KeyRecord.get();
                        btln.ff[n_kr] = FileFolderRecord.get();
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
            FileFolderRecord ffr;
            return ffr; 
        }

    }

    public class NameAttribute // 0x40
    {
        //key section

        //data section
        public static NameAttribute get(string key, string data)
        {

        }
    }

    public class ExtentStatus // 0x60
    {
        //key section

        //data section
        public UInt32 ExtentExist; //0x00

        public static ExtentStatus get(string key, string data)
        {

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

        }
    }
}
