using Bound.Managers;
using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls
{
    public class SaveInterface : Component
    {
        #region Fields & Properties

        private List<Component> _emptyComponents;
        private List<Component> _fullComponents;
        private Texture2D _texture;
        private SpriteFont _font;
        private BorderedBox _background;
        private Game1 _game;
        private int _saveNumber;
        private int _index;
        private ConfirmBox _confirmBox;

        private float _plusScale;
        private Texture2D _plusTexture;
        private Color _plusColour;
        private Vector2 _plusPosition;
        private Vector2 _namePosition;

        public bool MouseLock;
        public string Text;
        public Vector2 Position;
        public float Layer;
        public EventHandler Play;

        private MouseState _previousMouse;
        private MouseState _currentMouse;
        public int Width;

        public int Height;
        public Color BorderColor = Color.Black;
        public bool IsEmpty;
        public Color PenColor = Color.White;
        public bool StopUpdate;

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                if (IsEmpty)
                    return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        #endregion

        public SaveInterface(Texture2D texture, SpriteFont font, Game1 game)
        {
            _texture = texture;
            _font = font;
            _game = game;
            IsEmpty = true;

            _plusColour = Color.Gray;
            StopUpdate = false;
            PenColor = Color.DarkRed;
        }

        #region Inherited

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (_confirmBox != null)
                _confirmBox.Draw(gameTime, spriteBatch);

            var components = (IsEmpty) ? _emptyComponents : _fullComponents;

            foreach (var component in components)
                component.Draw(gameTime, spriteBatch);

            if (IsEmpty)
                spriteBatch.Draw(_plusTexture, _plusPosition, null, _plusColour, 0f, Vector2.Zero, _plusScale, SpriteEffects.None, Layer + 0.02f);
            else
            {
                spriteBatch.DrawString(_font, _game.SavesManager.Saves[_index].PlayerName, _namePosition, PenColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }

            spriteBatch.DrawString(_font, Text, Position, PenColor);
        }

        public override void Update(GameTime gameTime)
        {
            if (StopUpdate)
            {
                _confirmBox.Update(gameTime);
                if (_confirmBox.IsConfirmed)
                {
                    if (_confirmBox.IsYes)
                    {
                        RemoveSave();
                    }
                    //Do nothing if no

                    StopUpdate = false;
                    _confirmBox = null;
                }
                return;
            }

            var components = (IsEmpty) ? _emptyComponents : _fullComponents;

            foreach (var component in components)
                component.Update(gameTime);


            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);


            if (IsEmpty)
            {
                _plusColour = Color.Gray;
                _background.Colour = Color.Wheat;
                

                if (mouseRectangle.Intersects(Rectangle) && !MouseLock)
                    EmptyUpdate();

                if (MouseLock && _currentMouse.LeftButton == ButtonState.Released)
                    MouseLock = false;
            }
        }

        #endregion

        #region Updates
        private void EmptyUpdate()
        {
            _background.Colour = Color.Gray;
            _plusColour = Color.White;

            if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
            {
                NewSave();
                IsEmpty = false;
                _background.Colour = Color.Wheat;
            }
        }

        #endregion Updates

        public void LoadContent(Game1 _game, BorderedBox background, int index)
        {
            var textureScale = 0.65f;
            var fullScale = textureScale * Game1.ResScale;
            var gap = 200f * Game1.ResScale;

            InitialiseValues(_game, background, index, gap);
            SetEmptyValues(_game);
            SetFullValues(_game, fullScale, textureScale);
        }

        #region Init
        private void SetEmptyValues(Game1 _game)
        {
            _emptyComponents = new List<Component>()
            {
                _background,
            };

            _plusPosition = new Vector2
            (
                Position.X + Width / 2 - _game.Textures.Plus.Width / 2 * _plusScale,
                Position.Y + Height / 2 - _game.Textures.Plus.Height / 2 * _plusScale
            );
        }

        private void SetFullValues(Game1 _game, float scale, float textureScale)
        {
            Text = _saveNumber.ToString();
            var center = new Vector2(_background.Position.X + _background.Width / 2, _background.Position.Y + _background.Height / 2);
            var rightCentre = new Vector2(_background.Position.X + _background.Width - _background.Width / 8, center.Y);

            _fullComponents = new List<Component>()
            {
                _background,
                new Button(_game.Textures.PlayButton, _game.Textures.Font)
                {
                    Position = new Vector2(rightCentre.X - (_game.Textures.PlayButton.Width * scale) - (5 * scale), center.Y - (20 * scale)),
                    Click = new EventHandler(Button_Play_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale
                },
                new Button(_game.Textures.TrashCan, _game.Textures.Font)
                {
                    Position = new Vector2(rightCentre.X  + (5 * scale), center.Y - (20 * scale)),
                    Click = new EventHandler(Button_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale
                }
            };

            _namePosition = new Vector2(_background.Position.X + (10f * scale), _background.Position.Y + _background.Height / 4);
        }


        private void InitialiseValues(Game1 _game, BorderedBox background, int index, float gap)
        {
            _saveNumber = (Text.ToArray().Last() - '0') - 1;

            if (_game.SavesManager.Saves[_saveNumber] != null)
                IsEmpty = false;

            _index = index;

            _plusScale = 1f * Game1.ResScale;

            Width = (int)(_font.MeasureString(Text).X + gap + 170f * _plusScale);
            Height = (int)(_font.MeasureString(Text).Y * 3);

            Position = new Vector2(background.Position.X + background.Width / 2 - Width / 2, background.Position.Y + background.Height / 5 - Height / 2 + (Height + 10 * Game1.ResScale) * index);

            _plusTexture = _game.Textures.Plus;

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
        }


        #endregion

        #region Clicked Methdods

        private void NewSave()
        {
            _game.SavesManager.Saves[_index] = Save.NewSave(_game.SavesManager);
            _game.SavesManager.UploadSave(_index);
        }
        private void RemoveSave()
        {
            _game.SavesManager.Saves[_index] = null;
            _game.SavesManager.UploadSave(_index);
            IsEmpty = true;
            MouseLock = true;
        }

        private void Button_Play_Clicked(object sender, EventArgs e)
        {
            _game.Settings.Settings.General["MostRecentSave"] = _index.ToString();
            SettingsManager.Save(_game.Settings);
            Play(this, new EventArgs());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            _confirmBox = new ConfirmBox(_game, _font, "delete")
            {
                Layer = Layer + 0.08f
            };
            _confirmBox.LoadContent();
            StopUpdate = true;
        }

        #endregion
    }
}
