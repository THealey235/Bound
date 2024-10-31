using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bound.States;
using SharpDX.DirectWrite;

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

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        #endregion

        #region Properties

        public EventHandler Click;

        public SpriteEffects Effect;

        public bool Clicked { get; private set; }

        public float Layer { get; set; }

        public float TextureScale;

        public Vector2 RelativePosition; //Position relative to the parent

        public BorderedBox Parent;

        public int xOffset;

        public bool IsHovering
        {
            get
            {
                return _isHovering;
            }
            set
            {
                _isHovering = value;
            }
        }

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
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(_texture.Width * Scale), (int)(_texture.Height * Scale));
            }
        }

        public string Text;

        #endregion

        #region Methods

        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            TextureScale = 1f;

            PenColour = Color.Black;

            xOffset = 3;

            Effect = SpriteEffects.None;
        }

        public Button (Texture2D texture, SpriteFont font, BorderedBox parent) 
            : this(texture, font)
        {
            Parent = parent;
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.Gray;

            if (!_isHovering)
            {
                colour = Color.White;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Position.X) + ((Rectangle.Width - (_font.MeasureString(Text).X)) / 2) + (xOffset);
                var y = (Position.Y) + ((Rectangle.Height - (_font.MeasureString(Text).Y)) / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }

            if (Parent != null)
            {
                Position = new Vector2(Parent.Position.X + RelativePosition.X, Parent.Position.Y + RelativePosition.Y);
                spriteBatch.Draw(_texture, Position, null, colour, 0f, Vector2.Zero, Scale, Effect, Layer);
            }
            else
                spriteBatch.Draw(_texture, Position, null, colour, 0f, Vector2.Zero, Scale, Effect, Layer);
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
