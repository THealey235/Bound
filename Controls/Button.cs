﻿using Microsoft.Xna.Framework.Graphics;
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
    public class Button : ChoiceBox
    {
        #region Fields

        private MouseState _currentMouse;

        protected SpriteFont _font;

        protected bool _isHovering;

        private MouseState _previousMouse;

        protected Texture2D _texture;

        private Vector2 _position;

        private Color _hoveringColour = Color.Gray;

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

        //Whether to centre its position around the player's camera
        public bool ToCenter = true;

        public Color ButtonColour = Color.White;

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

        public Vector2 Position
        {
            get
            {
                if (Parent == null)
                    return _position;
                else
                    return new Vector2(Parent.Position.X + RelativePosition.X + xOffset, Parent.Position.Y + RelativePosition.Y);
            }
            set
            {
                _position = value;
            }
        }

        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(_texture.Width * Scale), (int)(_texture.Height * Scale));
            }
        }

        public Color Colour
        {
            get
            {
                return ButtonColour;
            }
            set
            {
                ButtonColour = value;
                _hoveringColour = Game1.BlendColors(value, Color.Gray, 0.75f);
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

            Effect = SpriteEffects.None;
        }

        public Button (Texture2D texture, SpriteFont font, BorderedBox parent) 
            : this(texture, font)
        {
            Parent = parent;
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = _hoveringColour;

            if (!_isHovering)
            {
                colour = ButtonColour;
            }

            spriteBatch.Draw(_texture, Position, null, colour, 0f, Vector2.Zero, Scale, Effect, Layer);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Position.X) + ((Rectangle.Width - (_font.MeasureString(Text).X)) / 2);
                var y = (Position.Y) + ((Rectangle.Height - (_font.MeasureString(Text).Y)) / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
            if (ToCenter)
            {
                mouseRectangle.X += (int)Game1.V2Transform.X;
                mouseRectangle.Y += (int)Game1.V2Transform.Y;
            }

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

        public override void LoadContent(Game1 game, BorderedBox background, float allignment)
        {
            //No content to load
        }

        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
        }

        #endregion
    }
}
