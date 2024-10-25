using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls
{
    public class Button : Component
    {
        #region Fields

        private MouseState _currentMouse;

        private SpriteFont _font;

        private bool _isHovering;

        private MouseState _previousMouse;

        private Texture2D _texture;

        private float _scale
        {
            get
            {
                return 2f * Game1.ResScale;
            }
        }

        #endregion

        #region Properties

        public EventHandler Click;

        public bool Clicked { get; private set; }

        public float Layer { get; set; }

        public Vector2 Origin
        {
            get
            {
                return new Vector2(_texture.Width / 2, _texture.Height / 2);
            }
        }

        public Vector2 ScaledOrigin
        {
            get
            {
                return new Vector2((_texture.Width * 3) / 2, (_texture.Height * 3) / 2);
            }
        }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));
            }
        }

        public string Text;

        #endregion

        #region Methods

        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.Black;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Position.X) + ((Rectangle.Width - (_font.MeasureString(Text).X * _scale)) / 2);
                var y = (Position.Y) + ((Rectangle.Height - (_font.MeasureString(Text).Y * _scale)) / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour, 0f, Vector2.Zero, _scale, SpriteEffects.None, Layer + 0.01f);
            }

            spriteBatch.Draw(_texture, Position, null, colour, 0f, Vector2.Zero, _scale, SpriteEffects.None, Layer);
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}
