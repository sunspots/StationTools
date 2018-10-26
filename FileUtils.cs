using System.IO;
using System.Collections.Generic;


namespace StationTools
{
    class FileUtils
    {
        private string gameRoot;

        private Dictionary<string, string> conf = new Dictionary<string, string>();

        private void ReadConf()
        {
            using (StreamReader reader = new StreamReader("conf.ini"))
            {
                string line = reader.ReadLine();
                var l = line.Split("=", 2);
                conf.Add(l[0], l[1]);
            }
        }

        public FileUtils(string gameRoot = null)
        {
            ReadConf();
            if (gameRoot != null) {
                this.gameRoot = gameRoot;
            } else {
                FindRoot();
            }
        }

        private void FindRoot()
        {
            this.gameRoot = conf["gameRoot"];
        }

        public string VersionPath()
        {
            return gameRoot + "\\rocketstation_Data\\StreamingAssets\\version.ini";
        }

        public string AssetDataRoot()
        {
            return gameRoot + "\\rocketstation_Data\\StreamingAssets\\Data";
        }
        public  string LanguageRoot()
        {
            return gameRoot + "\\rocketstation_Data\\StreamingAssets\\Language";
        }

        public string[] RecipeFiles()
        {
            var assetDataRoot = AssetDataRoot();
            if (Directory.Exists(assetDataRoot))
            {
                return Directory.GetFiles(assetDataRoot, "*.xml", SearchOption.AllDirectories);
            }
            return new string[] { };
        }
    }
}