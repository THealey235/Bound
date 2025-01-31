using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace Bound.Controls.Settings
{
    public class KeyInput : MultiChoice
    //_blackList.Contains(key) || key.Contains("NumPad") || key.Contains("Media") || key.Contains("Browser") || key.Contains("Chat")
    {
        private static List<string> _blackList = new List<string>()
        {
            "Num", "Media", "Browser", "Chat", "EraseEof", "Escape", "Exsel", "Ime", "Launch", "Windows", "OemAuto", "OemEnlW", "ProcessKey", "Volume", "Sleep", "Subtract"
            //not as if you'll get far enought to use Sleep, also i picked OemMinus over subtract
        };

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

            TextureScale = 0.35f;

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

            var gap = 5f * Game1.ResScale;

            var boxLength = _font.MeasureString(" ").X; //any 3 chars have the same length

            FullWidth = (int)(5 * Game1.ResScale + _longestInput + gap + game.Textures.Button.Width * Scale + gap);
            FullHeight = (int)(game.Textures.Button.Height * Scale + 10 * Scale);

            Position = new Vector2
            (
                background.Position.X + allignment,
                background.Position.Y + background.Height / 6 - FullHeight / 2 + (FullHeight + 10 * Scale) * Order
            );

            var boxHeight = (int)(game.Textures.Button.Height * Scale);

            _textPosition = new Vector2(Position.X + 5 * Game1.ResScale, Position.Y + FullHeight / 2 - (_font.MeasureString("Y").Y / 2));

            _borderedBox = new BorderedBox
                (
                    game.Textures.BaseBackground,
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
                new Button(game.Textures.Button, _font, _borderedBox)
                {
                    Text = Key,
                    Click = new EventHandler(Clicked),
                    Layer = Layer + 0.1f,
                    RelativePosition = new Vector2(10 * Game1.ResScale + _longestInput, (FullHeight - boxHeight) / 2),
                    TextureScale = TextureScale,
                    PenColour = Color.DarkRed,
                }
            };
        }


        public override void Update(GameTime gameTime)
        {

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            foreach (var component in _components)
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
            value = FormatKey(value);
            if (value == "invalid")
                return;

            NewKey = value;
            IsClicked = false;
            (_components[1] as Button).Text = value;
        }

        private string FormatKey(string key)
        {
            if ((from item in _blackList
                where key.Contains(item)
                select item).ToArray().Length > 0) //if the key contains any blacklisted phrase stop it
                //this is not efficient. TO DO: Fix this.
            {
                return "invalid";
            }
            if (Input.SpecialKeyMap.ContainsKey(key))
            {
                key = Input.SpecialKeyMap[key];
            }

            return key;
        }
    }
}
