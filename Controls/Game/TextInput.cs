using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls.Game
{
    public class TextInput : Component
    {
        #region Properties & Fields

        private SpriteFont _font;
        private List<Component> _components;
        private BorderedBox _textInputBox;

        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private KeyboardState _currentKeyboard;
        private KeyboardState _previousKeyboard;

        public Color PenColour;
        public Color TextColour;
        public Color BackColour;
        public Color BorderColour;
        public string Name;
        public string Text;
        public Vector2 Position;
        public float Layer;
        public float TextureScale;
        public bool IsTyping;
        private bool _isHovering;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)_textInputBox.Position.X, (int)_textInputBox.Position.Y, _textInputBox.Width, _textInputBox.Height);
            }
        }
        private Vector2 _textPosition
        {
            get
            {
                return new Vector2(_textInputBox.Position.X + _textInputBox.Width / 2 - _font.MeasureString(Text).X / 2, _textInputBox.Position.Y + (5f * TextureScale * Game1.ResScale));
            }
        }
        public bool IsHovering
        {
            set
            {
                if (_isHovering != value)
                    if (value)
                    {
                        _textInputBox.BorderColor = Color.Black;
                        _textInputBox.Colour = Color.White;
                        TextColour = Color.Black;
                    }
                    else
                    {
                        _textInputBox.BorderColor = Color.White;
                        _textInputBox.Colour = Color.Black;
                        TextColour = Color.White;
                    }

                _isHovering = value;
            }
        }

        #endregion

        #region Inherited

        public TextInput(SpriteFont font, string name)
        {
            _font = font;
            PenColour = TextColour = Color.Black;
            Name = name;
            BackColour = Color.White;
            BorderColour = Color.Black;
            Text = "";
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var c in _components)
                c.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(_font, Name, Position, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.05f);
            if (Text != null)
            {
                spriteBatch.DrawString(_font, Text, _textPosition, TextColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.02f); 
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouse = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            var hovering = false;
            CheckIfTyping(mouse);


            if (IsTyping)
            {
                _previousKeyboard = _currentKeyboard;
                _currentKeyboard = Keyboard.GetState();
                hovering = true;

                AppendLetter();
            }

            IsHovering = hovering;

        }


        public void LoadContent(Game1 game, Vector2 center)
        {
            var scale = TextureScale * Game1.ResScale;
            var width = (int)(_font.MeasureString(Name).X + _font.MeasureString("tenletters  ").X + (5f * scale) + (15f * scale) * 2);
            var height = (int)(_font.MeasureString(Name).Y);

            Position = new Vector2(center.X - (width / 2), center.Y - (height / 2));

            _textInputBox = new BorderedBox
                (
                    game.Textures.BaseBackground,
                    game.GraphicsDevice,
                    BackColour,
                    new Vector2(Position.X + _font.MeasureString(Name).X + (5f * scale) + (15f * scale), Position.Y),
                    Layer + 0.01f,
                    (int)(_font.MeasureString("tenletters  ").X + (10f * scale)),
                    height,
                    BorderColour
                );

            _components = new List<Component>()
            {
                _textInputBox,
            };
        }

        #endregion

        #region Other Methods

        private void CheckIfTyping(Rectangle mouse) 
        {
            if (_previousMouse.LeftButton == ButtonState.Released && _currentMouse.LeftButton == ButtonState.Pressed)
            {
                if(mouse.Intersects(Rectangle))
                    IsTyping = true;
                else
                    IsTyping = false;
            }

        }

        private void AppendLetter()
        {
            if (_currentKeyboard.GetPressedKeys().Length > 0)
            {
                var letter = _currentKeyboard.GetPressedKeys()[^1];

                if (_currentKeyboard.GetPressedKeys().Contains(Keys.Back) && !_previousKeyboard.GetPressedKeys().Contains(Keys.Back) && Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                else if (_currentKeyboard.GetPressedKeys().Contains(Keys.Enter))
                    IsTyping = false;
                else if (Text.Length < 10)
                {
                    if (_previousKeyboard.GetPressedKeys().Length > 0)
                    {
                        var prevKey = _previousKeyboard.GetPressedKeys()[^1];
                        if (prevKey == letter)
                            return;
                    }

                    if (letter == Keys.Space)
                        Text += " ";
                    else if (letter.ToString().Length == 1)
                        Text += letter.ToString();
                }
            }
        }

        #endregion
    }
}
