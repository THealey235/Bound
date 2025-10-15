using Bound.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Input;



namespace Bound.States.Popups.Game
{
    public class Options : Popup
    {
        private List<Component> _components;
        private bool _enableKey;

        public float Layer;

        public Options(Game1 game, ContentManager content, State parent) : base(game, content, parent)
        {
            Name = Game1.Names.GameOptions;
            Layer = 0.8f;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent()
        {
            var scale = 1f;
            var button = _game.Textures.Buttons["Blank"];
            var font = _game.Textures.Font;
            var leftOffset = 10 * Game1.ResScale;
            var spacing = (button.Height * scale + 15) * Game1.ResScale;
            var topOffset = (int)((Game1.ScreenHeight - (spacing * 4 - 15 * Game1.ResScale)) / 2);
            var buttonColour = Color.SandyBrown;

            _components = new List<Component>()
            {

                new Button(button, font)
                {
                    Text = "Inventory",
                    Click = new EventHandler(Button_Inventory_Clicked),
                    Layer = Layer,
                    TextureScale = scale,
                    Position = new Vector2(leftOffset, topOffset) + Game1.V2Transform,
                    ToCenter = true,
                    Colour = buttonColour,
                },
                new Button(button, font)
                {
                    Text = "Stats",
                    Click = new EventHandler(Button_Stats_Clicked),
                    Layer = Layer,
                    TextureScale = scale,
                    Position = new Vector2(leftOffset, topOffset + spacing) + Game1.V2Transform,
                    ToCenter = true,
                    Colour = buttonColour,
                },
                new Button(button, font)
                {
                    Text = "Settings",
                    Click = new EventHandler(Button_Settings_Clicked),
                    Layer = Layer,
                    TextureScale = scale,
                    Position = new Vector2(leftOffset, topOffset + spacing * 2) + Game1.V2Transform,
                    ToCenter = true,
                    Colour = buttonColour,
                },
                new Button(button, font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer,
                    TextureScale = scale,
                    Position = new Vector2(leftOffset, topOffset + spacing * 3) + Game1.V2Transform,
                    ToCenter = true,
                    Colour = buttonColour,
                }
            };
        }


        #region Clicked Methods

        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Settings_Clicked(object sender, EventArgs e)
        {
            var settings = new Settings(_game, _content, Parent, _game.GraphicsManager);
            Parent.Popups.Add(settings);
            settings.LoadContent();
            settings.LoadMenuButton();
        }

        private void Button_Inventory_Clicked(object sender, EventArgs e)
        {
            var inventory = new InventoryMenu(_game, _content, Parent);
            Parent.Popups.Add(inventory);
            inventory.LoadContent();
        }

        private void Button_Stats_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Add(new Stats(_game, _content, Parent));
            Parent.Popups[^1].LoadContent();
        }

        #endregion

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            if (Parent.Popups[^1] != this)
                return;

            foreach (var component in _components)
                component.Update(gameTime);

            if (_enableKey)
            {
                if (_game.PlayerKeys.IsPressed("Menu", false) ||
                   (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape) && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape)))
                    Button_Discard_Clicked(new object(), new EventArgs());
            }
            else
            {
                var key = _game.PlayerKeys.GetKey(_game.PlayerKeys.Keys["Menu"]);

                if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(key))
                    _enableKey = true;
            }
        }
    }
}
