using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls
{
    public class SaveState : Component
    {
        #region Fields & Properties

        private List<Component> _components;
        private Texture2D _texture;
        private SpriteFont _font;
        private bool _isSelected;

        public string Text;
        public Vector2 Position;
        public float Layer;
        public EventHandler Click;
        private MouseState _previousMouse;
        private MouseState _currentMouse;
        public int Width;
        public int Height;

        public Color BorderColor
        {
            get
            {
                if (_isSelected)
                    return Color.White;
                return Color.Black;
            }
        }

        public Color PenColor
        {
            get
            {
                if (_isSelected)
                    return Color.Black;
                return Color.White;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        #endregion

        public SaveState(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(_font, Text, Position, PenColor);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            (_components[0] as BorderedBox).BorderColor = BorderColor;

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            if (mouseRectangle.Intersects(Rectangle))
            {
                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                    _isSelected = true;
                }
            }
        }

        public void LoadContent(Game1 _game, BorderedBox background, int index)
        {
            var textureScale = 0.6f;
            var fullScale = textureScale * Game1.ResScale;
            var gap = 200f * Game1.ResScale;

            Width = (int)(_font.MeasureString(Text).X + gap);
            Height = (int)(_font.MeasureString(Text).Y * 3);

            Position = new Vector2(background.Position.X + background.Width / 2 - Width / 2, background.Position.Y + background.Height / 5 - Height / 2 + (Height + 10 * Game1.ResScale) * index);

            _components = new List<Component>()
            {
                new BorderedBox
                (
                    _texture,
                    _game.GraphicsDevice,
                    Color.Wheat,
                    Position,
                    Layer,
                    Width,
                    Height
                ),
            };
        }
    }
}
