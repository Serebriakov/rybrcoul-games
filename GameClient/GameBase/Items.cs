using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGame.GameBase
{
    public class Item
    {
        public int Id;
        public float Price;
        public bool IsSellable;
        public bool IsInventorable;
        public String InventoryPicture;
        public String Name;
    }

    public class Weapon : Item
    {
        public WeaponType WeaponType;
    }

    public enum WeaponType
    {
        ONEHANDEDSWORD,
        TWOHANDEDSWORD,
        AXE,
        BOW,
        CROSSBOW,
        SPELL,
    }

    public class Inventory
    {
        private Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public void AddItem(Item item)
        {
            if (!Items.ContainsKey(item.Id))
            {
                Items.Add(item.Id, item);
            }
        }

        public void RemoveItem(int id)
        {
            if (Items.ContainsKey(id))
            {
                Items.Remove(id);
            }
        }

        public void RemoveItem(Item item)
        {
            if (Items.ContainsKey(item.Id))
            {
                Items.Remove(item.Id);
            }
        }

        public Item GetItem(int id)
        {
            if (Items.ContainsKey(id))
                return Items[id];
            else return null;
        }

        public void MoveItem(int id, Inventory inventory)
        {
            if (Items.ContainsKey(id))
            {
                inventory.AddItem(Items[id]);
                Items.Remove(id);
            }
        }

        public void MoveItem(Item item, Inventory inventory)
        {
            if (Items.ContainsValue(item))
            {
                inventory.AddItem(item);
                Items.Remove(item.Id);
            }
        }
    }
}
