namespace Models
{
    public class Language
    {
        private string key = "en-us";
        private string chooseSaturnRamFile = "Choose Saturn RAM file";
        private string chooseSarooFile = "Choose SAROO save file";
        private string doNotImportBackupRam = "Do not import a Backup RAM file here!";
        private string doNotImportSarooFile = "Do not import a SAROO file here!";
        private string doNotImportSaveFile = "Do not import a single save file here!";
        private string saveLocation = "Save location";

        public string Key { get => key; set => key = value; }
        public string ChooseSaturnRamFile { get => chooseSaturnRamFile; set => chooseSaturnRamFile = value; }
        public string ChooseSarooFile { get => chooseSarooFile; set => chooseSarooFile = value; }
        public string DoNotImportBackupRam { get => doNotImportBackupRam; set => doNotImportBackupRam = value; }
        public string DoNotImportSarooFile { get => doNotImportSarooFile; set => doNotImportSarooFile = value; }
        public string DoNotImportSaveFile { get => doNotImportSaveFile; set => doNotImportSaveFile = value; }
        public string SaveLocation { get => saveLocation; set => saveLocation = value; }

    }
}