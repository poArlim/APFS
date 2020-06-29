using System;
using System.IO;
using System.Collections.Generic;

namespace APFS
{
    public class RECORD
    {
        public static Dictionary<UInt64, List<UInt64>> parent_node_dic = new Dictionary<UInt64, List<UInt64>>() ; 
        
        public static Dictionary<UInt64, FileFolderRecord> ffr_dict = new Dictionary<UInt64, FileFolderRecord>();
        
        public static List<ExtentStatus> es_list = new List<ExtentStatus>();
        public static List<KeyRecord> kr_list = new List<KeyRecord>();
        public static Dictionary<UInt64, ExtentRecord> er_dic = new Dictionary<UInt64, ExtentRecord>(); //NodeID, ExtentRecord

        public static void init_btln(FileStream stream, UInt64 block_num)
        {
           
            Table header = Table.get_table_header(stream, block_num);
            if(header.table_type == 0)
            {
                return;
            }
 
            TableType[] table_info = Table.save_record(stream, block_num, header);



            //Console.WriteLine("block_num: {0}, 0x{1}", block_num, block_num.ToString("X"));
            //Console.WriteLine("table type : {0}", header.table_type);

            for (int i = 0; i < header.record_num; i++)
            {
                char record_type = table_info[i].KeySection[14];
                //Console.WriteLine("\n\n{0} record\nrecord_type {1}", i, record_type);
                //Console.WriteLine("key len : {0}, data len {1}", table_info[i].KeyLength, table_info[i].DataLength);
                switch (record_type)
                {
                    case '3':
                        FileFolderRecord f = new FileFolderRecord(); 
                        try
                        {
                            f = FileFolderRecord.get(table_info[i].KeySection, table_info[i].DataSection);

                          
                            String fname = new string(f.FileName, 0, f.FileName.Length - 1);
                            // Console.WriteLine("Node ID : {0}, ParentID : {1}, Filename : {2}, Flag : {3} ", f.NodeID, f.ParentID, fname, f.Flag ); 
                            if (!ffr_dict.ContainsKey(f.NodeID))
                            {
                                ffr_dict.Add(f.NodeID, f); 
                            }
                            else
                            {
                                ffr_dict[f.NodeID] = f; 
                            }

                       
                            if (!parent_node_dic.ContainsKey(f.ParentID))
                            {
                                List<UInt64> node_list = new List<UInt64>();
                                node_list.Add(f.NodeID);
                                parent_node_dic.Add(f.ParentID, node_list);
                            }
                            else 
                            {
                                List<UInt64> child_list = parent_node_dic[f.ParentID];
                                if(!child_list.Exists(x => x == f.NodeID))
                                    parent_node_dic[f.ParentID].Add(f.NodeID); 
                            }
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("**************An element with Key = {0} already exists.", f.NodeID);
                        }
                 
                        break;

                    //case '6':
                    //    ExtentStatus es = ExtentStatus.get(table_info[i].KeySection, table_info[i].DataSection);
                    //    RECORD.es_list.Add(es);
    
                    //    break;

                    case '8':
                        ExtentRecord er = ExtentRecord.get(table_info[i].KeySection, table_info[i].DataSection);
                        if (!er_dic.ContainsKey(er.NodeID))
                        {
                            er_dic.Add(er.NodeID, er); 
                        }
                        else
                        {
                          //  Console.WriteLine("*****Modify Extent Record , nodeID{0}", er.NodeID); 
                            er_dic[er.NodeID] = er; 
                        }
               
                        break;

                    //case '9':
                    //    KeyRecord kr = KeyRecord.get(table_info[i].KeySection, table_info[i].DataSection);
                    //    RECORD.kr_list.Add(kr);
                
                    //    break;
                }
            }


            return;
        }


    }
 
