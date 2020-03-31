using System.Diagnostics;
using System.IO;
using System;

namespace APFS
{
    public class APFS
    {
        private Node _rootNode;

        public bool IsValid;

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
