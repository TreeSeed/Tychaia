// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Xunit;

namespace Tychaia.Game.Tests
{
    public class InventoryTests
    {
        [Fact]
        public void CanEquipItemIntoEmptySlot()
        {
            var inventory = new Inventory();
            var item = new WeightedItem { Weight = Weight.Heavy, Name = "hello" };
            inventory.Add(item);
            Assert.True(inventory.Equip(item));
            Assert.Same(inventory.HeavySlotItem, item);
        }

        [Fact]
        public void CanEquipItemIntoTakenSlotIfAutoUnequip()
        {
            var inventory = new Inventory();
            var item1 = new WeightedItem { Weight = Weight.Heavy, Name = "hello1" };
            var item2 = new WeightedItem { Weight = Weight.Heavy, Name = "hello2" };
            inventory.Add(item1);
            inventory.Add(item2);
            Assert.True(inventory.Equip(item1));
            Assert.Same(inventory.HeavySlotItem, item1);
            Assert.True(inventory.Equip(item2));
            Assert.Same(inventory.HeavySlotItem, item2);
        }

        [Fact]
        public void CanNotEquipItemIntoTakenSlotIfNoAutoUnequip()
        {
            var inventory = new Inventory();
            var item1 = new WeightedItem { Weight = Weight.Heavy, Name = "hello1" };
            var item2 = new WeightedItem { Weight = Weight.Heavy, Name = "hello2" };
            inventory.Add(item1);
            inventory.Add(item2);
            Assert.True(inventory.Equip(item1, false));
            Assert.Same(inventory.HeavySlotItem, item1);
            Assert.False(inventory.Equip(item2, false));
            Assert.Same(inventory.HeavySlotItem, item1);
            Assert.NotSame(inventory.HeavySlotItem, item2);
            Assert.NotNull(inventory.HeavySlotItem);
        }

        [Fact]
        public void CanNotEquipItemNotInInventory()
        {
            var inventory = new Inventory();
            var item = new WeightedItem { Weight = Weight.Heavy, Name = "hello" };
            Assert.False(inventory.Equip(item));
            inventory.Add(item);
            Assert.True(inventory.Equip(item));
        }

        [Fact]
        public void CanNotEquipNonWeightedItem()
        {
            var inventory = new Inventory();
            var item = new Item { Name = "hello" };
            inventory.Add(item);
            Assert.False(inventory.Equip(item));
        }

        [Fact]
        public void CanAddNonWeightedItem()
        {
            var inventory = new Inventory();
            var item = new Item { Name = "hello" };
            inventory.Add(item);
            Assert.Contains(item, inventory.AllItems);
            Assert.Contains(item, inventory.UnequippedItems);
            Assert.DoesNotContain(item, inventory.EquippedItems);
        }

        [Fact]
        public void CollectionBehaviourIsCorrect()
        {
            var inventory = new Inventory();
            var item1 = new WeightedItem { Weight = Weight.Heavy, Name = "hello1" };
            var item2 = new WeightedItem { Weight = Weight.Heavy, Name = "hello2" };
            inventory.Add(item1);
            inventory.Add(item2);
            Assert.Contains(item1, inventory.AllItems);
            Assert.Contains(item2, inventory.AllItems);
            Assert.Contains(item1, inventory.UnequippedItems);
            Assert.Contains(item2, inventory.UnequippedItems);
            Assert.DoesNotContain(item1, inventory.EquippedItems);
            Assert.DoesNotContain(item2, inventory.EquippedItems);
            Assert.True(inventory.Equip(item1));
            Assert.DoesNotContain(item1, inventory.UnequippedItems);
            Assert.Contains(item2, inventory.UnequippedItems);
            Assert.Contains(item1, inventory.EquippedItems);
            Assert.DoesNotContain(item2, inventory.EquippedItems);
            Assert.True(inventory.Equip(item2));
            Assert.Contains(item1, inventory.UnequippedItems);
            Assert.DoesNotContain(item2, inventory.UnequippedItems);
            Assert.DoesNotContain(item1, inventory.EquippedItems);
            Assert.Contains(item2, inventory.EquippedItems);
        }

        [Fact]
        public void CanEquipSpellBook()
        {
            var inventory = new Inventory();
            var spellBook = new SpellBook { Name = "hello" };
            inventory.Add(spellBook);
            Assert.Contains(spellBook, inventory.AllItems);
            Assert.Contains(spellBook, inventory.UnequippedItems);
            Assert.DoesNotContain(spellBook, inventory.EquippedItems);
            Assert.Null(inventory.SpellBookItem);
            inventory.Equip(spellBook);
            Assert.Contains(spellBook, inventory.AllItems);
            Assert.DoesNotContain(spellBook, inventory.UnequippedItems);
            Assert.Contains(spellBook, inventory.EquippedItems);
            Assert.Same(spellBook, inventory.SpellBookItem);
        }

        [Fact]
        public void CanEquipArmor()
        {
            var inventory = new Inventory();
            var armor = new Armor { Name = "hello" };
            inventory.Add(armor);
            Assert.Contains(armor, inventory.AllItems);
            Assert.Contains(armor, inventory.UnequippedItems);
            Assert.DoesNotContain(armor, inventory.EquippedItems);
            Assert.Null(inventory.ArmorItem);
            inventory.Equip(armor);
            Assert.Contains(armor, inventory.AllItems);
            Assert.DoesNotContain(armor, inventory.UnequippedItems);
            Assert.Contains(armor, inventory.EquippedItems);
            Assert.Same(armor, inventory.ArmorItem);
        }
    }
}

