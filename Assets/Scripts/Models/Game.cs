using System;

namespace Models
{
    public class Game
    {
        private string productId;
        private string version;
        private string name;
        private string areaCode;
        private int totalNumberOfSaves;

        public string ProductId { get => productId; set => productId = value; }
        public string Version { get => version; set => version = value; }
        public string Name { get => name; set => name = value; }
        public string AreaCode { get => areaCode; set => areaCode = value; }
        public int TotalNumberOfSaves { get => totalNumberOfSaves; set => totalNumberOfSaves = value; }

        public override string ToString()
        {
            return string.Format("[{0} {1}] - {2} ({3}) - {4}", ProductId, Version, Name, AreaCode, TotalNumberOfSaves);
        }
    }
}