using Bound.Controls;
using Bound.Managers;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Sprites
{
    public class Boss : Mob
    {
        private BorderedBox _healthBar;
        private Vector2 _bossBarPosition;
        private int _maxHealth;
        private int _bossBarBorderWidth = 1; //found from looking at the texture, unscaled to window dimensions
        private int _bossBarFullWidth;
        private SpriteFont _font;
        private Vector2 _namePosition;
        private float _nameScale = 0.9f;

        public Boss(Game1 game, string name) : base(game, name)
        {
            _name = name;

            TextureManager.MobInfo info;
            if (game.Textures.Sprites.TryGetValue(name, out _textures) && game.Textures.GetMobInfo(name, out info))
            {
                SetValues(_textures.Statics["Idle"], game);

                _health = _maxHealth = info.Health;
                SetHealthBar(game, name);
            }
            else _health = 0; //Kill the sprite before it even spawns since the boss is not loaded/doesn't exist
        }

        private void SetHealthBar(Game1 game, string name)
        {
            _font = game.Textures.Font;
            var bossBar = game.Textures.BossBar;
            _bossBarPosition = new Vector2((Game1.Viewport.Width - bossBar.Width * Game1.ResScale) / 2, Game1.Viewport.Height - (bossBar.Height + 10) * Game1.ResScale);
            _bossBarFullWidth = (int)((bossBar.Width - 2 * _bossBarBorderWidth) * Game1.ResScale);

            _healthBar = new BorderedBox(
                game.Textures.BaseBackground,
                game.GraphicsDevice,
                Color.IndianRed,
                new Vector2(_bossBarPosition.X + _bossBarBorderWidth * Game1.ResScale, _bossBarPosition.Y + _bossBarBorderWidth * Game1.ResScale),
                _game.Player.Level.HUD.Layer,
                _bossBarFullWidth,
                (int)((bossBar.Height - 2 * _bossBarBorderWidth) * Game1.ResScale)
            )
            { IgnoreCameraTransform = true };

            var nameDims = _font.MeasureString(name) * _nameScale;
            _namePosition = new Vector2(_bossBarPosition.X + (bossBar.Width * Game1.ResScale - nameDims.X) / 2, _bossBarPosition.Y + (bossBar.Height * Game1.ResScale - nameDims.Y) / 2);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Health > 0)
                _healthBar.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(_game.Textures.BossBar, _bossBarPosition + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, Game1.ResScale, _spriteEffects, _healthBar.Layer - 0.00001f);
            spriteBatch.DrawString(_font, Name, _namePosition + Game1.V2Transform, Color.White, 0f, Vector2.Zero, _nameScale, SpriteEffects.None, _healthBar.Layer + 0.00001f);
        }

        public override void Damage(string direction, float PATK, float MATK)
        {
            base.Damage(direction, PATK, MATK);
            if (Health > 0)
                _healthBar.Width = (int)(_bossBarFullWidth * (Health / _maxHealth));
            else _healthBar.Width = 1;
        }

        public override void ResetScaling()
        {
            base.ResetScaling();

            SetHealthBar(_game, Name);
        }

        public override void StartKnocback(string direction, float damage = 1, bool isPhsysical = true)
        {
            base.StartKnocback(direction, damage, isPhsysical);
        }
    }
}
