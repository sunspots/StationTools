using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StationTools
{
    class Utils
    {

        private static List<KeyValuePair<int, string>> units = new List<KeyValuePair<int, string>> {
            new KeyValuePair<int, string>(-12, "p"), // pico 	p 	0.000000000001
            new KeyValuePair<int, string>(-9, "n"),  // nano 	n 	0.000000001
            new KeyValuePair<int, string>(-6, "μ"),  // micro 	μ 	0.000001
            new KeyValuePair<int, string>(-3, "m"),  // milli 	m 	0.001
            new KeyValuePair<int, string>(-2, "c"),  // centi 	c 	0.01
            new KeyValuePair<int, string>(-1, "d"),  // deci 	d 	0.1
            new KeyValuePair<int, string>(0, ""),    //             1
            new KeyValuePair<int, string>(2, "h"),   // hecto 	h 	100
            new KeyValuePair<int, string>(3, "k"),   // kilo 	k 	1000
            new KeyValuePair<int, string>(6, "M"),   // mega 	M 	1000000
            new KeyValuePair<int, string>(9, "G"),   // giga 	G 	1000000000
            new KeyValuePair<int, string>(12, "T"),  // tera 	T 	1000000000000
            new KeyValuePair<int, string>(15, "P"),  // peta 	P 	1000000000000000
            new KeyValuePair<int, string>(18, "E"),  // exa 	E 	1000000000000000000
        };

        // TODO: Generate from files
        private static List<string> ingots = new List<string> {
            "Iron", "Gold", "Copper", "Silver", "Lead", "Nickel", "Silicon",
            "Steel", "Electrum", "Invar", "Constantan", "Solder"
        };

        // TODO: Generate from files
        private static List<string> ores = new List<string> {
            "Iron", "Gold", "Copper", "Silver", "Lead", "Nickel", "Silicon", "Uranium", "Coal"
        };


        public static double KelvinToCelsius(double val) {
            return val -273.15;
        }

        // Simple number formatter adding "K" for thousands
        public static string FormatNumber(float number)
        {
            string unit = "";
            if (number >= 1000)
            {
                number = number / 1000;
                unit = "K";
            }
            return String.Format(new CultureInfo("en-US"), "{0:0.##}{1}", number, unit);
        }

        // Number formatter using proper metric prefixes
        public static string MetricPrefix(double value, string unit) {
            foreach (var item in units)
            {
                if (item.Value == unit) {
                    value = value * Math.Pow(10, item.Key);
                    break;
                }
            }
            KeyValuePair<int, string> res = new KeyValuePair<int, string>(0, "");
            foreach (var item in units)
            {
                if (value >= Math.Pow(10, item.Key)) {
                    res = item;
                }
            }
            return String.Format(new CultureInfo("en-US"), "{0:0.##} {1}", value/Math.Pow(10, res.Key), res.Value);
        }


        // Return the ore or ingot item that represents a reagent
        public static string OreGot(String name, bool wantOres) {
            // Ore name
            if (wantOres && ores.Contains(name))
            {
                return "Item" + name + "Ore";
            }
            // Ingot name
            if (!wantOres && ingots.Contains(name))
            {
                return "Item" + name + "Ingot";
            }

            // Wants ores, but has none, might be an ingot
            if (ingots.Contains(name) && !ores.Contains(name))
            {
                return "Item" + name + "Ingot";
            }

            // Neither Ore or Ingot
            return name;
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
    }

}