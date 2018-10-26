using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StationTools
{
    class StationData
    {

        protected GameDataParser gameDataParser;

        public Dictionary<string, string> names;
        public Dictionary<string, List<Recipe>> recipes;
        public GameVersion version;


        //private string gameDirectory;

        public StationData(GameDataParser gameDataParser) {
            this.gameDataParser = gameDataParser;
        }
        public void LoadAll() {
            LoadVersion();
            LoadNames();
            LoadRecipes();
        }

        public void LoadNames()
        {
            names = gameDataParser.GetLocalisedNames("english");
        }
        public void LoadRecipes()
        {
            recipes = gameDataParser.ParseAllRecipes();
        }

        public void LoadVersion()
        {
            version = gameDataParser.GetVersion();
        }


        protected string GetTranslatedName(string prefabName)
        {
            return names.GetValueOrDefault(prefabName, prefabName);
        }

        public List<KeyValuePair<String, Recipe>> FindItemRecipes(string itemName) {
            List<KeyValuePair<String, Recipe>> result = new List<KeyValuePair<String, Recipe>>();

            foreach (var machine in recipes)
            {
                foreach (Recipe recipe in machine.Value)
                {
                    if (recipe.prefabName == itemName) {
                        result.Add(new KeyValuePair<string, Recipe>(machine.Key, recipe));
                    }
                }
            }
            return result;
        }
    }
}