    public class FileFolderRecord // 0x30
    {
        public string path;
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
        public string Flag; //0x50
        public UInt16 LengthMethod; //0x5c
        public UInt16 NameLengross; //0x5e
        public UInt16 DataType; //0x60
        public UInt16 FileNameLength; //0x62
        public char[] FileName; //0x68
        public UInt64 ContentLenLog; // lengthMethod가 2일 경우에만
        public UInt64 ContentLenGross; // lengthMethod가 2일 경우에만



          
        public static FileFolderRecord get(string key, string data)
        {
            //Console.WriteLine("key : {0}, data : {1}", key, data);

            int start, len;
            FileFolderRecord ffr = new FileFolderRecord();

            start = 0;
            len = 8;
            ffr.ParentID = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("ParentID : {0}, {1}", ffr.ParentID, data.Substring(start, 2 * len));

            start += 2 * len; 
            len = 8;
            ffr.NodeID = Utility.little_hex_to_uint64(data.Substring(start , 2 * len), len);
            //Console.WriteLine("NodeID : {0}, {1}", ffr.NodeID, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 8;
            ffr.CreateTime = Utility.hex_to_dateTime(data.Substring(start, 2 * len));
            //Console.WriteLine("CreateTime : {0}", ffr.CreateTime);

            start += 2 * len;
            len = 8;
            ffr.ModifyTime = Utility.hex_to_dateTime(data.Substring(start, 2 * len));
            //Console.WriteLine("ModifyTime : {0}", ffr.ModifyTime);

            start += 2 * len;
            len = 8;
            ffr.InodeModifyTime = Utility.hex_to_dateTime(data.Substring(start, 2 * len));
            //Console.WriteLine("InodeModifyTime : {0}", ffr.InodeModifyTime);

            start += 2 * len;
            len = 8;
            ffr.AccessedTime = Utility.hex_to_dateTime(data.Substring(start, 2 * len));
            //Console.WriteLine("AccessedTime : {0}", ffr.AccessedTime);

            start += 4 * len;
            len = 8;
            ffr.FileFolderNum = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("FileFolderNum : {0}, {1}", ffr.FileFolderNum, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 8;
            ffr.HardlinkNum = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("HardlinkNum : {0}, {1}", ffr.HardlinkNum, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 4;
            ffr.OwnerID = (uint)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("OwnerID : {0}, {1}", ffr.OwnerID, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 4;
            ffr.GroupID = (uint)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("GroupID : {0}, {1}", ffr.GroupID, data.Substring(start, 2 * len));

            /* - Flag - 
             * folder : 0100000 + rwxrwxrwx
             * file : 1000000+ rwxrwxrwx
             */

            start += 2 * len;
            len = 8;
            ffr.Flag = Utility.littleEndian_to_bigEndian(data.Substring(start, 2 * len), 2);
            //Console.WriteLine("Flag : {0}, {1}", ffr.Flag, data.Substring(start, 2 * len));


            start += 2 * len + 8;
            len = 2;
            ffr.LengthMethod = (UInt16)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("LengthMethod : {0}, {1}", ffr.LengthMethod, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 2;
            ffr.NameLengross = (UInt16)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("NameLengross : {0}, {1}", ffr.NameLengross, data.Substring(start, 2 * len));


            start += 2 * len;
            len = 2;
            ffr.DataType = (UInt16)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("DataType : {0}, {1}", ffr.DataType, data.Substring(start, 2 * len));

            start += 2 * len;
            len = 2;
            ffr.FileNameLength = (UInt16)Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
            //Console.WriteLine("FileNameLength : {0}, {1}", ffr.FileNameLength, data.Substring(start, 2 * len));


            if (ffr.LengthMethod == 1)
            {
                start += 2 * len;
                len = ffr.FileNameLength;
                ffr.FileName = Utility.hex_to_charArray(data.Substring(start, 2 * len));
            }
            else
            {

                start += 12;
                len = ffr.FileNameLength;
                ffr.FileName = Utility.hex_to_charArray(data.Substring(start, 2 * len));

                start += 2 * (len + 8 - len % 8);
                len = 8;
                ffr.ContentLenLog = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
                //Console.WriteLine("ContentLenLog : {0}, {1}", ffr.ContentLenLog, data.Substring(start, 2 * len));

                start += 2 * len;
                len = 8;
                ffr.ContentLenGross = Utility.little_hex_to_uint64(data.Substring(start, 2 * len), len);
                //Console.WriteLine("ContentLenGross : {0}, {1}", ffr.ContentLenGross, data.Substring(start, 2 * len));

            }

            return ffr;
        }

    }


    public class ExtentStatus // 0x60
    {
        //key section
        public UInt64 NodeID; //0x00
        //data section
        public UInt32 ExtentExist; //0x00


        public static ExtentStatus get(string key, string data)
        {
            string hex;
            ExtentStatus es = new ExtentStatus();
            hex = key.Substring(0, 14);
            es.NodeID = (UInt64)Utility.little_hex_to_uint64(hex, 7); 
            hex = data.Substring(0, 8);
            es.ExtentExist = (UInt32)Utility.little_hex_to_uint64(hex, 4);

            return es;
        }
    }

    public class ExtentRecord // 0x80
    {
        //key section
        public UInt64 NodeID; //0x00
        //data section
        public UInt64 ExtentLength; //0x00
        public UInt64 ExtentStartBlockNum; //0x08


        public static ExtentRecord get(string key, string data)
        {
            string hex;
            ExtentRecord er = new ExtentRecord();
            hex = key.Substring(0, 14);
            er.NodeID = (UInt64)Utility.little_hex_to_uint64(hex, 7);
            hex = data.Substring(0, 16);
            er.ExtentLength = (UInt32)Utility.little_hex_to_uint64(hex, 8);
            hex = data.Substring(16, 16);
            er.ExtentStartBlockNum = (UInt32)Utility.little_hex_to_uint64(hex, 8);

            //Console.WriteLine("er.NodeID:{0}", er.NodeID); 
            //Console.WriteLine("er.ExtentLength : {0}", er.ExtentLength);
            //Console.WriteLine("er.ExtentStartBlockNum : {0}", er.ExtentStartBlockNum);
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

            //Console.WriteLine("kr.KeyLength : {0}", kr.KeyLength);
            //Console.Write("kr.Key ");
            //for (int i = 0; i < kr.KeyLength-1; i++)
            //{
            //    //Console.Write("{0}", kr.Key[i]);
            //}
            //Console.WriteLine();
            //Console.WriteLine("kr.CNID : {0}", kr.CNID);
            //Console.WriteLine("kr.AddedDate : {0}", kr.AddedDate);

            return kr;
        }
    }
}
