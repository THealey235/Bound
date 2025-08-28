using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Bound.Managers
{
    public class Textures
    {
        #region PreDefined

        public int BlockWidth = 16;

        public enum Blocks
        {
            Grass, Path, OakPlank, OakLog, Glass, DoorA, DoorB, OakSlab, BlankTile, DirtGradient
        }

        public List<int> GhostBlocks = new List<int>() //contains blocks that you can walk through (no hitbox)
        {
            (int)Blocks.DoorA, (int)Blocks.DoorB, (int)Blocks.BlankTile, (int)Blocks.DirtGradient
        };

        public enum ItemTextureType
        {
            Icon,
            PlayerModel,
        }

        public enum ItemType
        {
            HeadGear,
            ChestArmour,
            LegArmour,
            Footwear,
            Accessory,
            Weapon,
            Consumable,
            Skill,
            HoldableItem,
            Item,
            Unrecognised,
        }

        #endregion

        private ContentManager _content;
        private Game1 _game;

        public Texture2D BlockAtlas;
        public Texture2D Button;
        public Texture2D BaseBackground;
        public Texture2D RedX;
        public Texture2D ArrowLeft;
        public Texture2D Plus;
        public Texture2D PlayButton;
        public Texture2D TrashCan;
        public Texture2D Null;
        public Texture2D Blank;
        public Texture2D PlayerStatic;
        public Texture2D Block;
        public Texture2D HotbarBG;
        public Texture2D HotbarSelectedSlot;
        public Texture2D EmptyBox;

        public Dictionary<string, Texture2D> HeadGear = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> ChestArmour = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> LegArmour = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Footwear= new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Accessories = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Weapons = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Consumables = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Skills = new Dictionary<string, Texture2D>();

        public Dictionary<string, Texture2D> Items = new Dictionary<string, Texture2D>();
        private Dictionary<int, string> IdToName = new Dictionary<int, string>();

        public Dictionary<string, Texture2D> Buttons;

        public List<SpriteFont> Fonts;

        

        public SpriteFont Font
        {
            get
            {
                return Game1.ScreenHeight switch
                {
                    720 => Fonts[0],
                    900 => Fonts[1],
                    1080 => Fonts[2],
                    1440 => Fonts[3],
                    2160 => Fonts[4],
                    _ => Fonts[2],
                };
            }
        }

        public Textures(ContentManager content, Game1 game)
        {

            _content = content;
            _game = game;

            Null = content.Load<Texture2D>("MissingTexture");
            Blank = new Texture2D(game.GraphicsDevice, 32, 32);
            Blank.SetData(Enumerable.Repeat(Color.Transparent, 32 * 32).ToArray());
            Fonts = new List<SpriteFont>()
            {
                content.Load<SpriteFont>("Fonts/JX-720"),
                content.Load<SpriteFont>("Fonts/JX-900"),
                content.Load<SpriteFont>("Fonts/JX-1080"),
                content.Load<SpriteFont>("Fonts/JX-1440"),
                content.Load<SpriteFont>("Fonts/JX-2160"),
            };

            #region Controls

            BaseBackground = content.Load<Texture2D>("Backgrounds/BaseBackground");
            RedX = content.Load<Texture2D>("Controls/Icos/RedX");
            ArrowLeft = content.Load<Texture2D>("Controls/Icos/ArrowLeft");
            Plus = content.Load<Texture2D>("Controls/Icos/Plus");
            PlayButton = content.Load<Texture2D>("Controls/Icos/PlayButton");
            TrashCan = content.Load<Texture2D>("Controls/Icos/TrashCan");
            Button = content.Load<Texture2D>("Controls/Buttons/Button1");

            Buttons = new Dictionary<string, Texture2D>
            {
                { "B&W", content.Load<Texture2D>("Controls/Buttons/BW")},
                { "Blank", content.Load<Texture2D>("Controls/Buttons/Button") }
            };

            EmptyBox = content.Load<Texture2D>("Controls/EmptyBox");
            #endregion

            #region Game Elements
            PlayerStatic = content.Load<Texture2D>("Player/Player1Static");
            BlockAtlas = content.Load<Texture2D>("Atlases/BlockAtlas");
            Block = content.Load<Texture2D>("Atlases/DirtBlock");
            HotbarBG = content.Load<Texture2D>("Backgrounds/HotbarBG");
            HotbarSelectedSlot = content.Load<Texture2D>("Backgrounds/HotbarSelectedBG");

            LoadDirectory("Content/Items/HeadGear", HeadGear);
            LoadDirectory("Content/Items/ChestArmour", ChestArmour);
            LoadDirectory("Content/Items/LegArmour", LegArmour);
            LoadDirectory("Content/Items/Footwear", Footwear);
            LoadDirectory("Content/Items/Accessories", Accessories);
            LoadDirectory("Content/Items/Consumables", Consumables);
            LoadDirectory("Content/Items/Skills", Skills);


            foreach (var dict in new List<Dictionary<string, Texture2D>>() { HeadGear, ChestArmour, LegArmour, Footwear, Accessories, Consumables, Skills})
            {
                foreach (var kvp in dict)
                {
                    if (kvp.Key == "Default") continue;
                    Items.Add(kvp.Key, kvp.Value);
                }
                if (!dict.ContainsKey("Default")) dict.Add("Default", Blank);
            }

            Items.Add("Default", Blank);

            #endregion

        }

        //was used for loading items with multiple sprites such as a chest piece with a worn sprite and a menu sprite
        //but it is more efficient to keep it as one texture and use a source rectangle so this may be deprecated
        //although this code may be useful in the future.
        private void LoadDirectory(string path, Dictionary<string, List<Texture2D>> outputList)
        {
            string x;
            string name;
            Texture2D texture;

            foreach (var file in System.IO.Directory.GetFiles(path))
            {
                x = string.Join('/', file.Replace('\\', '/').Split('/').Skip(1)).Replace(".xnb", String.Empty);
                name = x.Split('/')[^1];
                texture = _content.Load<Texture2D>(x);
                outputList.Add(
                    name,
                    new List<Texture2D> {
                        CreateSubTexture(_game.GraphicsDevice, texture, new Rectangle(0, 0, 32, 32)),
                        CreateSubTexture(_game.GraphicsDevice, texture, new Rectangle(32, 0, 32, 32))
                    }
                );
            }
        }

        private void LoadDirectory (string path, Dictionary<string, Texture2D> outputList)
        {
            string x;

            foreach (var file in System.IO.Directory.GetFiles(path))
            {
                x = string.Join('/', file.Replace('\\', '/').Split('/').Skip(1)).Replace(".xnb", String.Empty);
                outputList.Add(
                    x.Split('/')[^1],
                    _content.Load<Texture2D>(x)
                );
            }
        }
        public Dictionary<string, Item> LoadItems()
        {
            List<List<string>> readItems;
            using (var reader = new StreamReader(new FileStream("Content/Items.csv", FileMode.Open)))
            {
                readItems = reader.ReadToEnd()
                        .Split("\n")
                        .Select(x => x.Split(";")
                            .ToList())
                        .ToList();
            }

            var items = new Dictionary<string, Item>();

            // 0: Name, 1: ItemType, 2: Description, 3: Attributes
            Texture2D texture;
            for (int i = 0; i < readItems.Count; i++)
            {
                try
                {
                    texture = (Items.ContainsKey(readItems[i][0])) ? Items[readItems[i][0]] : Null;
                    //If it has attributes
                    if (readItems[i].Count > 2)
                        items.Add(readItems[i][0], new Item(texture, i, readItems[i][0], readItems[i][2], StringToType(readItems[i][1])));
                    else
                        items.Add(readItems[i][0], new Item(texture, i, readItems[i][0], readItems[i][2], readItems[i][1], StringToType(readItems[i][3])));

                    IdToName.Add(i, readItems[i][0]);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return items;

        }

        private ItemType StringToType(string input)
        {
            var output = input.ToLower() switch
            {
                "headgear" => ItemType.HeadGear,
                "chestarmour" => ItemType.ChestArmour,
                "legarmour" => ItemType.LegArmour,
                "footwear" => ItemType.Footwear,
                "accessory" => ItemType.Accessory,
                "consumable" => ItemType.Consumable,
                "skill" => ItemType.Skill,
                "weapon" => ItemType.Weapon,
                _ => ItemType.Unrecognised,
            };
            return output;
        }

        public Texture2D CreateSubTexture(GraphicsDevice graphicsDevice, Texture2D sourceTexture, Rectangle sourceRectangle)
        {
            Color[] sourceData = new Color[sourceTexture.Width * sourceTexture.Height];
            sourceTexture.GetData(sourceData);

            Color[] newTextureData = new Color[sourceRectangle.Width * sourceRectangle.Height];

            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                for (int x = 0; x < sourceRectangle.Width; x++)
                {
                    int sourceIndex = (sourceRectangle.Y + y) * sourceTexture.Width + (sourceRectangle.X + x);
                    int targetIndex = y * sourceRectangle.Width + x;
                    newTextureData[targetIndex] = sourceData[sourceIndex];
                }
            }

            Texture2D newTexture = new Texture2D(graphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            newTexture.SetData(newTextureData);

            return newTexture;
        }

        public Texture2D GetItemTexture(string itemName)
        {
            try
            {
                return Items[itemName];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Null;
            }
        }

        public Rectangle GetSourceRectangle(ItemType itemType, ItemTextureType textureType = ItemTextureType.Icon)
        {
            if ((int)(itemType) < 4)
            {
                return textureType switch
                {
                    ItemTextureType.PlayerModel => new Rectangle(32, 0, 32, 32),
                    _ => new Rectangle(0, 0, 32, 32),
                };
            }

            return new Rectangle(0, 0, 32, 32);
        }

        public string GetItemName(int ID) => (ID < IdToName.Count) ? IdToName[ID] : "Default";
    }
}
