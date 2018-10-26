using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace StationTools
{
    class GameDataParser
    {

        private FileUtils fileUtils;

        public GameDataParser(FileUtils fileUtils) {
            this.fileUtils = fileUtils;
        }

        public Dictionary<string, Recipe> FindAllRecipes(string prefabName)
        {

            var allRecipes = ParseAllRecipes();
            var res = new Dictionary<string, Recipe>();
            foreach (var machine in allRecipes)
            {
                var rec = machine.Value.Find(r => r.prefabName == prefabName);
                if (rec != null) res.Add(machine.Key, rec);
            }
            return res;
        }

        public Dictionary<string, List<Recipe>> ParseAllRecipes()
        {
            var files = fileUtils.RecipeFiles();
            var res = new Dictionary<string, List<Recipe>>();

            foreach (string filename in files)
            {
                var (name, parsed) = ParseRecipes(filename);
                if (parsed == null) continue;
                if (res.ContainsKey(name))
                {
                    res[name].AddRange(parsed);
                }
                else
                {
                    res.Add(name, parsed);
                }
            }
            return res;
        }

        public (string, List<Recipe>) ParseRecipes(string filename)
        {
            XDocument xdoc = XDocument.Load(filename);
            var recipes = xdoc.Element("GameData").Elements().First();
            if (recipes.Element("RecipeData") == null) return (null, null);

            var res = (from rec in recipes.Descendants("RecipeData")
                       select new Recipe
                       {
                           prefabName = rec.Element("PrefabName").Value,
                           recipe = (from item in rec.Element("Recipe").Elements()
                                     select new RecipeItem
                                     {
                                         name = item.Name.ToString(),
                                         value = item.HasElements ? 0 : float.Parse(item.Value, CultureInfo.InvariantCulture),
                                         start = item.Element("Start") != null ? float.Parse(item.Element("Start").Value, CultureInfo.InvariantCulture) : 0,
                                         stop = item.Element("Stop") != null ? float.Parse(item.Element("Stop").Value, CultureInfo.InvariantCulture) : 0
                                     }).ToList<RecipeItem>()
                       }).ToList<Recipe>();

            return (recipes.Name.LocalName, res);
        }

        public Dictionary<string, string> GetLocalisedNames(string language)
        {
            XDocument xdoc = XDocument.Load(fileUtils.LanguageRoot() + "\\" + language + ".xml");
            var all = xdoc.Element("Language");
            if (all.Element("Things") == null) return null;

            return (from thing in all.Element("Things").Descendants("Record")
                    select new KeyValuePair<string, string>(
                    thing.Element("Key").Value,
                    thing.Element("Value").Value
                    )).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public GameVersion GetVersion() {
            var v = new GameVersion();
            using (StreamReader reader = new StreamReader(fileUtils.VersionPath()))
            {
                string line = reader.ReadLine();
                v.updateVersion = line.Split("=", 2)[1];
                line = reader.ReadLine();
                v.updateDate = line.Split("=", 2)[1];
            }
            return v;
        }
    }
}
