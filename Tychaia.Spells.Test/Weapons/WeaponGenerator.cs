using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Weapons
{
    public static class WeaponGenerator
    {
        internal static Dictionary<Element, double> Elements = new Dictionary<Element, double>();
        internal static Dictionary<WeaponType, double> Types = new Dictionary<WeaponType, double>();
        internal static Dictionary<WeaponModifier, double> Modifiers = new Dictionary<WeaponModifier, double>();
        internal static double TotalWeightingElements = 0;
        internal static double TotalWeightingTypes = 0;
        internal static double TotalWeightingModifiers = 0;

        static WeaponGenerator()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(Element).IsAssignableFrom(t) && !t.IsAbstract)
                    Elements.Add(
                        (Element)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
                if (typeof(WeaponType).IsAssignableFrom(t) && !t.IsAbstract)
                    Types.Add(
                        (WeaponType)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
                if (typeof(WeaponModifier).IsAssignableFrom(t) && !t.IsAbstract)
                    Modifiers.Add(
                        (WeaponModifier)t.GetConstructor(Type.EmptyTypes).Invoke(null),
                        (double)t.GetField("Weight").GetValue(null)
                    );
            }

            foreach (KeyValuePair<Element, double> kv in Elements)
                TotalWeightingElements += kv.Value;
            foreach (KeyValuePair<WeaponType, double> kv in Types)
                TotalWeightingTypes += kv.Value;
            foreach (KeyValuePair<WeaponModifier, double> kv in Modifiers)
                TotalWeightingModifiers += kv.Value;
        }

        public static Weapon Generate(int input)
        {
            Random r = new Random(input);
            double elementSelect = r.NextDouble() * TotalWeightingElements;
            double typeSelect = r.NextDouble() * TotalWeightingTypes;
            double modifierSelect = r.NextDouble() * TotalWeightingModifiers;
            Element element = Locate<Element>(Elements, elementSelect);
            WeaponType type = Locate<WeaponType>(Types, typeSelect);
            WeaponModifier modifier = Locate<WeaponModifier>(Modifiers, modifierSelect);
            return new Weapon(element, type, modifier);
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
