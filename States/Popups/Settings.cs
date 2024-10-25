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

        public State Parent;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public Settings(Game1 game, ContentManager content, State parent) : base(game, content)
        {
            Parent = parent;
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;

            _components = new List<Component>()
            {
                new PopupWindow
                (
                    _game.BaseBackground,
                    Color.SaddleBrown,
                    new Vector2 (eigthWidth, eigthHeight),
                    0.6f,
                    (int) (eigthWidth * 6),
                    (int) (eigthHeight * 6)
                )
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();

            if (_currentKeys.IsKeyDown(Keys.Escape) && _previousKeys.IsKeyUp(Keys.Escape))
                Parent.Popups.Remove(this);

        }
        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }
    }
}
