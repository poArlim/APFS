using System;
using System.Collections.Generic;

namespace APFS
{
    public enum NodeType
    {
        File,
        Directory
    }

    public class Node
    {
        // 파일인지 디렉토리인지
        public NodeType Type;

        // 이름
        public string Name;

        // 데이터의 크기
        public long Size;

        // 타임스탬프. 각 파일 시스템마다 crtime이나 ctime이 없을 수 있음
        public DateTime CreateTime;
        public DateTime ChangeTime;
        public DateTime ModifyTime;
        public DateTime AccessTime;
        //ChangeTime 이랑 ModifyTime 은 무슨차이일까?

        // 데이터
        public NodeStream Data;
        
        // 자식 노드들(Directory의 경우)
        public List<Node> Children;
    }
}