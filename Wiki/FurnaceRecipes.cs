using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StationTools.Wiki
{
    class FurnaceRecipes : StationData
    {
        private static string furnaceTableStart = String.Join(Environment.NewLine,
            "{| class=\"wikitable sortable\"",
            "! colspan=\"1\" rowspan=\"2\" |'''Ingot'''",
            "! colspan=\"1\" rowspan=\"2\" class=\"unsortable\" |'''Input'''",
            "! colspan=\"2\" rowspan=\"1\" class=\"unsortable\" |'''Pressure (Pa)'''",
            "! colspan=\"2\" rowspan=\"1\" class=\"unsortable\" |'''Temperature (K)'''",
            "! colspan=\"2\" rowspan=\"1\" class=\"unsortable\" |'''Temperature (&#176;C)'''",
            "! colspan=\"1\" rowspan=\"2\" class=\"unsortable\" |'''Notes'''",
            "|-",
            "! '''Min'''",
            "! '''Max'''",
            "! '''Min'''",
            "! '''Max'''",
            "! '''Min'''",
            "! '''Max'''",
            "|-"
        );

        private static string wikiTableEnd = "|}";

        public FurnaceRecipes(GameDataParser gameDataParser) : base(gameDataParser)
        {
            LoadAll();
        }

        string GetReagentItem(string name) {
            return Utils.OreGot(name, true);
        }


        string GetItemIcon(string name, float value, bool inputIngots) {
            return String.Format("<div class=\"stationeers-icon\">[[File:{0}.png|link={1}]] <div class=\"stationeers-icon-text\">{2}</div></div>",
                                 name, GetTranslatedName(name), value);
        }

        string GetRecipeIcon(RecipeItem item) {
            string name = Utils.OreGot(item.name, true);
            string value = Utils.FormatNumber(item.value);

            var imgName = name;
            if (name == "Carbon" || name == "Hydrocarbon")
            {
                imgName = "ItemCoalOre";
            }

            return String.Format("<div class=\"stationeers-icon\">[[File:{0}.png|link={1}]] <div class=\"stationeers-icon-text\">{2}</div></div>",
                                 imgName, GetTranslatedName(name), value);
        }

        static void evenRatio(List<RecipeItem> items) {
            items = items.FindAll(i => i.value > 0);
            if (items.Count() == 2) {
                var sorted = items.OrderBy(p => p.value > 0 ? p.value : float.MaxValue);
                var low = sorted.First();
                var high = sorted.Last();
                high.value = high.value/low.value;
                low.value = 1;
            }
        }



        string StripIngotName(string name) {
            if (name.StartsWith("Ingot (")) {
                name = name.Substring("Ingot (".Length);
                return name.Substring(0, name.Length -1);
            }
            return name;
        }

        string GetFurnaceRecipeItems(List<RecipeItem> items)
        {
            evenRatio(items);
            return String.Join("<br>", items.FindAll(i => (i.value > 0 || (i.start != 0 || i.stop != 0))).FindAll(i => (i.name != "Temperature" && i.name != "Pressure") ).Select(item =>
            {
                return String.Format("{0} [[{1}|{2}]]", GetRecipeIcon(item), GetTranslatedName(GetReagentItem(item.name)), item.name);
            }));
        }

        private string WrapOriginal(string value, string original) {
            return String.Format("<span title=\"{0}\">{1}</span>", original, value);
        }

        void WikiTableRow(Recipe recipe, bool inputIngots) {
            RecipeItem p = recipe.recipe.Find(i => i.name == "Pressure");
            RecipeItem t = recipe.recipe.Find(i => i.name == "Temperature");

            if (t == null || t == null)
            {
                return;
            }
            String.Format(new CultureInfo("en-US"), "{0:0.##} {1}", 10, "as");
            string pstart = p != null ? WrapOriginal(Utils.MetricPrefix(p.start, "k") + "Pa", p.start + " kPa") : "";
            string pstop = p != null ? WrapOriginal(Utils.MetricPrefix(p.stop, "k") + "Pa", p.stop + " kPa") : "";
            string tstart = p != null ? t.start + "K" : "";
            string tstop = p != null ? t.stop + "K" : "";
            // or use &#176;
            string tstartC = p != null ? Math.Round(Utils.KelvinToCelsius(t.start)) + "&#176;C" : "";
            string tstopC = p != null ? Math.Round(Utils.KelvinToCelsius(t.stop)) + "&#176;C" : "";


            Console.WriteLine(String.Format(
                String.Join(Environment.NewLine,
                    "|-",
                    "! {0}'''[[{1}|{2}]]'''",
                    "| {3}",
                    "| style=\"text-align:center;\"| {4}",
                    "| style=\"text-align:center;\"| {5}",
                    "| style=\"text-align:center;\"| {6}",
                    "| style=\"text-align:center;\"| {7}",
                    "| style=\"text-align:center;\"| {8}",
                    "| style=\"text-align:center;\"| {9}",
                    "| {{{{{{{10}Notes|}}}}}}"
                ),
                GetItemIcon(recipe.prefabName, 1.0f, inputIngots),
                GetTranslatedName(recipe.prefabName),
                StripIngotName(GetTranslatedName(recipe.prefabName)),
                GetFurnaceRecipeItems(recipe.recipe),
                pstart,
                pstop,
                tstart,
                tstop,
                tstartC,
                tstopC,
                recipe.prefabName.Substring("Item".Length)
            ));
        }

        public void PrintTable()
        {
            Console.WriteLine(furnaceTableStart);
            var recipes = base.recipes["FurnaceRecipes"];
            foreach (Recipe recipe in recipes)
            {
                WikiTableRow(recipe, false);
            }
            Console.WriteLine("|-");
            Console.WriteLine("! colspan=\"9\" | {0} - {1} <span class=\"right\">[[Furnace/Recipes|/Recipes]]</span>", version.updateVersion, version.updateDate);
            Console.WriteLine(wikiTableEnd);

        }
    }
}