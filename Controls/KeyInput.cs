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
        private KeyValuePair<string, Keys> _keys;
        private int _longestInput;
        private Vector2 _textPosition;
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
        private MouseState _previousMouse;
        private MouseState _currentMouse;

        public Color PenColour;
        public int FullWidth;
        public int FullHeight;
        public float TextureScale;
        public float Layer;
        public Vector2 Position;
        public int Order;
        public bool IsClicked;
        public Keys NewKey = Keys.None;
        public string NewMouse;

        public Keys Key
        {
            get
            {
                if (NewKey != Keys.None)
                    return NewKey;
                if (NewMouse != null)
                return _keys.Value;
            }
        }

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        public KeyInput(SpriteFont font, KeyValuePair<string, Keys> key, int longestInput)
        { 
            _font = font;
            _components = new List<Component>();

            PenColour = Color.Black;

            _keys = key;

            TextureScale = 1f;
            
            _longestInput = longestInput;
           
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in _components)
                comp.Draw(gameTime, spriteBatch);

            if (!string.IsNullOrEmpty(_keys.Key))
            {
                spriteBatch.DrawString(_font, _keys.Key, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
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
                    Text = Key.ToString(),
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
                if(_previousKey.GetPressedKeys()[0] == Keys.None &&
                    _currentKey.GetPressedKeys()[0] != Keys.None)
                {
                     NewKey = _currentKey.GetPressedKeys()[0];
                }
                else if (_previousMouse.LeftButton == ButtonState.Released && 
                    _previousMouse.RightButton == ButtonState.Released && 
                    _previousMouse.MiddleButton == ButtonState.Released)                    
                {
                    if (_currentMouse.LeftButton == ButtonState.Pressed)
                        NewMouse = "Left";
                    else if (_currentMouse.RightButton == ButtonState.Pressed)
                        NewMouse = "Right";
                    else if (_currentMouse.MiddleButton == ButtonState.Pressed)
                        NewMouse = "Middle";

                }
            }
        }

        private void Clicked(object sender, EventArgs e)
        {
            IsClicked = true;
        }
        
    }
}
