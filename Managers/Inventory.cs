using Bound.Managers;
using Bound.Models.Items;
using Bound.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

using static Bound.Managers.TextureManager;

namespace Bound.Managers
{
    public class Inventory
    {

        private Game1 _game;
        private Sprite _owner;
        private Dictionary<string, Item> _headgear = new Dictionary<string, Item>();
        private Dictionary<string, Item> _chestArmour = new Dictionary<string, Item>();
        private Dictionary<string, Item> _legArmour = new Dictionary<string, Item>();
        private Dictionary<string, Item> _footwear = new Dictionary<string, Item>();
        private Dictionary<string, Item> _accessories = new Dictionary<string, Item>();
        private Dictionary<string, Item> _weapons = new Dictionary<string, Item>();
        private Dictionary<string, Item> _consumables = new Dictionary<string, Item>();
        private Dictionary<string, Item> _skills = new Dictionary<string, Item>();
        private List<Dictionary<string, Item>> _inventory;

        private static Item Blank = new Item(new Models.TextureCollection(), -1, "Blank", "null", ItemType.Unrecognised);

        public List<string> FlatInventory
        {
            get
            {
                var inv = new List<string>();
                foreach (var list in _inventory)
                    inv.AddRange(list.Keys); //returns strings not items for data safety
                return inv;
            }
        }

        public Sprite Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                foreach (var dict in _inventory)
                    foreach (var item in dict.Values)
                        item.Owner = _owner;
            }
        }

        public Inventory(Game1 game, Sprite Owner) 
        {
            _inventory = new List<Dictionary<string, Item>>()
            {
                _headgear, _chestArmour, _legArmour, _footwear, _accessories, _weapons, _consumables, _skills
            };

            _game = game;
            _owner = Owner;
        }

        public void Add(Item item)
        {
            var index = (int)item.Type;
            if (index >= _inventory.Count)
                return;

            _inventory[index].Add(item.Name, item.Clone());
            _inventory[index].Values.ToArray()[^1].Owner = _owner;
        }

        public void Add(string name, int numberTaken = 1)
        {
            var item = _game.Items[name].Clone();
            item.Quantity = numberTaken;
            item.Owner = _owner;
            var index = (int)item.Type;
            if (index >= _inventory.Count)
                return;

            if (!_inventory[index].ContainsKey(name))
                _inventory[index].Add(name, item);
            else
                _inventory[index][item.Name].Quantity += numberTaken;
        }

        //For importing items from saves
        public void Import(string item)
        {
            var args = new Dictionary<string, string>();

            foreach (var arg in item.Split(", "))
            {
                var kvp = arg.Split(": ");
                args.Add(kvp[0], kvp[1]);
            }

            var newItem = _game.Items[args["Name"]].Clone();
            newItem.Owner = _owner;
            if (args.ContainsKey("Ammount")) newItem.Quantity = int.Parse(args["Ammount"]);

            var index = (int)newItem.Type;
            if (index >= _inventory.Count) //If the item type is unrecognised it wont be added since it can't be sorted into a category
                return;

            _inventory[index].Add(newItem.Name, newItem);


        }

        public List<Item> GetParts(List<ItemType> types)
        {
            var output = new List<Item>();
            foreach (var t in types)
                output.AddRange(_inventory[(int)t].Values);
            return output;
        }

        public List<string> FormatForSave()
        {
            var output = new List<string>();
            foreach (var dict in _inventory)
            {
                foreach (var item in dict.Values)
                {
                    var str = $"Name: {item.Name}";
                    if (item.Quantity > 1)
                        str += $", Ammount: {item.Quantity}";
                    output.Add(str);
                }
            }

            return output;
        }

        public Item GetItem(TextureManager.ItemType type, string name)
        {
            if ((int)type > _inventory.Count)
                return null;
            else if (type == TextureManager.ItemType.HoldableItem)
            {
                if (_consumables.ContainsKey(name))
                    return _consumables[name]; 
                else if (_weapons.ContainsKey(name))
                    return _weapons[name]; 
            }
            else if (_inventory[(int)type].ContainsKey(name))
                return _inventory[(int)type][name];

            return Blank;
            
        }

        public void RemoveItem(string name)
        {
            foreach (var dict in _inventory)
                foreach (var kvp in dict)
                    if (kvp.Key == name)
                    {
                        dict.Remove(kvp.Key);
                        return;
                    }
        }

        public bool Contains(string name)
        {
            foreach (var dict in _inventory)
                if (dict.ContainsKey(name))
                    return true;
            return false;
        }
    }
}
