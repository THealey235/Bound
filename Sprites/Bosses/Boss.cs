using Bound.Controls;
using Bound.Managers;
using Bound.Models;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bound.Sprites
{
    public class Boss : Mob
    {

        protected BorderedBox _healthBar;
        protected Vector2 _bossBarPosition;
        protected int _maxHealth;
        protected int _bossBarBorderWidth = 1; //found from looking at the texture, unscaled to window dimensions
        protected int _bossBarFullWidth;
        protected SpriteFont _font;
        protected Vector2 _namePosition;
        protected float _nameScale = 0.9f;
        protected bool _canAttack = true;
        protected Dictionary<string, ActionInfo> _actions;
        protected ActionInfo _atRest = new ActionInfo(0, Vector2.Zero, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0), (0, 0));
        protected ActionInfo _currentAction = new ActionInfo(0, Vector2.Zero, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0), (0, 0));//Default at rest, this is so that base.Reset() that is called before this is set to _atRest can get Position
        protected Item _weapon;
        protected DebugRectangle _weaponDebugRectangle;
        protected float _attackCooldown = 1f;
        protected DebugRectangle _fullTextureRectangle;

        protected record ActionInfo(float Range, Vector2 PositionOffset, Rectangle HitBox, Rectangle BossHitBox, (int Start, int End) HitBoxActiveFrames);
        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                    CollisionRectXPos(_currentAction.BossHitBox.X, _currentAction.BossHitBox.Width),
                    (int)(VirtualPosition.Y + _currentAction.BossHitBox.Y * Scale),
                    (int)(_currentAction.BossHitBox.Width * Scale),
                    (int)(_currentAction.BossHitBox.Height * Scale)
                );
            }
        }

        protected Vector2 VirtualPosition
        {
            get
            {
                if (Effects == SpriteEffects.FlipHorizontally && _animations.ContainsKey("Walking"))
                    return new Vector2(_position.X - ((_animationManager.CurrentAnimation.FrameWidth * Scale) - (Scale * (_animations["Walking"].FrameWidth + _currentAction.PositionOffset.X))), _position.Y);
                return _position - (_currentAction.PositionOffset * Scale);
            }
        }

        protected Boss(Game1 game, string name) : base(game, name)
        {
            _name = name;

            TextureManager.MobInfo info;
            if (game.Textures.Sprites.TryGetValue(name, out _textures) && game.Textures.GetMobInfo(name, out info))
            {
                _health = _maxHealth = info.Health;
                SetHealthBar(game, name);
            }
            else _health = 0; //Kill the sprite before it even spawns since the boss is not loaded/doesn't exist

            _currentAction = _atRest; //should be reset at the end of the constructor of inherited class
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

            DrawHealthBar(gameTime, spriteBatch);

            if (Game1.InDebug)
            {
                if (_currentAction != _atRest &&
                    _animationManager.CurrentAnimation.CurrentFrame >= _currentAction.HitBoxActiveFrames.Start && _animationManager.CurrentAnimation.CurrentFrame <= _currentAction.HitBoxActiveFrames.End &&
                    _debugRectangle != null)
                {
                    _weaponDebugRectangle.Draw(gameTime, spriteBatch);
                }
                _fullTextureRectangle.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites = null, List<Sprite> dealsKnockback = null)
        {
            base.Update(gameTime, surfaces, collideableSprites, dealsKnockback);

            if (_currentAction != _atRest &&
                _animationManager.CurrentAnimation.CurrentFrame >= _currentAction.HitBoxActiveFrames.Start && _animationManager.CurrentAnimation.CurrentFrame <= _currentAction.HitBoxActiveFrames.End &&
                _weapon != null)
            {
                _weapon.CheckCollision(this, dealsKnockback);
            }

            if (_attackCooldown > 0)
            {
                _attackCooldown -= _dTime;
                if (_attackCooldown <= 0)
                    _canAttack = true;
            }

            if (_animationManager.IsPlaying == false)
            {
                _animationManager.Play(_animations["Walking"]);
                _weapon.ClearBlacklist();
                _lockEffects = false;
                _attackCooldown = 1f;
                _currentAction = _atRest;
                ResetFullTextureDebugRectangle();
            }

            var rect = Rectangle;
            _debugRectangle.Position = new Vector2(rect.X, rect.Y) * Game1.ResScale;
            _animationManager.Position = _fullTextureRectangle.Position = VirtualPosition * Game1.ResScale;
        }

        protected void DrawHealthBar(GameTime gameTime, SpriteBatch spriteBatch)
        {
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

        protected void ChangeAction(KeyValuePair<string, ActionInfo> action)
        {
            _currentAction = action.Value;
            _animationManager.Play(_animations[action.Key]);
            _animationManager.Loop = false;
            _canAttack = false;
            _lockEffects = true;

            var rectangle = new Rectangle(CollisionRectXPos(_currentAction.HitBox.X, _currentAction.HitBox.Width), (int)(VirtualPosition.Y + (_currentAction.HitBox.Y * Scale)), (int)(_currentAction.HitBox.Width * Scale), (int)(_currentAction.HitBox.Height * Scale));

            _weaponDebugRectangle = new Models.DebugRectangle(rectangle, _game.GraphicsDevice, _game.Player.Layer + 0.01f, Game1.ResScale);
            _weaponDebugRectangle.Position = new Vector2(rectangle.X, rectangle.Y) * Game1.ResScale;
            _weapon.UpdateCollisionRectangle(this, rectangle);
            ResetFullTextureDebugRectangle();
        }

        protected void ResetFullTextureDebugRectangle()
        {
            _fullTextureRectangle = new DebugRectangle(new Rectangle(0, 0, (int)(_animationManager.CurrentAnimation.FrameWidth * FullScale), (int)(_animationManager.CurrentAnimation.FrameHeight * FullScale)), _game.GraphicsDevice, _game.Player.Layer + 0.01f, 1f);
        }

        public int CollisionRectXPos(int offset, int width)
        {
            if (Effects == SpriteEffects.FlipHorizontally)
                return (int)((VirtualPosition.X + _animationManager.CurrentAnimation.FrameWidth * Scale) - (offset + width) * Scale);
            else return (int)(VirtualPosition.X + offset * Scale);
        }
    }
}
