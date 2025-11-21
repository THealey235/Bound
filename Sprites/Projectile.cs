using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Bound.Managers.TextureManager;

namespace Bound.Sprites
{
    public class Projectile : Sprite
    {
        private Sprite _owner;
        private float _elapsedTime;
        private Vector2 _throwVelocity;
        private bool _lockPhysics = false;
        private Vector2 _embeddedLength;
        private float _textureRoatation = 0f;

        public Sprite Owner { get { return _owner; } }

        public new float Rotation
        {
            get
            {
                return (float)(Math.PI / 2 + Math.Atan2(Velocity.Y, Velocity.X) - _textureRoatation);
            }
        }

        public Projectile(Texture2D texture, string name, Sprite owner, Game1 game, float damage, float throwVelocity = 700f) : base(texture, game)
        {
            _throwVelocity = new Vector2(throwVelocity, 0);
            _owner = owner;
            _spriteType = SpriteType.Projectile;
            _name = name;
            _knockbackDamageDealtOut = damage;

            _health = 1;
            Position = _owner.Position;

            if (owner.Name == "player")
            {
                var mousePos = _game.PlayerKeys.MousePosition + Game1.V2Transform;
                var ownerPos = owner.ScaledPosition + (owner.TextureCenter * owner.FullScale); //Position of the centre of the owner
                var throwDirectionVector = (mousePos) - (ownerPos);

                if (throwDirectionVector.X < 0)
                    _owner.Effects = SpriteEffects.FlipHorizontally;
                else
                    _owner.Effects = SpriteEffects.None;

                throwDirectionVector.Normalize(); //Makes unit vector in direciton of throwDirection
                _throwVelocity = throwDirectionVector * throwVelocity;
            }
            else if (owner.Effects == SpriteEffects.FlipHorizontally)
                _throwVelocity *= -1;

            if (_game.Textures.GetProjectileInfo(name, out ProjectileInfo info))
            {
                Scale *= info.Scale;
                _textureRoatation = info.TextureRotation;
            }

            Reset();

            _embeddedLength = new Vector2(_texture.Width * 0.2f, _texture.Height * 0.2f);

        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites = null, List<Sprite> dealsKnockback = null)
        {
            if (!_lockPhysics)
            {
                collideableSprites.Remove(_owner);
                collideableSprites = collideableSprites.FindAll(x => x.Type != SpriteType.Projectile);

                base.Update(gameTime, surfaces, collideableSprites, dealsKnockback);
            }

            _elapsedTime += _dTime;
            if (_elapsedTime > 10f)
                Destroy();
            else if (Velocity.X == 0 || Velocity.Y == 0)
                _lockPhysics = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null && _animationManager.IsPlaying == true)
                _animationManager.Draw(spriteBatch);
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition + TextureCenter, null, Colour, Rotation, TextureCenter, FullScale, Effects, Layer);

            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }
        protected override void Knockback()
        {
            return;
        }

        public override void StartKnocback(string direction, float damage = 1)
        {
            Destroy();
        }

        protected override void HandleMovements(ref bool inFreefall)
        {
            _elapsedTime += _dTime;
            Velocity += _throwVelocity;
        }

        public void Destroy() => Damage(1);

        protected override void SurfaceTouched(string surfaceFace, Rectangle surface)
        {
            switch (surfaceFace)
            {
                case "left":
                    Velocity.X = surface.Left - Rectangle.Right + _embeddedLength.X; break;
                case "right":
                    Velocity.X = surface.Right - Rectangle.Left - _embeddedLength.X; break;
                case "top":
                    Velocity.Y = surface.Top - Rectangle.Bottom + _embeddedLength.Y; break;
                case "bottom":
                    Velocity.Y = surface.Bottom - Rectangle.Top - _embeddedLength.Y; break;
            }

            _lockPhysics = true;
        }
    }
}
