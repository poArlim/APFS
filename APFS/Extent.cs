using System.IO;

namespace APFS
{
    public class Extent
    {
        // Stream에서 특정 Offset부터 Count만큼 한 Extent로 묶어 냄
        public long Offset { get; set; }

        public long Count { get; set; }

        protected Stream Stream;
    }
}
