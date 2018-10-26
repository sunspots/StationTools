using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StationTools.Wiki
{
    class GenericRecipes : StationData
    {
        class Creator
        {
            // The machine takes in ingots for processing rather than ores
            public bool inputIngots;
            public string prefabName;
            public Creator(bool inputIngots, string prefabName)
            {
                this.inputIngots = inputIngots;
                this.prefabName = prefabName;
            }
        }
        private static Dictionary<string, Creator> creators = new Dictionary<string, Creator> {
            { "ArcFurnaceRecipes", new Creator(false, "StructureArcFurnace") },
            { "AutolatheRecipes", new Creator(true, "StructureAutolathe") },
            { "CentrifugeRecipes", new Creator(false, "StructureCentrifuge") },
            { "ChemistryRecipes", new Creator(false, "ApplianceChemistryStation") },
            { "MicrowaveRecipes", new Creator(false, "ApplianceMicrowave") },
            { "HydraulicPipeBenderRecipes", new Creator(true, "StructureHydraulicPipeBender") },
            { "ElectronicsPrinterRecipes", new Creator(true, "StructureElectronicsPrinter") },
            { "FabricatorRecipes", new Creator(true, "StructureFabricator") },
            { "IngotRecipes", new Creator(false, "Ingot") },
            { "OrganicsPrinterRecipes", new Creator(false, "StructureOrganicsPrinter") },
            { "ToolManufactoryRecipes", new Creator(true, "StructureToolManufactory") },
            { "SecurityPrinterRecipes", new Creator(true, "StructureSecurityPrinter") },
            { "PaintMixRecipes", new Creator(true, "StructureSecurityPrinter") },

        };

        private static List<string> ingots = new List<string> {
            "Iron", "Gold", "Copper", "Silver", "Lead", "Nickel", "Silicon",
            "Steel", "Electrum", "Invar", "Constantan", "Solder"
        };
        private static List<string> ores = new List<string> {
            "Iron", "Gold", "Copper", "Silver", "Lead", "Nickel", "Silicon", "Uranium"
        };
        private static string wikiTableStart = String.Join(Environment.NewLine,
            "{| class=\"wikitable sortable\"",
            "! '''Item'''",
            "! class=\"unsortable\" |'''Input'''");

        private static string wikiTableEnd = "|}";
        private static Dictionary<string, string> machines = new Dictionary<string, string>
        {
            ["autolathe"] = "Autolathe",
            ["fabricator"] = "Fabricator",
            ["PipeBender"] = "Hydraulic Pipe Bender",
            ["recycling"] = "Recycler"
        };

        public GenericRecipes(GameDataParser gameDataParser) : base(gameDataParser)
        {
            LoadAll();
        }



        private string GetIcon(RecipeItem item, bool inputIngots) {
            String name = Utils.OreGot(item.name, !inputIngots);
            bool range = item.start != 0 || item.stop != 0;
            string value;
            if (item.value == 0) {
                value = String.Format("{0}-{1}", Utils.FormatNumber(item.start), Utils.FormatNumber(item.stop));
            } else {
                value = Utils.FormatNumber(item.value);
            }
            var imgName = name;
            if (name == "Carbon" || name == "Hydrocarbon")
            {
                imgName = "ItemCoalOre";
            }

            String className = range ? "stationeers-icon icon-range" : "stationeers-icon";
            return String.Format("<div class=\"{0}\">[[File:{1}.png|link={2}]] <div class=\"stationeers-icon-text\">{3}</div></div>",
                                 className, imgName, GetTranslatedName(name), value);
        }

        private string GetSimpleIcon(String name, float value, bool inputIngots) {
            var imgName = name;
            if (name == "Carbon" || name == "Hydrocarbon")
            {
                imgName = "ItemCoalOre";
            }
            String formatValue = Utils.FormatNumber(value);
            return String.Format("<div class=\"stationeers-icon\">[[File:{0}.png|link={1}]] <div class=\"stationeers-icon-text\">{2}</div></div>",
                                 imgName, GetTranslatedName(name), formatValue);
        }

        private string GetWikiRecipeComponents(List<RecipeItem> items, bool inputIngots)
        {
            return String.Join(" ", items.FindAll(i => i.value > 0 || (i.start != 0 || i.stop != 0)).Select(item =>
            {
                return GetIcon(item, inputIngots);
            }));
        }


        private string GetWikiRecipeItems(List<RecipeItem> items, bool inputIngots)
        {
            return String.Join(" ", items.FindAll(i => (i.value > 0 || (i.start != 0 || i.stop != 0))).FindAll(i => (i.name != "Temperature" && i.name != "Pressure") ).Select(item =>
            {
                return GetIcon(item, inputIngots);
            }));
        }


        private void RecipeWikiTableRow(Recipe recipe, bool inputIngots)
        {
            Console.WriteLine(String.Format(
                String.Join(Environment.NewLine,
                    "|-",
                    "! {0}'''[[{1}]]'''",
                    "| {2}"
                ),
                GetSimpleIcon(recipe.prefabName, 1.0f, inputIngots),
                GetTranslatedName(recipe.prefabName),
                GetWikiRecipeComponents(recipe.recipe, inputIngots)
            ));
        }


        private void RecipeWikiListEntry(Recipe recipe)
        {
            Console.WriteLine("* " + recipe.prefabName + " - [[" + names.GetValueOrDefault(recipe.prefabName, recipe.prefabName + "??") + "]]");
        }

        public void PrintAll()
        {
            foreach (var machine in recipes)
            {
                if (machine.Key == "FurnaceRecipes") {
                    continue;
                }
                if (!creators.ContainsKey(machine.Key))
                {
                    Console.WriteLine(machine.Key);
                    return;
                }
                Creator creator = creators[machine.Key];
                Console.WriteLine(String.Format("===[[{0}]]===", GetTranslatedName(creator.prefabName)));
                PrintTable(machine.Key);
            }
        }

        public void PrintTable(string machine) {
            Creator creator = creators[machine];
            Console.WriteLine(wikiTableStart);
            foreach (var recipe in recipes[machine])
            {
                RecipeWikiTableRow(recipe, creator.inputIngots);
            }
            Console.WriteLine(wikiTableEnd);
        }


    }
}