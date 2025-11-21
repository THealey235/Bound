using Bound.Controls;
using Bound.States.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
namespace Bound.States
{
    public class MainMenu : State
    {
        #region Properties & Fields

        private List<Component> _components;

        private GraphicsDeviceManager _graphics;

        public Color colour;



        #endregion

        #region Inherited Methods

        public MainMenu(Game1 game, ContentManager content, GraphicsDeviceManager graphics) : base(game, content)
        {
            _graphics = graphics;
            Name = Game1.Names.MainMenu;
        }

        public override void LoadContent()
        {
            colour = Color.White;

            var buttonTexture = _game.Textures.Button;
            var font = _game.Textures.Font;          

            var leftOffset = 30 + buttonTexture.Width / 2;
            var topOffset = Game1.ScreenHeight / 2 - buttonTexture.Height;
            var textureScale = 1f;
            var spacing = (buttonTexture.Height * ( Game1.ResScale * textureScale) / 2) + (30 * Game1.ResScale * textureScale);
            var layer = 0.5f;

            _components = new List<Component>()
            {
                new Button(buttonTexture, font)
                {
                    Text = "Saves",
                    Position = new Vector2(leftOffset , topOffset + (spacing)),
                    Click = new EventHandler(Button_Saves_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    ToCenter = false
                },
                new Button(buttonTexture, font)
                {
                    Text = "Settings",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 2)),
                    Click = new EventHandler(Button_Settings_Clicked),
                    TextureScale = textureScale,
                    Layer = layer,
                    ToCenter = false
                },
                new Button(buttonTexture, font)
                {
                    Text = "Quit",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 3)),
                    Click = new EventHandler(Button_Quit_Clicked),
                    TextureScale = textureScale,
                    ToCenter = false,
                    Layer = layer,
                },
            };

            if (_game.RecentSave != -1)
                _components.Add(new Button(buttonTexture, font)
                {
                    Text = "Continue",
                    Position = new Vector2(leftOffset, topOffset),
                    Click = new EventHandler(Button_Continue_Clicked),
                    ToCenter = false,
                    Layer = layer,
                    TextureScale = textureScale,
                });
        }

        public override void Update(GameTime gameTime)
        {
            if (Popups.Count == 0)
            {
                foreach (var component in _components)
                    component.Update(gameTime);
            }
            else
            {
                Popups[^1].Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var state in Popups)
                state.Draw(gameTime, spriteBatch);

        }

        #endregion

        #region Clicked Methods

        private void Button_Continue_Clicked(object sender, EventArgs e)
        {
            _game.ChangeState(_game.SavesManager.GetState(_game.RecentSave, _game, _content, _graphics));
        }

        private void Button_Saves_Clicked(object sender, EventArgs e)
        {
            Popups.Add(new SavesMenu(_game, _content, this));
            Popups[^1].LoadContent();
        }

        private void Button_Settings_Clicked(object sender, EventArgs e)
        {
            Popups.Add(new Settings(_game, _content, this, _graphics));
            Popups[^1].LoadContent();
        }

        private void Button_Quit_Clicked(object sender, EventArgs e)
        {
            _game.SavesManager.UploadAll();
            _game.Exit();
        }

        #endregion
    }
}
