using Bound.Models;
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
    public class TextureManager
    {
        #region PreDefined

        public int BlockWidth = 16;

        //Name corresponding to the index of a texture in the block atlas
        public enum CommonBlocks
        {
            Grass, Path, OakPlank, OakLog, Glass, DoorA, DoorB, OakSlab, BlankTile, DirtGradient0, DirtGradient1, DirtGradient2
        }

        public readonly List<string> CommonBlocksKey = new List<string>()
        {
            "Grass", "Path", "OakPlank", "OakLog", "Glass", "DoorA", "DoorB", "OakSlab", "BlankTile", "DirtGradient0", "DirtGradient1", "DirtGradient2"
        };

        public string GetAtlasKey(int i, string s) => GetAtlasInformation(s).AtlasKeys[i];
            
        public readonly Dictionary<int, Rectangle> SpecialShapeBlocks = new Dictionary<int, Rectangle>()
        {
            {(int)CommonBlocks.OakSlab, new Rectangle(0, 8, 16, 8)}
        };

        public List<int> GhostBlocks = new List<int>() //contains blocks that you can walk through (no hitbox)
        {
            (int)CommonBlocks.DoorA, (int)CommonBlocks.DoorB, (int)CommonBlocks.BlankTile, (int)CommonBlocks.DirtGradient0, (int)CommonBlocks.DirtGradient1, (int)CommonBlocks.DirtGradient2
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

        public record ProjectileInfo(float Scale, bool HasProjTexture, float TextureRotation);
        public record SpriteSheetInfo(int Width, float Speed);

        private readonly Dictionary<string, SpriteSheetInfo> _spriteSheetConstants = new Dictionary<string, SpriteSheetInfo>()
        {
            {"Wooden Sword-Use",  new SpriteSheetInfo(42, 0.45f / 18f)},
        };

        private readonly Dictionary<string, ProjectileInfo> _projectileInfo = new Dictionary<string, ProjectileInfo>()
        {
            {"Throwing Dagger", new ProjectileInfo(0.5f, true, (float)Math.PI / 2f)},
            {"Rock", new ProjectileInfo(0.5f, false, 0f) },
        };

        #endregion

        private ContentManager _content;
        private Game1 _game;

        public Texture2D Button;
        public Texture2D BaseBackground;
        public Texture2D RedX;
        public Texture2D ArrowLeft;
        public Texture2D Plus;
        public Texture2D PlayButton;
        public Texture2D TrashCan;
        public Texture2D Null;
        public Texture2D Blank;
        public Texture2D HotbarBG;
        public Texture2D HotbarSelectedSlot;
        public Texture2D EmptyBox;

        public Dictionary<string, Models.TextureCollection> HeadGear = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> ChestArmour = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> LegArmour = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Footwear= new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Accessories = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Weapons = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Consumables = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Skills = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<string, Models.TextureCollection> Sprites = new Dictionary<string, Models.TextureCollection>();

        public Dictionary<string, Models.TextureCollection> Items = new Dictionary<string, Models.TextureCollection>();
        public Dictionary<ItemType, Dictionary<string, Models.TextureCollection>> SortedItems = new Dictionary<ItemType, Dictionary<string, Models.TextureCollection>>();
        private Dictionary<int, string> IdToName = new Dictionary<int, string>();

        public Dictionary<string, Texture2D> Buttons;
        public List<SpriteFont> Fonts;

        private Dictionary<string, Texture2D> CommonBlockAtlas = new Dictionary<string, Texture2D>();

        

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

        public TextureManager(ContentManager content, Game1 game)
        {

            _content = content;
            _game = game;

            Null = Models.TextureCollection.MissingItemTexture = content.Load<Texture2D>("MissingTexture");
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

            LoadBlockDirectory("Content/Atlases/Common", "common");
            HotbarBG = content.Load<Texture2D>("Backgrounds/HotbarBG");
            HotbarSelectedSlot = content.Load<Texture2D>("Backgrounds/HotbarSelectedBG");

            LoadTextureDirectory("Content/Items/HeadGear", HeadGear);
            LoadTextureDirectory("Content/Items/ChestArmour", ChestArmour);
            LoadTextureDirectory("Content/Items/LegArmour", LegArmour);
            LoadTextureDirectory("Content/Items/Footwear", Footwear);
            LoadTextureDirectory("Content/Items/Accessories", Accessories);
            LoadTextureDirectory("Content/Items/Consumables", Consumables);
            LoadTextureDirectory("Content/Items/Skills", Skills);
            LoadTextureDirectory("Content/Items/Weapons", Weapons);


            foreach (var dict in new List<Dictionary<string, Models.TextureCollection>>() { HeadGear, ChestArmour, LegArmour, Footwear, Accessories, Consumables, Skills, Weapons})
            {
                foreach (var kvp in dict)
                {
                    if (kvp.Key == "Default") continue;
                    Items.Add(kvp.Key, kvp.Value);
                }
                if (!dict.ContainsKey("Default")) dict.Add("Default", new Models.TextureCollection());
            }

            Items.Add("Default", new Models.TextureCollection());
            Items["Default"].Statics.Add("Icon", Blank);

            LoadSprites();

            #endregion

        }


        private void LoadTextureDirectory (string masterPath, Dictionary<string, Models.TextureCollection> outputList)
        {
            var paths = new List<string>();
            try
            { 
                paths.AddRange(Directory.GetFiles(masterPath + "/Statics"));
                paths.AddRange(Directory.GetFiles(masterPath + "/Sheets"));
            }
            catch (Exception e) { }

            var files = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (var path in paths)
            {
                var file = path.Replace("\\", "/").Replace(".xnb", string.Empty);
                var itemName = file.Split('/')[^1].Split('-')[^2];
                if (!files.ContainsKey(itemName))
                {
                    files.Add(
                        itemName,
                        new Dictionary<string, List<string>>() { { "Statics", new List<string>() }, { "Sheets", new List<string>() } }
                    );
                }
                    
                files[itemName][file.Split('/')[^2]].Add(file);
            }

            var sheetConstants = new Dictionary<string, SpriteSheetInfo>();
            SpriteSheetInfo constants;
            foreach (var file in files)
            {
                sheetConstants.Clear();
                foreach (var texture in file.Value["Sheets"])
                {
                    if (_spriteSheetConstants.TryGetValue(texture.Split('/')[^1], out constants))
                        sheetConstants.Add(texture.Split('/')[^1].Split('-')[^1], constants);
                }

                outputList.Add(
                    file.Key,
                    new Models.TextureCollection(
                        _content,
                        file.Value["Statics"].ToDictionary(x => x.Split('/')[^1].Split('-')[^1], x => x),
                        file.Value["Sheets"].ToDictionary(x => x.Split('/')[^1].Split("-")[^1], x => x),
                        sheetConstants
                    )
                );
            }

            if (outputList.ContainsKey("Default"))
                foreach (var item in outputList)
                    item.Value.BlankItemTexture = outputList["Default"].GetIcon();

            var type = StringToType(masterPath.Split('/')[^1].ToLower());
            if (type != ItemType.Unrecognised)
                SortedItems.Add(type, outputList);

        }

        private void LoadBlockDirectory(string masterPath, string atlasName)
        {
            var paths = Directory.GetFiles(masterPath).Select(x => x.Replace("Content/", string.Empty).Replace(".xnb", string.Empty));
            var atlasInfo = GetAtlasInformation(atlasName);
            foreach (var path in paths)
            {
                var key = path.Split('\\')[^1];
                if (atlasInfo.AtlasKeys.Contains(key))
                    atlasInfo.Atlas.Add(key, _content.Load<Texture2D>(path));
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
            List<string> item;

            // 0: Name, 1: ItemType, 2: Description, 3: Attributes, 4:Type (more specific)
            Models.TextureCollection textures;
            for (int i = 0; i < readItems.Count; i++)
            {
                try
                {
                    item = readItems[i];
                    textures = (Items.ContainsKey(item[0])) ? Items[item[0]] : new Models.TextureCollection();
                    switch (StringToType(item[1]))
                    {
                        case (ItemType.Weapon):
                            items.Add(item[0], new Weapon(_game, textures, i, item[0], item[2], StringToType(item[1]), item[3]));
                            break;
                        case (ItemType.Consumable):
                            items.Add(item[0], new Consumable(_game, textures, i, item[0], item[2], StringToType(item[1]), item[4], item[3]));
                            break;
                        default:
                            items.Add(item[0], new Item(textures, i, item[0], item[2], StringToType(item[1]), item[3]));
                            break;
                    }

                    IdToName.Add(i, item[0]);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    continue;
                }
            }

            return items;

        }

        public void LoadSprites()
        {
            var path = "Content/Sprites";
            var sprites = Directory.GetDirectories(path);
            foreach(var sprite in sprites)
            {
                var textures = Directory.GetFiles(sprite)
                    .Select(x => x.Replace("Content/", string.Empty).Replace(".xnb", string.Empty))
                    .ToDictionary(x => x.Split('\\')[^1].Split('-')[0], x => x);

                var sheetConstants = new Dictionary<string, SpriteSheetInfo>();
                SpriteSheetInfo x;
                foreach (var kvp in textures)
                {
                    if (_spriteSheetConstants.TryGetValue(kvp.Value.Split('\\')[^1], out x))
                        sheetConstants.Add(kvp.Key, x);
                }
                    
                Sprites.Add(
                    sprite.Split("\\")[1],
                    new Models.TextureCollection(
                        _content,
                        textures.Where(x => !x.Value.Contains("-Sheet")).ToDictionary(x => x.Key, x => x.Value),
                        textures.Where(x => x.Value.Contains("-Sheet")).ToDictionary(x => x.Key, x => x.Value),
                        sheetConstants
                    )
                );
            }
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
                _ => ItemType.Unrecognised
            };
            return output;
        }

        private (List<string> AtlasKeys, Dictionary<string, Texture2D> Atlas) GetAtlasInformation(string atlasName)
        {
            switch (atlasName.ToLower())
            {
                default:
                    return (CommonBlocksKey, CommonBlockAtlas);
            }
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

        public Texture2D GetItemIcon(string itemName, bool nullAsMissing = true, ItemType type = ItemType.Item)
        {
            try
            {
                var texture = Items[itemName].GetIcon(nullAsMissing);
                if (texture.Name == null && !nullAsMissing)
                {
                    type = (itemName == "Default") ? type : _game.Items[itemName].Type;
                    texture = SortedItems[type].ToList()[0].Value.GetIcon(nullAsMissing);
                }
                return texture;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return nullAsMissing ? Null : Blank;
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

        public Texture2D GetBlock(int index, string name)
        {
            var info = GetAtlasInformation(name);
            return info.Atlas[info.AtlasKeys[index]];
        }

        public bool GetProjectileInfo(string name, out ProjectileInfo info) => _projectileInfo.TryGetValue(name, out info);
    }
}
