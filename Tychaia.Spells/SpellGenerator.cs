using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Tychaia.Spells
{
    public static class SpellGenerator
    {
        private static List<SpellElement> Elements = new List<SpellElement>();
        private static List<SpellType> Types = new List<SpellType>();
        private static List<SpellModifier> Modifiers = new List<SpellModifier>();

        static SpellGenerator()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(SpellElement).IsAssignableFrom(t) && !t.IsAbstract)
                    Elements.Add((SpellElement)t.GetConstructor(Type.EmptyTypes).Invoke(null));
                if (typeof(SpellType).IsAssignableFrom(t) && !t.IsAbstract)
                    Types.Add((SpellType)t.GetConstructor(Type.EmptyTypes).Invoke(null));
                if (typeof(SpellModifier).IsAssignableFrom(t) && !t.IsAbstract)
                    Modifiers.Add((SpellModifier)t.GetConstructor(Type.EmptyTypes).Invoke(null));
            }
        }

        public static Spell Generate(int input)
        {
            Random r = new Random(input);
            int elementSelect = r.Next() % Elements.Count;
            int typeSelect = r.Next() % Types.Count;
            int modifierSelect = r.Next() % Modifiers.Count;
            return new Spell(Elements[elementSelect], Types[typeSelect], Modifiers[modifierSelect]);
        }
    }
}
