using Bound.Controls;
using Bound.States.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.States
{
    public class MainMenu : State
    {
        #region Properties & Fields

        private List<Component> _components;

        private State _nextPopup;

        public Color colour;

        #endregion

        #region State Methods

        public MainMenu(Game1 game, ContentManager content) : base(game, content)
        {
            
        }

        public override void LoadContent()
        {
            colour = Color.White;

            var buttonTexture = _game.Button;
            var font = _game.SetSpriteFont();          

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
                },
                new Button(buttonTexture, font)
                {
                    Text = "New Game",
                    Position = new Vector2(leftOffset , topOffset + (spacing)),
                    Click = new EventHandler(Button_NewGame_Clicked),
                    Layer = layer,
                },
                new Button(buttonTexture, font)
                {
                    Text = "Settings",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 2)),
                    Click = new EventHandler(Button_Settings_Clicked),
                    Layer = layer,
                },
                new Button(buttonTexture, font)
                {
                    Text = "Training",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 3)),
                    Click = new EventHandler(Button_Training_Clicked),
                    Layer = layer,
                },
                new Button(buttonTexture, font)
                {
                    Text = "Quit",
                    Position = new Vector2(leftOffset , topOffset + (spacing * 4)),
                    Click = new EventHandler(Button_Quit_Clicked),
                    Layer = layer,
                },
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (_nextPopup != null)
            {
                _nextPopup.LoadContent();
                Popups.Append(_nextPopup);
            }

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

            //blur out previous things if there is a popup
            if (Popups.Count > 0)
                _game.GraphicsDevice.Clear(new Color(128, 128, 128, 128));

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
            Popups.Add(new Settings(_game, _content, this));
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
