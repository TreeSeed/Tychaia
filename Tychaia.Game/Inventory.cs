// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Linq;
using Dx.Runtime;

namespace Tychaia.Game
{
    public class Inventory
    {
        [Synchronised]
        private List<Item> m_Items = new List<Item>();

        public IEnumerable<Item> AllItems
        {
            get
            {
                return this.m_Items.AsEnumerable();
            }
        }
        
        public IEnumerable<Item> UnequippedItems
        {
            get
            {
                return from item in this.m_Items
                       where this.ArmorItem != item &&
                             this.HeavySlotItem != item &&
                             this.MediumSlot1Item != item &&
                             this.MediumSlot2Item != item &&
                             this.LightSlotItem != item &&
                             this.SpellBookItem != item
                       select item;
            }
        }
        
        public IEnumerable<Item> EquippedItems
        {
            get
            {
                if (this.ArmorItem != null)
                    yield return this.ArmorItem;
                if (this.HeavySlotItem != null)
                    yield return this.HeavySlotItem;
                if (this.MediumSlot1Item != null)
                    yield return this.MediumSlot1Item;
                if (this.MediumSlot2Item != null)
                    yield return this.MediumSlot2Item;
                if (this.LightSlotItem != null)
                    yield return this.LightSlotItem;
                if (this.SpellBookItem != null)
                    yield return this.SpellBookItem;
            }
        }

        public Armor ArmorItem { get; set; }
        public WeightedItem HeavySlotItem { get; set; }
        public WeightedItem MediumSlot1Item { get; set; }
        public WeightedItem MediumSlot2Item { get; set; }
        public WeightedItem LightSlotItem { get; set; }
        public SpellBook SpellBookItem { get; set; }

        public void Add(Item item)
        {
            if (!this.m_Items.Contains(item))
                this.m_Items.Add(item);
        }

        public void Remove(Item item)
        {
            if (this.m_Items.Contains(item))
                this.m_Items.Remove(item);
        }

        public bool Equip(Item item, bool automaticallyUnequip = true)
        {
            if (!this.AllItems.Contains(item))
                return false;

            var armor = item as Armor;
            if (armor != null)
            {
                if (this.ArmorItem == null || automaticallyUnequip)
                {
                    this.ArmorItem = armor;
                    return true;
                }
                
                return false;
            }

            var spellBook = item as SpellBook;
            if (spellBook != null)
            {
                if (this.SpellBookItem == null || automaticallyUnequip)
                {
                    this.SpellBookItem = spellBook;
                    return true;
                }
                
                return false;
            }

            var weightedItem = item as WeightedItem;
            if (weightedItem == null)
                return false;

            if (weightedItem.Weight == Weight.Heavy)
            {
                if (this.HeavySlotItem == null || automaticallyUnequip)
                {
                    this.HeavySlotItem = weightedItem;
                    return true;
                }
                
                return false;
            }

            if (weightedItem.Weight == Weight.Medium)
            {
                if (this.MediumSlot1Item == null)
                {
                    this.MediumSlot1Item = weightedItem;
                    return true;
                }
                
                if (this.MediumSlot2Item == null)
                {
                    this.MediumSlot2Item = weightedItem;
                    return true;
                }
                
                if (automaticallyUnequip)
                {
                    this.MediumSlot1Item = weightedItem;
                    return true;
                }
                
                return false;
            }

            if (weightedItem.Weight == Weight.Light)
            {
                if (this.LightSlotItem == null || automaticallyUnequip)
                {
                    this.LightSlotItem = weightedItem;
                    return true;
                }
                
                return false;
            }

            return false;
        }
    }
}
