using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace APFS
{
    public class APFS
    {
        private Node _rootNode;

        public bool IsValid;
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

                        if (input.Equals("1"))
                        {
                            try
                            {
                                init_APFS(fs, msb);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Error at init_APRS : {0}", e);
                                return; 
                            }
                            
                        }
                        else if (input.Equals("2"))
                        {
                            init_APFS(fs, csb_list[csb_list.Count / 2]);
                        }
                        else if (input.Equals("3"))
                        {
                            bool find = false;
                            String to_search = "";
                            Console.WriteLine("Enter the name of the Folder you want to restore : ");
                            to_search = Console.ReadLine();
                            foreach (CSB c in csb_list)
                            {
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
                                    try
                                    {
                                        if (restore_folder(fs, deleted_csb_list[i], to_search))
                                        {
                                            find = true;
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Cannot Find Folder...");
                                        break;
                                    }

                                }
                            }
                            if (!find)
                            {
                                Console.WriteLine("Cannot Find Folder...");
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
                                    try
                                    {
                                        if (restore_file(fs, deleted_csb_list[i], to_search))
                                        {
                                            find = true;
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Cannot Find file...");
                                        break;
                                    }

                                }
                            }
                            if (!find)
                            {
                                Console.WriteLine("Cannot Find file...");
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

                }

            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Cannot Access dmg file");
                return; 
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

                        Console.WriteLine("     NodeID = {0}, {1}", child_node_Id , fname);

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
                            Console.WriteLine("make file!!!!!!!!!!!!!"); 
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
                        if(restore)
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
            Console.WriteLine("***************Make File Structure at Checkpoint : {0}*************", csb.CSB_Checkpoint); 
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

        public APFS(Stream stream, long startAddress = 0)
        {
            IsValid = initApfs(stream);

            if (IsValid)
                BuildFileSystem(stream);
        }

        // APFS의 SuperBlock과 기타 중요한 메타데이터를 분석하고, APFS로 분석 가능한 데이터인지 확인하는 함수
        private bool initApfs(Stream stream)
        {
            // SuperBlock 분석하여 root node의 inode 번호를 알아낸 뒤, 해당 값으로 root node를 만들어 내야 한다.
            var rootINodeNum = 0;
            _rootNode = makeRootNode(rootINodeNum);
            return true;
        }

        // 파일시스템의 모든 Node들을 만들어내는 함수
        public void BuildFileSystem(Stream stream)
        {
            expandAll(_rootNode);
        }

        // 가장 최상위 디렉토리 "/"를 노드로 만들어내는 함수
        private Node makeRootNode(long iNodeNum)
        {
            return makeNode(getINode(iNodeNum));
        }

        // Directory entry로부터 실제 Node를 만들어내는 함수
        private Node makeNode(iNode iNode)
        {
            var node = new Node
            {
                Name = iNode.Name,
                Size = iNode.Size,
                Data = iNode.Data,
                ChangeTime = iNode.ChangeTime,
                CreateTime = iNode.CreateTime,
                AccessTime = iNode.AccessTime,
                ModifyTime = iNode.ModifyTime
            };

            return node;
        }

        // iNode 번호를 이용하여 실제 iNode를 만들어내는 함수
        private iNode getINode(long iNodeNum)
        {
            iNode iNode = null;
            // APFS에서 iNode의 번호를 이용해서 iNode를 만들어내는 로직 들어가야함
            return iNode;
        }

        // 재귀적으로 모든 Node들을 Expand하는 함수
        private bool expandAll(Node node)
        {
            if (node == null)
                return true;

            node = expand(node);

            if (node == null)
                return true;

            foreach (var child in node.Children)
            {
                // Directory인 경우에만 Expand하면 된다.
                if (child.Type == NodeType.Directory)
                    if (!expandAll(child))
                        return false;
            }

            return true;
        }

        // 현재 Node 하위의 Children을 만들어내는 함수
        private Node expand(Node from)
        {
            // 현재 Node의 데이터를 byte배열로 읽어들여서 DirEntryBlock으로 해석한다.
            var buffer = new byte[from.Data.Length];
            from.Data.Read(buffer, 0, buffer.Length);

            var node = new DirEntryBlock(buffer);

            // 해석된 DirEntry들을 통해서 자식 노드들을 만들어 Children으로 붙여주는 부분
            foreach (var dirEntry in node.DirEntries)
            {
                from.Children.Add(makeNode(getINode(dirEntry.iNodeNum)));
            }

            return from;
        }
    }
}