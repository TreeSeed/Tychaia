// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
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
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(Element).IsAssignableFrom(t) && !t.IsAbstract)
                {
                    var constructorInfo = t.GetConstructor(Type.EmptyTypes);
                    if (constructorInfo != null)
                        Elements.Add(
                            (Element) constructorInfo.Invoke(null),
                            (double) t.GetField("Weight").GetValue(null)
                            );
                }
                if (typeof(SpellType).IsAssignableFrom(t) && !t.IsAbstract)
                {
                    var constructorInfo = t.GetConstructor(Type.EmptyTypes);
                    if (constructorInfo != null)
                        Types.Add(
                            (SpellType) constructorInfo.Invoke(null),
                            (double) t.GetField("Weight").GetValue(null)
                            );
                }
                if (typeof(SpellModifier).IsAssignableFrom(t) && !t.IsAbstract)
                {
                    var constructorInfo = t.GetConstructor(Type.EmptyTypes);
                    if (constructorInfo != null)
                        Modifiers.Add(
                            (SpellModifier) constructorInfo.Invoke(null),
                            (double) t.GetField("Weight").GetValue(null)
                            );
                }
            }

            foreach (var kv in Elements)
                TotalWeightingElements += kv.Value;
            foreach (var kv in Types)
                TotalWeightingTypes += kv.Value;
            foreach (var kv in Modifiers)
                TotalWeightingModifiers += kv.Value;
        }

        public static Spell Generate(int input)
        {
            var r = new Random(input);
            var elementSelect = r.NextDouble() * TotalWeightingElements;
            var typeSelect = r.NextDouble() * TotalWeightingTypes;
            var modifierSelect = r.NextDouble() * TotalWeightingModifiers;
            var element = Locate(Elements, elementSelect);
            var type = Locate(Types, typeSelect);
            var modifier = Locate(Modifiers, modifierSelect);
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
