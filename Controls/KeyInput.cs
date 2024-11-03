using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Bound.Controls
{
    public class KeyInput : MultiChoice
    {
        private List<Component> _components;
        private BorderedBox _borderedBox;
        private SpriteFont _font;
        private int _longestInput;
        private Vector2 _textPosition;
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
        private MouseState _previousMouse;
        private MouseState _currentMouse;

        public KeyValuePair<string, string> Keys;
        public Color PenColour;
        public int FullWidth;
        public int FullHeight;
        public float TextureScale;
        public float Layer;
        public Vector2 Position;
        public int Order;
        public bool IsClicked;
        public string NewKey;

        public string Key
        {
            get
            {
                if (NewKey != null)
                    return NewKey;
                return Keys.Value;
            }
        }

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        public KeyInput(SpriteFont font, KeyValuePair<string, string> key, int longestInput)
        { 
            _font = font;
            _components = new List<Component>();

            PenColour = Color.Black;

            Keys = key;

            TextureScale = 1f;
            
            _longestInput = longestInput;
           
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in _components)
                comp.Draw(gameTime, spriteBatch);

            if (!string.IsNullOrEmpty(Keys.Key))
            {
                spriteBatch.DrawString(_font, Keys.Key, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }
        }

        public override void LoadContent(Game1 game, BorderedBox background, float allignment)
        {
            TextureScale = 1.1f;

            var gap = 10f * Game1.ResScale;

            var boxLength = _font.MeasureString(" ").X; //any 3 chars have the same length

            FullWidth = (int)(10 + _longestInput + gap + game.Button.Width * Scale + gap);
            FullHeight = (int)(_font.MeasureString("T").Y + (10));

            Position = new Vector2
            (
                background.Position.X + (allignment),
                (background.Position.Y + (background.Height / 6) - (FullHeight / 2)) + (FullHeight + (10 * Scale)) * Order
            );

            var boxHeight = (int)(game.Button.Height * Scale);

            _textPosition = new Vector2(Position.X + 10, Position.Y + 5);

            _borderedBox = new BorderedBox
                (
                    game.BaseBackground,
                    game.GraphicsDevice,
                    Color.White,
                    Position,
                    Layer,
                    FullWidth,
                    FullHeight
                );

            _components = new List<Component>
            {
                _borderedBox,
                new Button(game.Button, _font, _borderedBox)
                {
                    Text = Key,
                    Click = new EventHandler(Clicked),
                    Layer = Layer + 0.1f,
                    RelativePosition = new Vector2(10 + _longestInput, (FullHeight - boxHeight) / 2),
                    TextureScale = TextureScale,
                    PenColour = Color.Red
                }
            };
        }


        public override void Update(GameTime gameTime)
        {

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            foreach(var component in _components) 
                component.Update(gameTime);


            if (IsClicked)
            {
                (_components[1] as Button).IsHovering = true;

                if (_previousKey.GetPressedKeys().Length == 1 &&
                    _currentKey.GetPressedKeys().Length > 0)
                {
                    if (_currentKey.GetPressedKeys()[0].ToString() != "Pause")
                        ChangeKey(_currentKey.GetPressedKeys()[0].ToString());
                    if (_currentKey.GetPressedKeys().Length > 1)
                        ChangeKey(_currentKey.GetPressedKeys()[1].ToString());
                }
                else if (_previousMouse.LeftButton == ButtonState.Released &&
                    _previousMouse.RightButton == ButtonState.Released &&
                    _previousMouse.MiddleButton == ButtonState.Released)
                {
                    if (_currentMouse.LeftButton == ButtonState.Pressed)
                        ChangeKey("M1");
                    else if (_currentMouse.RightButton == ButtonState.Pressed)
                        ChangeKey("M2");
                    else if (_currentMouse.MiddleButton == ButtonState.Pressed)
                        ChangeKey("M3");
                    else if (_currentMouse.XButton1 == ButtonState.Pressed)
                        ChangeKey("M4");
                    else if (_currentMouse.XButton2 == ButtonState.Pressed)
                        ChangeKey("M5");
                }
            }
        }

        private void Clicked(object sender, EventArgs e)
        {
            IsClicked = true;
        }
        
        private void ChangeKey(string value)
        {
            NewKey = value;
            IsClicked = false;
            (_components[1] as Button).Text = value;
        }
    }
}
