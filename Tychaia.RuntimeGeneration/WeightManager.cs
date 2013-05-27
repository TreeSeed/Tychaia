using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tychaia.RuntimeGeneration.Elements;
using Tychaia.RuntimeGeneration.Spells;

namespace Tychaia.RuntimeGeneration
{
    public static class WeightManager
    {
        public static void PrintWeights(string filter = "")
        {
            foreach (var v in SpellGenerator.Elements.Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
                Console.WriteLine(v.Key.GetType().Name.PadRight(30) + " - " + v.Value);
            foreach (var v in SpellGenerator.Types.Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
                Console.WriteLine(v.Key.GetType().Name.PadRight(30) + " - " + v.Value);
            foreach (var v in SpellGenerator.Modifiers.Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
                Console.WriteLine(v.Key.GetType().Name.PadRight(30) + " - " + v.Value);
        }

        public static void SaveWeights()
        {
            Console.Write("Saving to weights.txt... ");
            using (StreamWriter writer = new StreamWriter("weights.txt"))
            {
                foreach (var v in SpellGenerator.Elements)
                    writer.WriteLine("E " + v.Key.GetType().Name + " " + v.Value);
                foreach (var v in SpellGenerator.Types)
                    writer.WriteLine("T " + v.Key.GetType().Name + " " + v.Value);
                foreach (var v in SpellGenerator.Modifiers)
                    writer.WriteLine("M " + v.Key.GetType().Name + " " + v.Value);
            }
            Console.WriteLine("done.");
        }

        public static void LoadWeights()
        {
            if (!File.Exists("weights.txt"))
            {
                Console.WriteLine("The weights.txt file does not exist.");
                return;
            }

            Console.Write("Loading from weights.txt... ");
            int count = 1;
            try
            {
                using (StreamReader reader = new StreamReader("weights.txt"))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] r = reader.ReadLine().Split(' ');
                        bool set = false;
                        switch (r[0])
                        {
                            case "E":
                                foreach (var v in SpellGenerator.Elements.Clone())
                                    if (v.Key.GetType().Name.ToLower() == r[1].ToLower())
                                    {
                                        SpellGenerator.TotalWeightingElements += Convert.ToDouble(r[2]) - SpellGenerator.Elements[v.Key];
                                        SpellGenerator.Elements[v.Key] = Convert.ToDouble(r[2]);
                                        set = true;
                                    }
                                break;
                            case "T":
                                foreach (var v in SpellGenerator.Types.Clone())
                                    if (v.Key.GetType().Name.ToLower() == r[1].ToLower())
                                    {
                                        SpellGenerator.TotalWeightingTypes += Convert.ToDouble(r[2]) - SpellGenerator.Types[v.Key];
                                        SpellGenerator.Types[v.Key] = Convert.ToDouble(r[2]);
                                        set = true;
                                    }
                                break;
                            case "M":
                                foreach (var v in SpellGenerator.Modifiers.Clone())
                                    if (v.Key.GetType().Name.ToLower() == r[1].ToLower())
                                    {
                                        SpellGenerator.TotalWeightingModifiers += Convert.ToDouble(r[2]) - SpellGenerator.Modifiers[v.Key];
                                        SpellGenerator.Modifiers[v.Key] = Convert.ToDouble(r[2]);
                                        set = true;
                                    }
                                break;
                            default:
                                Console.WriteLine("malformed on line " + count + "!");
                                return;
                        }
                        if (!set)
                        {
                            Console.WriteLine("unknown type on line " + count + "!");
                            return;
                        }
                        count++;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("exception on line " + count + "!");
                return;
            }
        }

        public static void ChangeWeights(string filter, double value)
        {
            Console.WriteLine("------------------------ OLD ------------------------");
            PrintWeights(filter);
            foreach (var v in SpellGenerator.Elements.Clone().Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
            {
                SpellGenerator.TotalWeightingElements += value - SpellGenerator.Elements[v.Key];
                SpellGenerator.Elements[v.Key] = value;
            }
            foreach (var v in SpellGenerator.Types.Clone().Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
            {
                SpellGenerator.TotalWeightingTypes += value - SpellGenerator.Types[v.Key];
                SpellGenerator.Types[v.Key] = value;
            }
            foreach (var v in SpellGenerator.Modifiers.Clone().Where(v => v.Key.GetType().Name.ToLower().Contains(filter.ToLower())))
            {
                SpellGenerator.TotalWeightingModifiers += value - SpellGenerator.Modifiers[v.Key];
                SpellGenerator.Modifiers[v.Key] = value;
            }
            Console.WriteLine("------------------------ NEW ------------------------");
            PrintWeights(filter);
            Console.WriteLine("-----------------------------------------------------");
        }
    }

    internal static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }
    }
}
