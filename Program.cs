using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using StationTools.Wiki;

namespace StationTools
{
    class Program
    {

        static void printUsage() {
            Console.WriteLine("Usage: <name> [command] [options]");
            Console.WriteLine("Commands:");
            Console.WriteLine("  wiki furnace|recipes");
            Console.WriteLine("  hash [propName]");
            Console.WriteLine("  recipe [propName]");
            Console.WriteLine("  version");
        }

        static void printItemRecipes(string itemName) {
            var data = new StationData(new GameDataParser(new FileUtils()));
            data.LoadAll();
            List<KeyValuePair<String, Recipe>> recipes = data.FindItemRecipes(itemName);
            foreach (var recipe in recipes)
            {
                string reagents = "";
                foreach (var item in recipe.Value.recipe)
                {
                    if (item.value != 0 || item.start != 0) {
                        reagents += String.Format("{0} x{1} ", item.name, item.value);
                    }
                }
                Console.WriteLine("{0} - {1}", recipe.Key, reagents);
            }
        }

        static void handleWiki(ArraySegment<string> args) {
            switch (args[0])
            {
                case "furnace":
                    var furnaceRecipes = new FurnaceRecipes(new GameDataParser(new FileUtils()));
                    furnaceRecipes.PrintTable();
                    break;
                case "recipes":
                    var genericRecipes = new GenericRecipes(new GameDataParser(new FileUtils()));
                    genericRecipes.PrintAll();
                    break;
                default:
                    printUsage();
                    break;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                printUsage();
                return;
            }
            var cmd = args[0];
            switch (cmd)
            {
                case "wiki":
                    if (args.Length < 2)
                    {
                        printUsage();
                        return;
                    }
                    handleWiki(new ArraySegment<string>( args, 1, args.Length -1));
                    break;
                case "hash":
                    Console.WriteLine(ItemHash.Compute(args[1]));
                    break;
                case "recipe":
                    printItemRecipes(args[1]);
                    break;
                case "version":
                    var data = new StationData(new GameDataParser(new FileUtils()));
                    data.LoadVersion();
                    Console.WriteLine("Stationeers {0} - {1}", data.version.updateVersion, data.version.updateDate);
                    break;
                default:
                    printUsage();
                    break;

            }
        }
    }
}
