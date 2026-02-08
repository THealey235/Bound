using Bound.Managers;
using Bound.Models.Items;
using Bound.Sprites;
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
        private Dictionary<string, float> _statBoosts = new Dictionary<string, float>() { {"PDEF", 0f}, { "MDEF", 0 } };
        private static Item Default = new Item(new Models.TextureCollection(), -1, "Default", "null", ItemType.Unrecognised);
        private Dictionary<string, List<Item>> _equippedItems = new Dictionary<string, List<Item>>()
        {
            { "headgear", new List<Item>()},
            { "chestarmour", new List<Item>()},
            { "legarmour", new List<Item>()},
            { "footwear", new List<Item>()},
            { "accessory", new List<Item>()},
            { "hotbar", new List<Item>()},
            { "skills", new List<Item>()},
        };

        public int Money;

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

        public List<Item> AllItems
        {
            get
            {
                var output = new List<Item>();
                _inventory.ForEach(x => output.AddRange(x.Values));
                return output;
            }
        }

        public Dictionary<string, List<Item>> EquippedItems
        {
            get { return _equippedItems; }
        }

        public Dictionary<string, float> StatBoosts
        {
            get
            {
                return new Dictionary<string, float>(_statBoosts);
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

            if (_inventory[index].ContainsKey(item.Name))
                _inventory[index][item.Name].Quantity += item.Quantity;
            else
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

            if (args["Name"] == "Money")
            {
                Money = int.Parse(args["Ammount"]);
                return;
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
            output.Add($"Name: Money, Ammount: {Money}");
            return output;
        }

        public Item GetItem(string name)
        {
            foreach (var dict in _inventory)
                if (dict.ContainsKey(name)) return dict[name]; 
            return Default;
        }

        public void RemoveItem(string name)
        {
            foreach (var dict in _inventory)
                foreach (var kvp in dict)
                    if (kvp.Key == name)
                    {
                        UpdateStats(dict[kvp.Key].Type, dict[kvp.Key], false);
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

        public bool EquipItem(string key, string name)
        {
            if ((!Contains(name) && !(name == "Default")) || !_equippedItems.ContainsKey(key))
                return false;

            if (name == "Default")
                _equippedItems[key].Add(Default);

            var item = GetItem(name);
            _equippedItems[key].Add(item);

            UpdateStats(item.Type, item, true);
            return true;
        }

        public void ClearEquippedItems()
        {
            foreach (var item in _equippedItems)
                item.Value.Clear();
        }

        private void UpdateStats(ItemType type, Item item, bool toAdd)
        {
            if (_equippedItems.Any(x => x.Value.Contains(item)) && (type == ItemType.HeadGear || type == ItemType.ChestArmour || type == ItemType.LegArmour || type == ItemType.Footwear || type == ItemType.Accessory))
            {
                foreach (var attribute in item.Attributes)
                {
                    if (_statBoosts.ContainsKey(attribute.Key))
                        _statBoosts[attribute.Key] += (toAdd ? 1 : -1) * attribute.Value.Value;
                    else _statBoosts[attribute.Key] = attribute.Value.Value;
                }
            }
        }
    }
}
