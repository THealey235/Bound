using Bound.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.States.Popups
{
    public class Settings : State
    {
        private List<Component> _components;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public State Parent;
        public Dictionary<string, string> SettingsStates;

        public Settings(Game1 game, ContentManager content, State parent) : base(game, content)
        {
            Parent = parent;
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var texture = _game.BaseBackground;
            var bbWidth = (int)(eigthWidth * 4);
            var bbHeight = (int)(eigthHeight * 4);
            var background = new BorderedBox
                (
                    texture,
                    _game.GraphicsDevice,
                    Color.BlanchedAlmond,
                    new Vector2(eigthWidth * 2, eigthHeight * 2),
                    0.6f,
                    bbWidth,
                    bbHeight
                );

            int resBoxIndex = Game1.ScreenHeight switch
                {
                    720 => 0,
                    1080 => 1,
                    2560 => 2,
                    2160 => 3,
                    _ => -1
                };

            _components = new List<Component>()
            {
                background,
                new Button(_game.Button, _game.Font, background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                    xOffset = 2
                },
                new Button(_game.Button, _game.Font, background)
                {
                    Text = "Apply",
                    Click = new EventHandler(Button_Apply_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                    xOffset = (int)(2 * Game1.ResScale),
                },
                new MultiChoiceBox(texture, _game.ArrowLeft, _game.Font, resBoxIndex)
                {
                    Text = "Resolution",
                    Choices = new List<string>()
                    {
                        "1280x720",
                        "1920x1080",
                        "2560x1440",
                        "3840x2160"
                    },
                    Layer = 0.8f,
                },
            };

            Button comp;

            comp = _components[1] as Button;
            float buttonHeight = _game.Button.Height * comp.Scale;
            float buttonWidth = _game.Button.Width * comp.Scale;
            //possibly spaghetti code
            comp.CustomPosition = new Vector2(((bbWidth - buttonWidth) / 2) - (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

            comp = _components[2] as Button;
            comp.CustomPosition = new Vector2(((bbWidth - buttonWidth) / 2) + (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

            for (int i = 3; i < 4; i++)
            {
                var mmcb = _components[i] as MultiChoiceBox;
                mmcb.LoadContent(_game, background, i - 3);
            }

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        #region Other Methods
        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }
        private void Button_Apply_Clicked(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        private void ApplyChanges()
        {
            
        }
        #endregion
    }
}
