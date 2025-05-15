using Bound.Models.Items;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bound.Managers.Textures;

namespace Bound.Models
{
    public class Inventory
    {
        /*public enum ItemType
        {
            HeadGear,
            ChestArmour,
            LegArmour,
            Footwear,
            Accessory,
            Weapon,
            Consumable,
            Skill,
            Item,
            Unrecognised,
        }*/
        private Game1 _game;
        private Dictionary<string, Item> _headgear = new Dictionary<string, Item>();
        private Dictionary<string, Item> _chestArmour = new Dictionary<string, Item>();
        private Dictionary<string, Item> _legArmour = new Dictionary<string, Item>();
        private Dictionary<string, Item> _footwear = new Dictionary<string, Item>();
        private Dictionary<string, Item> _accessories = new Dictionary<string, Item>();
        private Dictionary<string, Item> _weapons = new Dictionary<string, Item>();
        private Dictionary<string, Item> _consumables = new Dictionary<string, Item>();
        private Dictionary<string, Item> _skills = new Dictionary<string, Item>();
        private List<Dictionary<string, Item>> _inventory;

        public List<string> EntireInvetory
        {
            get
            {
                var inv = new List<string>();
                foreach (var list in _inventory)
                    inv.AddRange(list.Keys);
                return inv;
            }
        }

        public Inventory(Game1 game) 
        {
            _inventory = new List<Dictionary<string, Item>>()
            {
                _headgear, _chestArmour, _legArmour, _footwear, _accessories, _weapons, _consumables, _skills
            };

            _game = game;
        }

        public void Add(string name, Item item)
        {
            var index = (int)item.Type;
            if (index >= _inventory.Count)
                return;

            _inventory[index].Add(name, item.Clone());
        }

        public void Add(string name, int numberTaken = 1)
        {
            var item = _game.Items[name].Clone();
            var index = (int)item.Type;
            if (index >= _inventory.Count)
                return;

            if (!_inventory[index].ContainsKey(name))
                _inventory[index].Add(name, item);
            else
                _inventory[index][item.Name].Ammount += numberTaken;
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
            if (args.ContainsKey("Ammount")) newItem.Ammount = int.Parse(args["Ammount"]);

            var index = (int)newItem.Type;
            if (index >= _inventory.Count) //If the item type in Unrecognised it wont be added since it can't be sorted into a category
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
                    if (item.Ammount > 1)
                        str += $", Ammount: {item.Ammount}";
                    output.Add(str);
                }
            }

            return output;
        }
    }
}
