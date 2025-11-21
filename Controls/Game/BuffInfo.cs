
using Bound.Models;
using Bound.Models.Items;
using Microsoft.VisualBasic.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound.Controls.Game
{
    public class BuffInfo : Component
    {
        private static Texture2D _redX;

        private float _gap;
        private Buff _buff;
        private Game1 _game;
        private Vector2 _iconPosition;
        private Vector2 _timerPosition;
        private float _scale;
        private float _layer;
        private BorderedBox _background;
        private SpriteFont _font;
        private Color _opacity;
        private bool _isRecoveryConsumable;

        private Texture2D _icon
        {
            get { return _buff.Icon; }
        }

        public string TimeRemaning
        {
            get
            {
                int mins = (int)_buff.SecondsRemaining / 60;
                return $"{FormatTime(mins)}:{FormatTime((int)_buff.SecondsRemaining % 60)}";
            }
        }

        public float SecondsRemaining
        {
            get { return _buff.SecondsRemaining; } 
        }

        public float FullScale
        {
            get { return _scale * Game1.ResScale; }
        }

        public Vector2 Position
        {
            get { return _background.Position; }
            set
            {
                _background.Position = value;
                SetComponentPositions(value);
            }
        }

        public int Height
        {
            get { return _background.Height; }
        }

        public int Width
        {
            get { return _background.Width; }
        }

        public Buff Buff
        {
            get { return _buff; }
        }

        public BuffInfo(Buff buff, Game1 game, Vector2 position, float scale, float layer)
        {
            _buff = buff;
            _game = game;
            _scale = scale;
            _layer = layer;
            _font = _game.Textures.Font;
            _gap = 2 * FullScale;
            _opacity = new Color(Color.White, 150);
            _isRecoveryConsumable = (_game.Items[buff.Source] as Consumable).ConsumableType == Consumable.ConsumableTypes.Recovery;

            SetComponentPositions(position);
            LoadContent(position);
        }

        private void LoadContent(Vector2 position)
        {
            
            var _timerDimensions = _font.MeasureString("xx:xx") * _scale;

            _background = new BorderedBox(
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[0],
                position,
                _layer,
                (int)((_timerPosition.X - position.X) + _timerDimensions.X + 2 * _gap ),
                (int)(_icon.Height * FullScale + 2 * _gap)
            )
            {
                IgnoreCameraTransform = true
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_icon, _iconPosition + Game1.V2Transform, null, _opacity, 0f, Vector2.Zero, FullScale, SpriteEffects.None, _layer + 0.001f);
            _background.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(_font, TimeRemaning, _timerPosition + Game1.V2Transform, _opacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, _layer + 0.001f);
            if (_isRecoveryConsumable)
                spriteBatch.Draw(_redX, _iconPosition + Game1.V2Transform, null, _opacity, 0f, Vector2.Zero, FullScale, SpriteEffects.None, _layer + 0.0011f);

        }

        public override void Update(GameTime gameTime)
        {
        }
        private void SetComponentPositions(Vector2 value)
        {
            _iconPosition = value + new Vector2(_gap, _gap);
            _timerPosition = value + new Vector2(_gap + _icon.Width * FullScale + 2 * _gap - 1f * Game1.ResScale, _gap + (_icon.Height * FullScale - _font.MeasureString("0").Y * _scale) / 2);
        }

        private string FormatTime(int time) => (time / 10 < 1) ? $"0{time}" : time.ToString();

        public static void SetRedXTexture(Texture2D t) => _redX = t;
    }
}
