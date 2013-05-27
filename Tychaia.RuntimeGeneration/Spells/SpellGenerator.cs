using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Spells
{
    public static class SpellGenerator
    {
        internal static Dictionary<Element, double> Elements = new Dictionary<Element, double>();
        internal static Dictionary<SpellType, double> Types = new Dictionary<SpellType, double>();
        internal static Dictionary<SpellModifier, double> Modifiers = new Dictionary<SpellModifier, double>();
        internal static double TotalWeightingElements = 0;
        internal static double TotalWeightingTypes = 0;
        internal static double TotalWeightingModifiers = 0;

        static SpellGenerator()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(Element).IsAssignableFrom(t) && !t.IsAbstract)
                    Elements.Add(
                        (Element)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
                if (typeof(SpellType).IsAssignableFrom(t) && !t.IsAbstract)
                    Types.Add(
                        (SpellType)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
                if (typeof(SpellModifier).IsAssignableFrom(t) && !t.IsAbstract)
                    Modifiers.Add(
                        (SpellModifier)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
            }

            foreach (KeyValuePair<Element, double> kv in Elements)
                TotalWeightingElements += kv.Value;
            foreach (KeyValuePair<SpellType, double> kv in Types)
                TotalWeightingTypes += kv.Value;
            foreach (KeyValuePair<SpellModifier, double> kv in Modifiers)
                TotalWeightingModifiers += kv.Value;
        }

        public static Spell Generate(int input)
        {
            Random r = new Random(input);
            double elementSelect = r.NextDouble() * TotalWeightingElements;
            double typeSelect = r.NextDouble() * TotalWeightingTypes;
            double modifierSelect = r.NextDouble() * TotalWeightingModifiers;
            Element element = Locate<Element>(Elements, elementSelect);
            SpellType type = Locate<SpellType>(Types, typeSelect);
            SpellModifier modifier = Locate<SpellModifier>(Modifiers, modifierSelect);
            return new Spell(element, type, modifier);
        }

        private static T Locate<T>(Dictionary<T, double> dict, double select)
        {
            double count = 0;
            foreach (var v in dict)
                if (select >= count && select < count + v.Value)
                    return v.Key;
                else
                    count += v.Value;
            throw new InvalidOperationException();
        }
    }
}
