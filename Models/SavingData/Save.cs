using Bound.Managers;
using Bound.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models
{
    //This class is like staring at a sick, old, dying man with no arms or legs pretending to be functional and useful
    public class Save
    {
        public SaveManager Manager;

        public string Level;
        public string PlayerName;

        public int MaxHealth;
        public int MoveSpeed;
        public int MaxMana;
        public int MaxStamina;
        public int MaxDashes;

        public Dictionary<string, Attribute> Attributes;
        public Dictionary<int, Item> Inventory;

        public override string ToString()
        {
            //basic encryption, tough enough to prevent laymen from changing critical values but not enough to prevent modders.
            //In other words: too lazy to do anything beter rn
            var str = PlayerName + "\n";
            str += EncryptString("Level", FomratStr(Level, "Level")) + "\n";
            str += "\n";

            var attributeOrder = Attributes.Keys.ToList();
            attributeOrder.Sort();

            foreach (var item in attributeOrder)
                str += EncryptString(Attributes[item].Name, FormatIntAsStr(Attributes[item].Value)) + "\n";
            str += "\n";
            foreach (var item in Inventory.Keys)
                str += EncryptString(FormatIntAsStr(item), SaveManager.InventoryCode) + "\n";

            return str;
        }


        private static void AddMissingKeys(SaveManager manager, Save save, List<string> attributeKeys)
        {
            foreach (var key in attributeKeys)
            {
                if (!save.Attributes.ContainsKey(key))
                    save.Attributes.Add(key, new Attribute(key, manager.DefaultAttributes[key]));
            }
        }

        #region Formating
        public string FormatIntAsStr(int value)
        {
            var strVal = value.ToString();
            return strVal.Length switch
            {
                1 => "000" + strVal,
                2 => "00" + strVal,
                3 => "00" + strVal,
                _ => strVal
            };
        }

        private string FomratStr(string value, string name)
        {
            var maxIndex = Manager.AttributeMap["Level"].Aggregate(0, (a, c) => (c > a) ? c : a);
            for (int i = 0; i < (maxIndex + 1) - value.Length; i++)
                value += "0";
            return value;
        }

        #endregion

        //Hold all methods which encrypt or decrypt parts of the save file, also includes some helepers
        #region Encryption
        public string EncryptString(string name, string value)
        {
            var highestIndex = Manager.AttributeMap[name].Aggregate(0, (a, x) => (x > a) ? x : a);
            var str = GenerateLongNumber(highestIndex + 1).ToList();

            var code = Manager.AttributeMap[name];
            var cursor = 0;
            foreach (var i in code)
            {
                str[i] = value[cursor];
                cursor++;
            }

            return str.Aggregate("", (a, x) => a += x);
        }

        public string EncryptString(string value, List<int> map)
        {
            var highestIndex = map.Aggregate(0, (a, x) => (x > a) ? x : a);
            var str = GenerateLongNumber(highestIndex + 1).ToList();

            var cursor = 0;
            foreach (var i in map)
            {
                str[i] = value[cursor];
                cursor++;
            }

            return str.Aggregate("", (a, x) => a += x);
        }


        //TODO: MAKE NOT STATIC PLS I BEG
        public static string DecryptString(SaveManager manager, string name, string input)
        {
            var str = "";
            foreach (var i in manager.AttributeMap[name])
                str += input[i];

            if (char.IsLetter(str.ToArray()[0]))
            {
                int length = str.Aggregate(0, (a, c) => (char.IsLetter(c)) ? a + 1 : a);
                str = str.Substring(0, length);
            }
                
            return str;
        }
        public string GenerateLongNumber(int length)
        {
            var str = "";
            for (var i = 0; i < length; i++)
            {
                str += Game1.Random.Next(0, 10);
            }
            return str;
        }

        //ALL THIS CRAP BEING STATIC IS PISSING ME OFF
        public static KeyValuePair<int, Item> DecryptItem(string item, Game1 game)
        {
            var strItemCode = "";
            foreach (var index in SaveManager.InventoryCode)
            {
                strItemCode += item[index];
            }

            var itemCode = int.Parse(strItemCode);

            return new KeyValuePair<int, Item>(itemCode, game.Items[itemCode]);
        }
        #endregion

        //returns a new save
        #region New Saves

        public static Save NewSave(SaveManager manager)
        {
            var save = new Save()
            {
                Level = "newgame",
                PlayerName = "_",
                Manager = manager,
                Attributes = new Dictionary<string, Attribute>(),
                Inventory = new Dictionary<int, Item>(),
            };

            AddMissingKeys(manager, save, manager.DefaultAttributes.Keys.ToList());

            return save;
        }


        public static Save ImportSave(SaveManager manager, List<string> values, Game1 game)
        {
            try
            {

                var save = new Save()
                {
                    Level = DecryptString(manager, "Level", values[1]),
                    PlayerName = values[0],
                    Manager = manager,
                    Attributes = new Dictionary<string, Attribute>(),
                    Inventory = new Dictionary<int, Item>(),
                };

                //adds attributes found in file
                var attributeKeys = manager.DefaultAttributes.Keys.ToList();
                attributeKeys.Sort();
                string name;
                int pointer = 3;
                //3 is the ammount of non-Attribute values + 1
                for (int i = pointer; i < attributeKeys.Count + 3; i++)
                {
                    if (values[i] == "\n")
                    {
                        pointer = i + 1;
                        break;
                    }
                    name = attributeKeys[i - 3];
                    save.Attributes.Add(name, new Attribute(name, int.Parse(DecryptString(manager, name, values[i].ToString()))));
                }

                //if the save does not contain some attributes create them using the default values
                if (save.Attributes.Count != manager.DefaultAttributes.Count)
                {
                    AddMissingKeys(manager, save, attributeKeys);
                }

                pointer = (pointer == 3) ? attributeKeys.Count + 4 : pointer;
                //Add inventory
                if (values[pointer] != "")
                {
                    KeyValuePair<int, Item> plainItem;
                    for (int i = pointer; i < values.Count; i++)
                    {
                        plainItem = DecryptItem(values[i], game);
                        save.Inventory.Add(plainItem.Key, plainItem.Value);
                    }
                }

                return save;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Save, load failed");
                return null;
            }
        }

        #endregion

    }
}
