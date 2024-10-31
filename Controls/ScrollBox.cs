using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls
{
    public class ScrollBox : MultiChoice
    {
        #region Properties/Fields

        private SpriteFont _font;
        private List<Component> _components;

        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private Vector2 _textPosition;
        private Vector2 _valuePosition;
        private Vector2 _barPos;
        private int _barLength;
        private int _barHeight;
        private float _max;
        private int _cursorWidth;
        private BorderedBox _greenBar;
        private BorderedBox _cursor;
        private bool _isPressed;
        private string _symbol;

        public string Name;
        public string Text;
        public Vector2 Position;
        public float Layer;
        public float TextureScale;
        public int FullWidth;
        public int FullHeight;
        public Color PenColour;
        public string CurValue;
        public int Order;
        public int yOffset;

        //"curosor" refers to the black box which sets the current value. NOT the mouse curosr
        private Rectangle _cursorRectangle
        {
            get
            {
                return new Rectangle((int)_cursor.Position.X, (int)_cursor.Position.Y, _cursorWidth, _barHeight);
            }
        }

        public int ChosenValue
        {
            get
            {
                if (Char.IsNumber(CurValue, CurValue.Length - 1))
                    return int.Parse(CurValue);
                else
                    return int.Parse(CurValue.Substring(0, CurValue.Length - 1));
            }
        }

        public Vector2 CursorPositon
        {
            get
            {
                var x = new Vector2(_barPos.X + GreenBarLength - (_cursorWidth / 2), _barPos.Y) ;
                MathHelper.Clamp(x.X, _barPos.X, _barLength);
                return x ;
            }
        }

        public int GreenBarLength
        {
            get
            {
                var x = (ChosenValue == 0) ? 1f : (float)ChosenValue;
                return (int)((x / _max) * _barLength);
            }
        }

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        #endregion

        #region Constructor/Inherited Methods
        public ScrollBox(SpriteFont font, string name, float max, string symbol)
        {
            _font = font;
            _components = new List<Component>();
            _max = max;

            Name = name;
            TextureScale = 1f;
            PenColour = Color.Black;

            _symbol = symbol;

            CurValue = Game1.SettingsStates[name] + symbol;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (!string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(_font, Text, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }

            //Alligns value to the right side of the control
            var valuePos = new Vector2(_valuePosition.X - _font.MeasureString(CurValue).X, _valuePosition.Y);

            spriteBatch.DrawString(_font, CurValue, valuePos, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            //if it has just been clicked on or left click is still pressed
            if ((mouseRectangle.Intersects(_cursorRectangle) &&
                _currentMouse.LeftButton == ButtonState.Pressed && 
                _previousMouse.LeftButton == ButtonState.Released) ||
                _isPressed)
            {
                _isPressed = true;
            }
            if (_currentMouse.LeftButton == ButtonState.Released)
                _isPressed = false;

            if (_isPressed)
                FollowCursor();
        }


        //Sets the position and values of all children.
        public override void LoadContent(Game1 _game, BorderedBox background, float allignment)
        {
            var gap = 10f * Game1.ResScale;
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            TextureScale = 1f;

            _barLength = (int)(300 * Scale);

            var toAdd = (Game1.ScreenHeight == 720) ? 10 : 1.5f * Game1.ResScale; //I dislike 720p

            int longestName = 0;
            if (true)
                longestName = Game1.SettingsStates.Keys
                    .Aggregate(
                        0,
                        (a, c) => (a >= _font.MeasureString(c).X) ? a : (int)_font.MeasureString(c).X) + (int)_font.MeasureString(" ").X;
                        //" " accounts for that the Keys don't have a space 


            FullHeight = (int)((_font.MeasureString(Text).Y + (10)));

            FullWidth = (int)(longestName + gap + _barLength + 2 * gap + _font.MeasureString("100%").X + toAdd);

            Position = new Vector2
            (
                (background.Position.X + (allignment * Scale)),
                (background.Position.Y + (background.Height / 6) - (FullHeight / 2)) + (FullHeight + (10 * Scale)) * Order + (yOffset * Scale)
            );
            
            _textPosition = new Vector2(Position.X + 10, Position.Y + ((FullHeight - _font.MeasureString(Text).Y) / 2) );
            _valuePosition = new Vector2(Position.X + FullWidth - Game1.ResScale, Position.Y + (FullHeight- _font.MeasureString(CurValue).Y) / 2) ;

            _barHeight = (int)(FullHeight / 2);
            _barPos = new Vector2(10 + Position.X + longestName + gap, Position.Y + (FullHeight  / 4) + (2 * Game1.ResScale));

            _cursorWidth = (int)(15f * Game1.ResScale);
            _greenBar = new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.DarkGreen,
                    _barPos,
                    Layer + 0.02f,
                    GreenBarLength,
                    _barHeight
                );

            _cursor = new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.Black,
                    CursorPositon,
                    Layer + 0.03f,
                    _cursorWidth,
                    _barHeight
                );

            _components = new List<Component>
            {
                new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.White,
                    Position,
                    Layer - 0.01f,
                    FullWidth,
                    FullHeight
                ),
                new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.Red,
                    _barPos,
                    Layer + 0.01f,
                    _barLength,
                    _barHeight
                ),
                _cursor,
                _greenBar,
                
            };
        }

        #endregion

        #region Other Methods

        private void FollowCursor()
        {
            var x = MathHelper.Clamp(_currentMouse.X, _barPos.X, _barPos.X + _barLength);
            x = ((float)(x - _barPos.X) / (float)_barLength) * 100;
            x = (x > 99f) ? 100f : x; 
            CurValue = ((int)x).ToString() +  _symbol ;
            _greenBar.Width = GreenBarLength;
            _cursor.Position = CursorPositon;
        }

        #endregion
    }
}
