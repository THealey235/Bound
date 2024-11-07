using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls
{
    public class SaveInterface : Component
    {
        #region Fields & Properties

        private List<Component> _components;
        private Texture2D _texture;
        private SpriteFont _font;
        private BorderedBox _background;
        private Game1 _game;
        private int _saveNumber;

        private float _plusScale;
        private Texture2D _plusTexture;
        private Color _plusColour;
        private Vector2 _plusPosition;
        private int _index;

        public string Text;
        public Vector2 Position;
        public float Layer;
        public EventHandler Click;
        private MouseState _previousMouse;
        private MouseState _currentMouse;
        public int Width;
        public int Height;
        public Color BorderColor = Color.Black;
        public bool IsEmpty;
        public Color PenColor = Color.White;

        public Rectangle Rectangle
        {
            get
            {
                if (IsEmpty)
                    return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                //TODO: Change this to a list of rectangles (?) and have the delete and continue buttons
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        #endregion

        public SaveInterface(Texture2D texture, SpriteFont font, Game1 game)
        {
            _texture = texture;
            _font = font;
            _game = game;

            //TODO: Change once i implement saves
            IsEmpty = true;

            _plusColour = Color.Gray;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (IsEmpty)
                spriteBatch.Draw(_plusTexture, _plusPosition, null, _plusColour, 0f, Vector2.Zero, _plusScale, SpriteEffects.None, Layer + 0.09f);

            spriteBatch.DrawString(_font, Text, Position, PenColor);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);


            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            _plusColour = Color.Gray;
            _background.Colour = Color.Wheat;
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            if (mouseRectangle.Intersects(Rectangle))
            {
                if (IsEmpty)
                {
                    EmptyUpdate();
                }
            }
        }

        #region Updates
        private void EmptyUpdate()
        {
            _background.Colour = Color.Gray;
            _plusColour = Color.White;

            if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
            {
                NewSave();
                IsEmpty = false;
            }
        }

        #endregion Updates

        public void LoadContent(Game1 _game, BorderedBox background, int index)
        {
            var textureScale = 0.6f;
            var fullScale = textureScale * Game1.ResScale;
            var gap = 200f * Game1.ResScale;

            InitialiseValues(_game, background, index, gap);
            SetValues(_game);
        }

        #region Init
        private void SetValues(Game1 _game)
        {
            _background = new BorderedBox
                        (
                            _texture,
                            _game.GraphicsDevice,
                            Color.Wheat,
                            Position,
                            Layer,
                            Width,
                            Height
                        );

            _components = new List<Component>()
            {
                _background,
            };

            _plusPosition = new Vector2
            (
                Position.X + Width / 2 - _game.Textures.Plus.Width / 2 * _plusScale,
                Position.Y + Height / 2 - _game.Textures.Plus.Height / 2 * _plusScale
            );
        }

        private void InitialiseValues(Game1 _game, BorderedBox background, int index, float gap)
        {
            _saveNumber = (Text.ToArray().Last() - '0') - 1;

            if (_game.SavesManager.Saves[_saveNumber] != null)
                IsEmpty = false;

            _index = index;

            _plusScale = 1f * Game1.ResScale;

            Width = (int)(_font.MeasureString(Text).X + gap);
            Height = (int)(_font.MeasureString(Text).Y * 3);

            Position = new Vector2(background.Position.X + background.Width / 2 - Width / 2, background.Position.Y + background.Height / 5 - Height / 2 + (Height + 10 * Game1.ResScale) * index);

            _plusTexture = _game.Textures.Plus;
        }

        #endregion

        private void NewSave()
        {
            _game.SavesManager.Saves[_index] = Save.NewSave(_game.SavesManager);
            _game.SavesManager.UploadSave(_index);
        }
    }
}
