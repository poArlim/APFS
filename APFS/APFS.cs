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



            static void Main()
        {


            using (FileStream fs = new FileStream(@"/Users/yang-yejin/Desktop/file_info/newhan.dmg", FileMode.Open))
            {


                CSB.TotalSize = (UInt64)fs.Length;
                CSB.BlockSize = 4096;

                List<CSB> csb_list = CSB.get_csb_list(fs);
                CSB msb = csb_list[csb_list.Count - 1];
                foreach (CSB c in csb_list)
                {
                    Console.WriteLine("Chckpoint : {0}", c.CSB_Checkpoint);
                }

                init_APFS(fs, msb);
                //이전 csb로 init_APFS하는 법 : csb_list index : 0 ~ csb_list.count-1중 아무 csb나 선택해서 init_APFS에 넣csb_list[csb_list.Count - 1]
                // init_APFS(fs, csb_list[0]); // 첫번째 checkpoint로 하는 예


            }

        }

        public static void init_APFS(FileStream fs, CSB csb)
            {
                CSB.TotalSize = (UInt64)fs.Length;
                CSB.BlockSize = 4096;

                //CSBD
                //CSBD csbd = CSBD.init_csbd(fs, 0);
                //csbd.records = new CSBD_recode[csbd.RecordNum];
                //UInt64 start_addr = csbd.CSBD_Address + 40; //40 = 0x28
                //for (UInt16 i = 0; i < csbd.RecordNum; i++)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine($"record[{i}]");
                //    csbd.records[i] = CSBD_recode.init_csbd_recode(fs, start_addr);
                //    start_addr += 40; // 40 = 0x28
                //}

                ////volume structure
                UInt64 VRB_addr = csb.VolumesIndexblock;
                Console.WriteLine();
                Console.WriteLine("VRB address : {0}", VRB_addr);

                UInt64 VB_addr = Table.get_block_address(fs, VRB_addr, "0x30");
                Console.WriteLine("VB address : {0}", VB_addr);



                Table VB_h;
                VB_h = Table.get_table_header(fs, VB_addr);
                //Console.WriteLine("VB check_point : {0}", VB_h.check_point);
                //Console.WriteLine("VB table_type : {0}", VB_h.table_type);
                //Console.WriteLine("VB record_num : {0}", VB_h.record_num);
                //Console.WriteLine("VB len_record_def : {0}", VB_h.len_record_def);
                //Console.WriteLine("VB len_key_section : {0}", VB_h.len_key_section);
                //Console.WriteLine("VB gap_key_data : {0}", VB_h.gap_key_data);

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


                //foreach (KeyValuePair<UInt64, List<UInt64>> kvp in RECORD.parent_node_dic)
                //{
                //    List<UInt64> l = kvp.Value;
                //    String name = "";
                //    try
                //    {
                //        name = new string(RECORD.ffr_dict[kvp.Key].FileName, 0, RECORD.ffr_dict[kvp.Key].FileName.Length - 1);
                //    }
                //    catch (Exception e)
                //    {
                //        name = "";
                //    }


                //    Console.WriteLine("====Key : {0}, {1}=======", kvp.Key, name);
                //    foreach (UInt64 nid in l)
                //    {
                //        String fname = new string(RECORD.ffr_dict[nid].FileName, 0, RECORD.ffr_dict[nid].FileName.Length - 1);
                //        Console.WriteLine("NodeID : {0}, {1}", nid, fname);
                //    }
                //}

                // Queue 만들기
                Queue<UInt64> q = new Queue<UInt64>();

                    q.Enqueue(2);   // NodeID of root node

                    ulong old_idx = 2;
                    RECORD.ffr_dict[old_idx].path = "/Users/yang-yejin/test";

                    while (q.Count > 0)
                    {
                        UInt64 parent_node_id = q.Dequeue();
                        old_idx = parent_node_id;
                        String parent_path = RECORD.ffr_dict[old_idx].path;

                        Console.WriteLine("ParentID = {0}", parent_node_id);
                        if (!RECORD.parent_node_dic.ContainsKey(parent_node_id)) continue;
                        foreach (UInt64 new_node_id in RECORD.parent_node_dic[parent_node_id])
                        {
                            ulong new_idx = new_node_id;
                            String new_name = new string(RECORD.ffr_dict[new_idx].FileName, 0, RECORD.ffr_dict[new_idx].FileName.Length - 1);
                            String new_path = Path.Combine(parent_path, new_name); 
                            RECORD.ffr_dict[new_idx].path = new_path;

                            if (RECORD.ffr_dict[new_idx].ParentID == RECORD.ffr_dict[old_idx].NodeID)
                            {
                                Console.WriteLine("     NodeID = {0}, path : {1}", new_node_id, new_path);

                                if (RECORD.ffr_dict[new_idx].Flag[0] == '8')
                                {
                                    if (RECORD.er_dic.ContainsKey(new_node_id))
                                    {
                                        Extent new_extent = Extent.read_extent(fs, (long)Utility.get_address(RECORD.er_dic[new_node_id].ExtentStartBlockNum), (long)RECORD.er_dic[new_node_id].ExtentLength);
                                        Extent.write_extent(new_node_id, new_extent.buf, new_extent.Count, new_path);
                                    }
                                    else
                                    {
                                    
                                    File.Create(new_path);
                                    }

                                }
                                else if (RECORD.ffr_dict[new_idx].Flag[0] == '4')
                                {
                                    q.Enqueue(new_node_id);
                                    
                                    Directory.CreateDirectory(new_path);
                                
                                    string octal = Utility.StringToOctal(RECORD.ffr_dict[new_idx].Flag);
                                    Utility.Exec("chmod " + octal.Substring(3) + " " + new_path);
          
                                }
                            }
              

                        }
                    }

                    Console.WriteLine();


                }
            Console.WriteLine("FIN");
            
          
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