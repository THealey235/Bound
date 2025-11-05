using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Sprites
{
    public class Projectile : Sprite
    {
        private Sprite _owner;
        private float _elapsedTime;
        private float _horizontalVelocity;
        private bool _lockPhysics = false;

        public Sprite Owner { get { return _owner; } }

        public Projectile(Texture2D texture, string name, Sprite owner, Game1 game, float rotation, Vector2 offset, float horizontalVelocity = 700f) : base(texture, game)
        {
            Rotation = (float)Math.PI / 2f;
            _horizontalVelocity = horizontalVelocity;
            _owner = owner;
            _spriteType = SpriteType.Projectile;
            _name = name;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            _health = 1;
            Position = _owner.Position + offset;

            if (owner.Name == "player")
            {
                if (_game.PlayerKeys.MouseRectangle.X < owner.ScaledPosition.X + owner.ScaledRectangle.Width / 2)
                {
                    _horizontalVelocity *= -1;
                    Effects = _owner.Effects = SpriteEffects.FlipHorizontally;
                }
                else
                    _owner.Effects = SpriteEffects.None;
            }
            else if (owner.Effects == SpriteEffects.FlipHorizontally)
            {
                _horizontalVelocity *= -1;
            }

            if (_game.Textures.GetCustomProjectileHitbox(name, out (int Width, int Height, Vector2 Origin, float Scale) bounds))
            {
                CustomHitbox = new Rectangle(0, 0, bounds.Width, bounds.Height);
                Scale *= bounds.Scale;
            }

            Reset();

            _debugRectangle.BorderColour = Color.Red;
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
            if (_elapsedTime > 15f)
                Destroy();
            else if (Velocity.X == 0 || Velocity.Y == 0)
                _lockPhysics = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
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
            Velocity += new Vector2(_horizontalVelocity, 0);
        }

        public void Destroy() => Damage(1);
    }
}
