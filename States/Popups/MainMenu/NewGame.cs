using Bound.Controls;
using Bound.States.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;



namespace Bound.States
{
    public class NewGame : Popup
    {

        private List<Component> _components;
        private GraphicsDeviceManager _graphics;
        private SaveState _currentSave;

        public NewGame(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content, parent, graphics)
        {
        }

        public override void LoadContent()
        {
            var button = _game.Textures.Button;
            var font = _game.Textures.Font;
            var eigthWidth = Game1.ScreenWidth / 4;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bbWidth = (int)(eigthWidth * 2);
            var bbHeight = (int)(eigthHeight * 6);

            var background = new BorderedBox
            (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.BlanchedAlmond,
                    new Vector2(eigthWidth, eigthHeight),
                    0.6f,
                    bbWidth,
                    bbHeight
            );

            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.75f,
                },
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Continue",
                    Click = new EventHandler(Button_Apply_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.75f,
                },
            };

            var comp = _components[^1] as Button;
            float buttonHeight = _game.Textures.Button.Height * comp.Scale;
            float buttonWidth = _game.Textures.Button.Width * comp.Scale;

            //some of these numbers have been pulled straight out of my ass
            var buttonPosition = new Vector2((bbWidth - buttonWidth) / 2 - buttonWidth / 2 - 8 * comp.Scale, bbHeight - (buttonHeight + (15 * comp.Scale)));

            for (int i = 1 ; i < 3; i++)
            {
                comp = _components[i] as Button;
                comp.RelativePosition = buttonPosition;
                comp.xOffset = (int)(bbWidth / 2 - buttonWidth) * (i - 1);
            }

            for (int i = 1; i < 6; i++)
            {
                _components.Add(new SaveState(_game.Textures.BaseBackground, font)
                {
                    Text = "Save " + i.ToString(),
                    Click = new EventHandler(State_Choose_Click),
                    Layer = 0.8f

                });
                var ss = _components[^1] as SaveState;
                ss.LoadContent(_game, background, i - 1);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in _components)
                comp.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var comp in _components)
                comp.Update(gameTime);
        }


        #region Clicked Methods

        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }
        private void Button_Apply_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void State_Choose_Click(object sender, EventArgs e)
        {
            _currentSave = sender as SaveState;
        }

        #endregion
    }
}
