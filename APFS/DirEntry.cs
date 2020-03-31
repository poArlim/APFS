using System.Collections.Generic;

namespace APFS
{
    // Node를 만들어내는 데에 필요한 정보들이 들어있음
    public class DirEntry
    {
        public string Name;
        public NodeType Type;
        public long iNodeNum;
    }

    // Directory Node의 데이터들은 자식 Node들의 정보를 담고 있음
    public class DirEntryBlock
    {
        public DirEntryBlock(byte[] buffer)
        {
            readDentries(buffer);
        }

        private void readDentries(byte[] buffer)
        {
            // Directory entry를 하나하나 만들어서 DirEntries에 담는다.
        }

        public List<DirEntry> DirEntries;
    }
}
