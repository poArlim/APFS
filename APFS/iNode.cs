using System;

namespace APFS
{
    public class iNode
    {
        public iNode(byte[] buffer)
        {
            // iNode에 해당하는 데이터에서 파일 혹은 폴더의 여러 정보들과 데이터 값을 분석해서 값을 넣는다.
            makeNodeStream(buffer);
        }

        public string Name;
        public long Size;
        public DateTime CreateTime;
        public DateTime ChangeTime;
        public DateTime ModifyTime;
        public DateTime AccessTime;
        public NodeStream Data;

        private void makeNodeStream(byte[] buffer)
        {
            // iNode의 데이터를 이용하여 실제 데이터를 NodeStream으로 표현한다.
            Data = new NodeStream();
        }
    }
}
