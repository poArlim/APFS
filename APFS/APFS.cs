using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace APFS
{
    public class APFS
    {
        public static String dmgPath = "", restore_to_path = "";
        static void Main()
        {
            Console.WriteLine("Enter the absolute path of the dmg file : ");
            dmgPath = Console.ReadLine();
            Console.WriteLine("Enter an absolute path to save the file system. : ");
            restore_to_path = Console.ReadLine();

            try
            {
                using (FileStream fs = new FileStream(@dmgPath, FileMode.Open))
                {

                    bool make_csb = false;
                    List<CSB> csb_list = new List<CSB>();
                    List<CSB> deleted_csb_list = new List<CSB>();
                    CSB msb = new CSB();
                    while (true)
                    {
                        String input = "";
                        Console.WriteLine("-----------------------------------------------------");
                        Console.WriteLine(" 1. Restore Filesystem Structure");
                        Console.WriteLine(" 2. TimeMachine");
                        Console.WriteLine(" 3. Restore Deleted Folder");
                        Console.WriteLine(" 4. Restore Deleted File");
                        Console.WriteLine(" 5. quit");
                        Console.WriteLine("-----------------------------------------------------");
                        input = Console.ReadLine();

                        if (!make_csb)
                        {
                            CSB.TotalSize = (UInt64)fs.Length;
                            CSB.BlockSize = (UInt32)4096;
                            csb_list = CSB.get_csb_list(fs);
                            deleted_csb_list = CSB.get_deleted_csb_list(fs, Utility.get_address(csb_list[csb_list.Count - 1].NextCSBD) + CSB.BlockSize);
                            csb_list.Sort((a, b) => a.CSB_Checkpoint.CompareTo(b.CSB_Checkpoint));
                            msb = csb_list[csb_list.Count - 1];
                            foreach (CSB csb in csb_list)
                            {
                                Console.WriteLine("*************Chckpoint : {0} \n address : {1}", csb.CSB_Checkpoint, csb.CSB_Address);
                                Console.WriteLine("checkpoint : {0}", csb.CSB_Checkpoint);
                                Console.WriteLine(" OldestCSBD : {0}", csb.OldestCSBD);
                                Console.WriteLine(" OriginalCSBD : {0}", csb.OriginalCSBD);
                                Console.WriteLine(" NextCSBD : {0}", csb.NextCSBD);

                            }
                            foreach (CSB csb in deleted_csb_list)
                            {
                                Console.WriteLine("--------------deleted Chckpoint : {0}\n address : {1}", csb.CSB_Checkpoint, csb.CSB_Address);
                                Console.WriteLine("checkpoint : {0}", csb.CSB_Checkpoint);
                                Console.WriteLine(" OldestCSBD : {0}", csb.OldestCSBD);
                                Console.WriteLine(" OriginalCSBD : {0}", csb.OriginalCSBD);
                                Console.WriteLine(" NextCSBD : {0}", csb.NextCSBD);

                            }
                            Console.WriteLine("-------------------");
                            Console.WriteLine("msb checkpoint : {0}", msb.CSB_Checkpoint);
                            make_csb = true;
                        }

                        try
                        {
                            if (input.Equals("1"))
                            {
                                try
                                {
                                    init_APFS(fs, msb);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("This APFS Cannot Recover... :");
                                    Console.WriteLine(e);
                                    return;
                                }

                            }
                            else if (input.Equals("2"))
                            {
                                int chk, idx;

                                Console.WriteLine("\n\nPlease select the checkpoint you want to go back to.");
                                Console.WriteLine("(The larger the checkpoint, the more recent checkpoint.)");
                                Console.Write("Checkpoint : ");
                                for (int i = 0; i < csb_list.Count; i++)
                                {
                                    Console.Write("{0}  ", csb_list[i].CSB_Checkpoint);
                                }
                                Console.Write("\n Deleted Checkpoint : ");
                                for (int i = 0; i < deleted_csb_list.Count; i++)
                                {
                                    Console.Write("{0}  ", deleted_csb_list[i].CSB_Checkpoint);
                                }
                                Console.WriteLine("");
                                string s_chk = Console.ReadLine();
                                chk = Convert.ToInt32(s_chk);
                                if (chk >= (int)csb_list[0].CSB_Checkpoint && chk <= (int)msb.CSB_Checkpoint)
                                {
                                    idx = chk - (int)msb.CSB_Checkpoint + csb_list.Count - 1;
                                    init_APFS(fs, csb_list[idx]);
                                }
                                else if (chk >= (int)deleted_csb_list[0].CSB_Checkpoint && chk <= (int)deleted_csb_list[deleted_csb_list.Count - 1].CSB_Checkpoint)
                                {
                                    idx = chk - (int)deleted_csb_list[deleted_csb_list.Count - 1].CSB_Checkpoint + deleted_csb_list.Count - 1;
                                    init_APFS(fs, deleted_csb_list[idx]);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Checkpoint Number");
                                }
                            }
                            else if (input.Equals("3"))
                            {
                                bool find = false;
                                String to_search = "";
                                Console.WriteLine("Enter the name of the Folder you want to restore : ");
                                to_search = Console.ReadLine();
                                for (int i = csb_list.Count - 1; i >= 0; i--)
                                {
                                    CSB c = csb_list[i];
                                    Console.WriteLine("=========Chckpoint : {0}", c.CSB_Checkpoint);
                                    if (restore_folder(fs, c, to_search))
                                    {
                                        find = true;
                                        break;
                                    }
                                }

                                if (!find)
                                {
                                    for (int i = deleted_csb_list.Count - 1; i >= 0; i--)
                                    {
                                        Console.WriteLine("=========deleted Chckpoint : {0}", deleted_csb_list[i].CSB_Checkpoint);
                                        if (restore_folder(fs, deleted_csb_list[i], to_search))
                                        {
                                            find = true;
                                            break;
                                        }

                                    }
                                }
                            }
                            else if (input.Equals("4"))
                            {
                                bool find = false;
                                String to_search = "";
                                Console.WriteLine("Enter the name of the file you want to restore : ");
                                to_search = Console.ReadLine();
                                for (int i = csb_list.Count - 1; i >= 0; i--)
                                {
                                    Console.WriteLine("=========Chckpoint : {0}", csb_list[i].CSB_Checkpoint);
                                    if (restore_file(fs, csb_list[i], to_search))
                                    {
                                        find = true;
                                        break;
                                    }
                                }
                                if (!find)
                                {
                                    for (int i = deleted_csb_list.Count - 1; i >= 0; i--)
                                    {
                                        Console.WriteLine("=========Chckpoint : {0}", deleted_csb_list[i].CSB_Checkpoint);

                                        if (restore_file(fs, deleted_csb_list[i], to_search))
                                        {
                                            find = true;
                                            break;
                                        }


                                    }
                                }
                            }
                            else if (input.Equals("5"))
                            {
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Input");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                }

            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Cannot Access dmg file");
                return;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Cannot Find File...");
            }


        }


        public static bool restore_file(FileStream fs, CSB csb, string search_name)
        {
            RECORD.parent_node_dic.Clear();
            RECORD.parent_node_dic = new Dictionary<UInt64, List<UInt64>>();
            RECORD.ffr_dict.Clear();
            RECORD.ffr_dict = new Dictionary<UInt64, FileFolderRecord>();
            RECORD.er_dic.Clear();
            RECORD.er_dic = new Dictionary<UInt64, ExtentRecord>();

            ////volume structure
            UInt64 VRB_addr = csb.VolumesIndexblock;
            UInt64 VB_addr = Table.get_block_address(fs, VRB_addr, "0x30");


            Table VB_h;
            VB_h = Table.get_table_header(fs, VB_addr);
            if (VB_h == null)
            {
                throw new Exception("This csb is invalid. Cannot restore file");

            }

            //VB record
            Console.WriteLine();
            TableType[] VBrecords = Table.save_record(fs, VB_addr, VB_h);

            //VCSB
            for (int i = 0; i < VB_h.record_num; i++)
            {
                Console.WriteLine();
                UInt64 VCSB_addr;
                VCSB_addr = VBrecords[i].BlockNum;

                //각 VCSB
                Console.WriteLine();
                VCSB vcsb = VCSB.init_vcsb(fs, VCSB_addr);

                //BTCS
                Console.WriteLine();
                UInt64 BTCS_addr = vcsb.BTCS;
                UInt64 BTOM_addr = Table.get_block_address(fs, BTCS_addr, "0x30");


                int n = 0;

                List<BTCS> btrn_btln = BTCS.init_btom(fs, BTOM_addr);

                foreach (BTCS b in btrn_btln)
                {
                    n++;

                    if (n > 1)
                    {

                        RECORD.init_btln(fs, b.BlockNum);
                    }

                }


                // Queue 만들기
                Queue<UInt64> q = new Queue<UInt64>();

                q.Enqueue(2);   // NodeID of root node

                ulong root = 2;

                if (!RECORD.parent_node_dic.ContainsKey(root)) return false;
                RECORD.ffr_dict[root].path = @restore_to_path;



                while (q.Count > 0)
                {
                    UInt64 parent_node_id = q.Dequeue();
                    String parent_path = RECORD.ffr_dict[parent_node_id].path;

                    Console.WriteLine("ParentID = {0}", parent_node_id);
                    if (!RECORD.parent_node_dic.ContainsKey(parent_node_id)) continue;
                    foreach (UInt64 child_node_Id in RECORD.parent_node_dic[parent_node_id])
                    {
                        String fname = new string(RECORD.ffr_dict[child_node_Id].FileName, 0, RECORD.ffr_dict[child_node_Id].FileName.Length - 1);
                        //String new_path = Path.Combine(parent_path, fname);
                        RECORD.ffr_dict[child_node_Id].path = parent_path;

                        Console.WriteLine("     NodeID = {0}, {1}", child_node_Id, fname);

                        if (RECORD.ffr_dict[child_node_Id].Flag[0] == '8' && fname.Equals(search_name))
                        {
                            String new_path = Path.Combine(parent_path, fname);
                            if (RECORD.er_dic.ContainsKey(child_node_Id))
                            {
                                Extent new_extent = Extent.read_extent(fs, (long)Utility.get_address(RECORD.er_dic[child_node_Id].ExtentStartBlockNum), (long)RECORD.er_dic[child_node_Id].ExtentLength);
                                Extent.write_extent(child_node_Id, new_extent.buf, new_extent.Count, new_path);
                            }
                            else
                            {
                                if (!File.Exists(new_path))
                                {
                                    File.Create(new_path);
                                }
                            }
                            Console.WriteLine("Restore File");
                            return true;

                        }
                        else if (RECORD.ffr_dict[child_node_Id].Flag[0] == '4')
                        {
                            q.Enqueue(child_node_Id);

                        }



                    }
                }

                Console.WriteLine();


            }
            Console.WriteLine("FIN");

            return false;
        }

        public static bool restore_folder(FileStream fs, CSB csb, string search_name)
        {
            RECORD.parent_node_dic.Clear();
            RECORD.parent_node_dic = new Dictionary<UInt64, List<UInt64>>();
            RECORD.ffr_dict.Clear();
            RECORD.ffr_dict = new Dictionary<UInt64, FileFolderRecord>();
            RECORD.er_dic.Clear();
            RECORD.er_dic = new Dictionary<UInt64, ExtentRecord>();

            bool restore = false;
            ////volume structure
            UInt64 VRB_addr = csb.VolumesIndexblock;
            UInt64 VB_addr = Table.get_block_address(fs, VRB_addr, "0x30");

            Table VB_h = Table.get_table_header(fs, VB_addr);
            if (VB_h == null)
            {
                throw new Exception("This csb is invalid.\n Cannot restore folder");

            }



            //VB record
            Console.WriteLine();
            TableType[] VBrecords = Table.save_record(fs, VB_addr, VB_h);

            //VCSB
            for (int i = 0; i < VB_h.record_num; i++)
            {
                Console.WriteLine();
                UInt64 VCSB_addr;
                VCSB_addr = VBrecords[i].BlockNum;

                //각 VCSB
                Console.WriteLine();
                VCSB vcsb = VCSB.init_vcsb(fs, VCSB_addr);

                //BTCS
                Console.WriteLine();
                UInt64 BTCS_addr = vcsb.BTCS;
                UInt64 BTOM_addr = Table.get_block_address(fs, BTCS_addr, "0x30");


                int n = 0;

                List<BTCS> btrn_btln = BTCS.init_btom(fs, BTOM_addr);

                foreach (BTCS b in btrn_btln)
                {
                    n++;

                    if (n > 1)
                    {

                        RECORD.init_btln(fs, b.BlockNum);
                    }

                }


                // Queue 만들기
                Queue<UInt64> q = new Queue<UInt64>();

                q.Enqueue(2);   // NodeID of root node

                ulong root = 2;

                if (!RECORD.parent_node_dic.ContainsKey(root)) return false;
                RECORD.ffr_dict[root].path = @restore_to_path;

                restore = false;

                while (q.Count > 0)
                {
                    UInt64 parent_node_id = q.Dequeue();
                    String parent_path = RECORD.ffr_dict[parent_node_id].path;

                    Console.WriteLine("ParentID = {0}", parent_node_id);
                    if (!RECORD.parent_node_dic.ContainsKey(parent_node_id)) continue;
                    foreach (UInt64 child_node_Id in RECORD.parent_node_dic[parent_node_id])
                    {
                        String fname = new string(RECORD.ffr_dict[child_node_Id].FileName, 0, RECORD.ffr_dict[child_node_Id].FileName.Length - 1);
                        if (restore)
                            RECORD.ffr_dict[child_node_Id].path = Path.Combine(parent_path, fname);
                        else RECORD.ffr_dict[child_node_Id].path = parent_path;

                        Console.WriteLine("     NodeID = {0}, {1}", child_node_Id, fname);
                        String new_path = RECORD.ffr_dict[child_node_Id].path;
                        if (restore && RECORD.ffr_dict[child_node_Id].Flag[0] == '8')
                        {

                            if (RECORD.er_dic.ContainsKey(child_node_Id))
                            {
                                Extent new_extent = Extent.read_extent(fs, (long)Utility.get_address(RECORD.er_dic[child_node_Id].ExtentStartBlockNum), (long)RECORD.er_dic[child_node_Id].ExtentLength);
                                Extent.write_extent(child_node_Id, new_extent.buf, new_extent.Count, new_path);
                            }
                            else if (!File.Exists(new_path))
                            {
                                File.Create(new_path);

                            }
                        }
                        if (RECORD.ffr_dict[child_node_Id].Flag[0] == '4')
                        {
                            if (!restore)
                            {
                                if (fname.Equals(search_name))
                                {
                                    Console.WriteLine("---------Find Folder-------");
                                    RECORD.ffr_dict[child_node_Id].path = Path.Combine(RECORD.ffr_dict[child_node_Id].path, fname);
                                    Directory.CreateDirectory(RECORD.ffr_dict[child_node_Id].path);
                                    q.Clear();
                                    q.Enqueue(child_node_Id);
                                    restore = true;
                                    break;
                                }
                                else
                                {
                                    q.Enqueue(child_node_Id);
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(new_path);
                                q.Enqueue(child_node_Id);
                            }

                        }
                    }

                }

                Console.WriteLine();


            }
            Console.WriteLine("FIN");

            return restore;
        }



        public static bool init_APFS(FileStream fs, CSB csb)
        {
            Console.WriteLine("*************** Make File Structure at Checkpoint : {0} *************", csb.CSB_Checkpoint);
            RECORD.parent_node_dic.Clear();
            RECORD.parent_node_dic = new Dictionary<UInt64, List<UInt64>>();
            RECORD.ffr_dict.Clear();
            RECORD.ffr_dict = new Dictionary<UInt64, FileFolderRecord>();
            RECORD.er_dic.Clear();
            RECORD.er_dic = new Dictionary<UInt64, ExtentRecord>();

            ////volume structure
            UInt64 VRB_addr = csb.VolumesIndexblock;
            Console.WriteLine();
            Console.WriteLine("VRB address : {0} {1}", VRB_addr, Utility.get_address(VRB_addr));

            UInt64 VB_addr = Table.get_block_address(fs, VRB_addr, "0x30");
            Console.WriteLine("VB address : {0}", VB_addr);

            Console.WriteLine("first csb address : {0}, blocksize : {1}, totalLength : {2}", CSB.first_csb_address, CSB.BlockSize, CSB.TotalSize);

            Table VB_h;
            VB_h = Table.get_table_header(fs, VB_addr);
            if (VB_h == null)
            {
                throw new Exception("This csb is invalid.");

            }

            //VB record
            Console.WriteLine();
            TableType[] VBrecords = Table.save_record(fs, VB_addr, VB_h);

            //VCSB
            for (int i = 0; i < VB_h.record_num; i++)
            {
                Console.WriteLine();
                UInt64 VCSB_addr;
                VCSB_addr = VBrecords[i].BlockNum;
                Console.WriteLine("VCSB_addr : {0}", VCSB_addr);

                //각 VCSB
                Console.WriteLine();
                VCSB vcsb = VCSB.init_vcsb(fs, VCSB_addr);

                //BTCS
                Console.WriteLine();
                UInt64 BTCS_addr = vcsb.BTCS;
                Console.WriteLine("BTCS address : {0}", BTCS_addr);

                UInt64 BTOM_addr = Table.get_block_address(fs, BTCS_addr, "0x30");
                Console.WriteLine("BTOM address : {0}", BTOM_addr);


                int n = 0;

                List<BTCS> btrn_btln = BTCS.init_btom(fs, BTOM_addr);

                foreach (BTCS b in btrn_btln)
                {
                    n++;
                    Console.WriteLine("btrn_btln : {0} ", n);
                    Console.WriteLine("node id : {0}", b.NodeID);
                    Console.WriteLine("Checkpoint : {0}", b.Checkpoint);
                    Console.WriteLine("block num: {0}, {1}\n", b.BlockNum, Utility.get_address(b.BlockNum));

                    if (n > 1)
                    {

                        RECORD.init_btln(fs, b.BlockNum);
                    }

                }


                // Queue 만들기
                Queue<UInt64> q = new Queue<UInt64>();

                q.Enqueue(2);   // NodeID of root node

                ulong root = 2;

                if (!RECORD.parent_node_dic.ContainsKey(root)) return true;
                RECORD.ffr_dict[root].path = @restore_to_path;




                while (q.Count > 0)
                {
                    UInt64 parent_node_id = q.Dequeue();
                    String parent_path = RECORD.ffr_dict[parent_node_id].path;

                    Console.WriteLine("ParentID = {0}", parent_node_id);
                    if (!RECORD.parent_node_dic.ContainsKey(parent_node_id)) continue;
                    foreach (UInt64 child_node_Id in RECORD.parent_node_dic[parent_node_id])
                    {
                        String fname = new string(RECORD.ffr_dict[child_node_Id].FileName, 0, RECORD.ffr_dict[child_node_Id].FileName.Length - 1);
                        String new_path = Path.Combine(parent_path, fname);
                        RECORD.ffr_dict[child_node_Id].path = new_path;


                        Console.WriteLine("     NodeID = {0}, path : {1}", child_node_Id, new_path);

                        if (RECORD.ffr_dict[child_node_Id].Flag[0] == '8')
                        {
                            if (RECORD.er_dic.ContainsKey(child_node_Id))
                            {
                                Extent new_extent = Extent.read_extent(fs, (long)Utility.get_address(RECORD.er_dic[child_node_Id].ExtentStartBlockNum), (long)RECORD.er_dic[child_node_Id].ExtentLength);
                                Extent.write_extent(child_node_Id, new_extent.buf, new_extent.Count, new_path);
                            }
                            else
                            {
                                if (!File.Exists(new_path))
                                {
                                    File.Create(new_path);
                                }
                            }

                        }
                        else if (RECORD.ffr_dict[child_node_Id].Flag[0] == '4')
                        {
                            q.Enqueue(child_node_Id);

                            Directory.CreateDirectory(new_path);
                        }




                    }
                }

                Console.WriteLine();


            }
            Console.WriteLine("FIN");
            return true;

        }

    }
}