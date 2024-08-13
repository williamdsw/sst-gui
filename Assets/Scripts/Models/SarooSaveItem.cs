using System;

namespace Models
{
    public class SarooSaveItem
    {
        private string rawHeader;
        private string productId;
        private string version;

        public string ProductId { get => productId; set => productId = value; }
        public string Version { get => version; set => version = value; }
        public string RawHeader => rawHeader;
        public string FormattedHeader => string.Format("[{0}]", RawHeader);

        public SarooSaveItem(string header)
        {
            rawHeader = header;
            string[] values = header.Split("V");
            productId = values[0].Trim();
            version = $"V{values[1].Trim()}";
        }
    }
}