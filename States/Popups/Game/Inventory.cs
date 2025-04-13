using Bound.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Bound.Managers;
using Bound.Controls.Game;

namespace Bound.States.Popups.Game
{
    public class Inventory : Popup
    {
        private List<Component> _components;
        public BorderedBox _background;
        private List<(string Text, Vector2 Position)> _playerStats = new List<(string Text, Vector2 Position)>();
        private List<InventorySlot> _armour;
        private List<InventorySlot> _accessories;
        private List<InventorySlot> _hotbar;
        private List<InventorySlot> _skills;
        private List<(string Text, Vector2 Position)> _headings;
        private Vector2 _playerPosition;

        public float Layer;

        public Inventory(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.InventoryWindow;
            Layer = 0.79f;

            var bgWidth = Game1.ScreenWidth / 8 * 3;
            _headings = new List<(string Text, Vector2 Position)>()
            {
                ("Armour",  new Vector2(55 * Game1.ResScale, 5 * Game1.ResScale)),
                ("Accessories",  new Vector2(137 * Game1.ResScale, 5 * Game1.ResScale)),
                ("Hotbar",  new Vector2((bgWidth - _game.Textures.Font.MeasureString("Hotbar").X) / 2, 148 * Game1.ResScale)),
                ("Skills",  new Vector2((bgWidth - _game.Textures.Font.MeasureString("Skills").X) / 2, 190 * Game1.ResScale)),
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

           foreach (var pair in _headings)
                spriteBatch.DrawString(_game.Textures.Font, pair.Text, pair.Position + _background.Position, Game1.MenuColorPalette[2], 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);

            spriteBatch.Draw(_game.Textures.PlayerStatic, _playerPosition, null, Color.White, 0f, Vector2.Zero, 1.5f * Game1.ResScale, SpriteEffects.FlipHorizontally, Layer + 0.001f);
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bgPos = new Vector2(Game1.ScreenWidth / 2 + Game1.V2Transform.X - (eigthWidth * 3 / 2), eigthHeight + Game1.V2Transform.Y);

            _background = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[0],
                bgPos,
                Layer,
                eigthWidth * 3,
                eigthHeight * 6
            );

            _components = new List<Component>()
            {
                _background,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2((_background.Width - _game.Textures.Button.Width * Game1.ResScale * 0.75f) / 2, (_background.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = _background
                },
            };

            SetCustomiseButtons(_background);

        }


        private void SetCustomiseButtons(BorderedBox background)
        {
            var buttonTexture = _game.Textures.EmptyBox;
            var textureScale = 0.75f;
            var scale = textureScale * Game1.ResScale;
            var font = _game.Textures.Font;
            var layer = Layer + 0.001f;
            var ySpacing = (buttonTexture.Height * textureScale + 5) * Game1.ResScale;
            var xSpacing = (int)((buttonTexture.Width * textureScale + 5) * Game1.ResScale);

            var x = background.Width / 8;
            var y = background.Height / 12;

            _armour = new List<InventorySlot>();

            var armourIDs = new Dictionary<string, Textures.ItemType>() 
            { 
                {"headSlot", Textures.ItemType.HeadGear },
                {"chestSlot", Textures.ItemType.ChestArmour },
                {"legSlot", Textures.ItemType.LegArmour },
                {"feetSlot", Textures.ItemType.Footwear },
            };

            _playerPosition = new Vector2(background.Position.X + (x + Game1.ResScale * 40), background.Position.Y + (y + Game1.ResScale * 25));

            var acc = 0;
            foreach (var kvp in armourIDs)
            {
                _armour.Add(new InventorySlot(_game, font, kvp.Key, "Default", kvp.Value)
                {
                    Click = new EventHandler(Button_Armour_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x, y + ySpacing * acc),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
                acc++;
            }

            x *= 5;
            acc = 0;
            _accessories = new List<InventorySlot>();
            for (int i = 0; i < 8; i++)
            {
                if (i == 4) { acc = 0; x += xSpacing; }

                _accessories.Add(new InventorySlot(_game, font, "accessory", "Default", Textures.ItemType.Accessory)
                {
                    Click = new EventHandler(Button_Accessory_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x, y + ySpacing * acc),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });

                acc++;
            }

            var hbSlots = 3;
            x = (int)((background.Width - ((hbSlots - 1) * (xSpacing) + (buttonTexture.Width * scale))) / 2);
            y = (int)(background.Height * (3f / 5f));
            _hotbar = new List<InventorySlot>();
            for (int i = 0; i < hbSlots; i++)
            {
                _hotbar.Add(new InventorySlot(_game, font, "hotbar", "Default", Textures.ItemType.Item)
                {
                    Click = new EventHandler(Button_Hotbar_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x + xSpacing * i, y),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
            }

            _skills = new List<InventorySlot>();
            var skillSlots = 3;
            x = (int)((background.Width - ((skillSlots - 1) * (xSpacing) + (buttonTexture.Width * scale))) / 2);
            y = (int)(background.Height * (4f / 5f) - 10 * Game1.ResScale);
            for (int i = 0; i < skillSlots; i++)
            {
                _skills.Add(new InventorySlot(_game, font, "skill", "Default", Textures.ItemType.Skill)
                {
                    Click = new EventHandler(Button_Skills_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x + xSpacing * i, y),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
            }

            _components.AddRange(_armour);
            _components.AddRange(_accessories);
            _components.AddRange(_hotbar);
            _components.AddRange(_skills);
        }

        
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape)
                && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape))
                Button_Discard_Clicked(new object(), new EventArgs());

        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        #region Clicked Handlers
        private void Button_Discard_Clicked(object sender, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Armour_Clicked(object sender, EventArgs eventArgs)
        {
            var button = sender as InventorySlot;
        }

        private void Button_Accessory_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
        }
        private void Button_Hotbar_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
        }
        private void Button_Skills_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
        }

        #endregion

    }
}
