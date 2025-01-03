using Bound.Models.Items;
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
        private ContentManager _content;

        public Texture2D Button;
        public Texture2D BaseBackground;
        public Texture2D RedX;
        public Texture2D ArrowLeft;
        public Texture2D Plus;
        public Texture2D PlayButton;
        public Texture2D TrashCan;
        public Texture2D Null;
        public Texture2D PlayerStatic;

        public Dictionary<string, Texture2D> Items;
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

        public Textures(ContentManager content)
        {
            Items = new Dictionary<string, Texture2D>();

            _content = content;

            Null = content.Load<Texture2D>("MissingTexture");
            Button = content.Load<Texture2D>("Controls/Buttons/Button1");
            Fonts = new List<SpriteFont>()
            {
                content.Load<SpriteFont>("Fonts/JX-720"),
                content.Load<SpriteFont>("Fonts/JX-900"),
                content.Load<SpriteFont>("Fonts/JX-1080"),
                content.Load<SpriteFont>("Fonts/JX-1440"),
                content.Load<SpriteFont>("Fonts/JX-2160"),
            };

            BaseBackground = content.Load<Texture2D>("Backgrounds/BaseBackground");
            RedX = content.Load<Texture2D>("Controls/Icos/RedX");
            ArrowLeft = content.Load<Texture2D>("Controls/Icos/ArrowLeft");
            Plus = content.Load<Texture2D>("Controls/Icos/Plus");
            PlayButton = content.Load<Texture2D>("Controls/Icos/PlayButton");
            TrashCan = content.Load<Texture2D>("Controls/Icos/TrashCan");

            Buttons = new Dictionary<string, Texture2D>
            {
                { "B&W", content.Load<Texture2D>("Controls/Buttons/BW")}
            };

            PlayerStatic = content.Load<Texture2D>("Player/Player1Static");
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
    }
}
