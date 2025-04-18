﻿using Bound.Models.Items;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
namespace Bound.Managers
{
    public class Textures
    {
        #region PreDefined

        public int BlockWidth = 16;

        public enum Blocks
        {
            Grass, Path, OakPlank, OakLog, Glass, DoorA, DoorB, OakSlab
        }

        public List<int> GhostBlocks = new List<int>() //contains blocks that you can walk through
        {
            (int)Blocks.DoorA, (int)Blocks.DoorB
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
            Item,
            Skill,
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

        public Dictionary<string, List<Texture2D>> HeadGear = new Dictionary<string, List<Texture2D>>();
        public Dictionary<string, List<Texture2D>> ChestArmour = new Dictionary<string, List<Texture2D>>();
        public Dictionary<string, List<Texture2D>> LegArmour = new Dictionary<string, List<Texture2D>>();
        public Dictionary<string, List<Texture2D>> Footwear = new Dictionary<string, List<Texture2D>>();
        public Dictionary<string, Texture2D> Accessories = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Items = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Skills = new Dictionary<string, Texture2D>();
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
            HotbarBG = content.Load<Texture2D>("Backgrounds/HotbarBG1");
            HotbarSelectedSlot = content.Load<Texture2D>("Overlays/HotbarSelectedSlot1");

            LoadDirectory("Content/Items/HeadGear", HeadGear);
            LoadDirectory("Content/Items/ChestArmour", ChestArmour);
            LoadDirectory("Content/Items/LegArmour", LegArmour);
            LoadDirectory("Content/Items/Footwear", Footwear);
            LoadDirectory("Content/Items/Accessories", Accessories);


            Accessories["Default"] = Blank;
            Items.Add("Default", Blank);
            Skills.Add("Default", Blank);

            #endregion

        }

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
        public (Dictionary<int, Item>, Dictionary<string, int>) LoadItems()
        {
            List<List<string>> readItems;
            using (var reader = new StreamReader(new FileStream("Content/Items.csv", FileMode.Open)))
            {
                readItems = reader.ReadToEnd()
                        .Split("\r\n")
                        .Select(x => x.Split(";")
                            .ToList())
                        .ToList();
            }

            var items = new Dictionary<int, Item>();
            var codes = new Dictionary<string, int>();

            Texture2D texture;
            for (int i = 0; i < readItems.Count; i++)
            {
                try
                {
                    texture = (Items.ContainsKey(readItems[i][0])) ? Items[readItems[i][0]] : Null;
                    //If it has attributes
                    if (readItems[i].Count > 2)
                        items.Add(i, new Item(texture, i, readItems[i][0], readItems[i][1], readItems[i][2]));
                    else
                        items.Add(i, new Item(texture, i, readItems[i][0], readItems[i][1]));
                    codes.Add(readItems[i][0], i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            

            return (items, codes);
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

        public Texture2D GetItemTexture(string itemName, ItemType itemType, ItemTextureType type)
        {
            var index = (int)type;
            try
            {
                switch (itemType)
                {
                    case ItemType.HeadGear:
                        return HeadGear[itemName][index];
                    case ItemType.ChestArmour:
                        return ChestArmour[itemName][index];
                    case ItemType.LegArmour:
                        return LegArmour[itemName][index];
                    case ItemType.Footwear:
                        return Footwear[itemName][index];
                    case ItemType.Accessory:
                        return Accessories[itemName];
                    case ItemType.Item:
                        return Items[itemName];
                    case ItemType.Skill:
                        return Skills[itemName];
                    default:
                        return Null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Null;
            }
        }
    }
}
