using Bound.Controls;
using Bound.States.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public override void LoadContent()
        {
            colour = Color.White;

            var buttonTexture = _game.Button;
            var font = _game.Font;          

            var leftOffset = 30 + buttonTexture.Width / 2;
            var topOffset = Game1.ScreenHeight / 2 - buttonTexture.Height;
            //2f is base button scale
            var spacing = (buttonTexture.Height * ( Game1.ResScale * 2f) / 2) + (60 * Game1.ResScale);
            var layer = 0.5f;

            _components = new List<Component>()
            {
                new Button(buttonTexture, font)
                {
                    Text = "Load Game",
                    Position = new Vector2(leftOffset , topOffset),
                    Click = new EventHandler(Button_LoadGame_Clicked),
                    Layer = layer,
                    TextureScale = 2f,
                },
                new Button(buttonTexture, font)
                {
                    Text = "New Game",
                    Position = new Vector2(leftOffset , topOffset + (spacing)),
                    Click = new EventHandler(Button_NewGame_Clicked),
                    Layer = layer,
                    TextureScale = 2f,

                },
                new Button(buttonTexture, font)
                {
                    Text = "Settings",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 2)),
                    Click = new EventHandler(Button_Settings_Clicked),
                    TextureScale = 2f,
                    Layer = layer,
                },
                new Button(buttonTexture, font)
                {
                    Text = "Training",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 3)),
                    Click = new EventHandler(Button_Training_Clicked),
                    TextureScale = 2f,
                    Layer = layer,
                },
                new Button(buttonTexture, font)
                {
                    Text = "Quit",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 4)),
                    Click = new EventHandler(Button_Quit_Clicked),
                    TextureScale = 2f,
                    Layer = layer,
                },
            };
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

        public override void PostUpdate(GameTime gameTime)
        {
            
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

        private void Button_LoadGame_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_NewGame_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Settings_Clicked(object sender, EventArgs e)
        {
            Popups.Add(new Settings(_game, _content, this, _graphics));
            Popups[^1].LoadContent();
        }

        private void Button_Training_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Quit_Clicked(object sender, EventArgs e)
        {
            _game.Exit();
        }

        #endregion
    }
}
