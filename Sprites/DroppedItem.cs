using Bound.Models.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Bound.Sprites
{
    public class DroppedItem : Sprite
    {
        private Item _item;

        public DroppedItem(Game1 game, Item item, Vector2 position) : base(game.Textures.Items[item.Name].GetIcon(), game)
        {
            _spriteType = SpriteType.DroppedItem;
            _position = position;
            _item = item;
            Velocity = new Vector2((float)Game1.Random.NextDouble() * 2f, (float)Game1.Random.NextDouble() * 2f);
            _health = 1;
            Scale = 0.5f;
            Layer = _game.Player.Layer;

            _debugRectangle = new Models.DebugRectangle(
                Rectangle,
                _game.GraphicsDevice,
                _game.Player.Layer + 0.01f,
                FullScale
            );
            Reset();
        }

        protected override void CheckSpriteCollision(List<Sprite> sprites, List<Sprite> dealsKnockback)
        {
            var sprite = _game.Player;

            if (sprite.Velocity.X > 0 && sprite.IsTouchingLeft(this) ||
                sprite.Velocity.X < 0 && sprite.IsTouchingRight(this) ||
                sprite.Velocity.Y > 0 && sprite.IsTouchingTop(this) ||
                sprite.Velocity.Y < 0 && sprite.IsTouchingBottom(this))
            {
                sprite.Inventory.Add(_item);
                _health = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites = null, List<Sprite> dealsKnockback = null)
        {
            base.Update(gameTime, surfaces, collideableSprites, dealsKnockback);
        }
    }
}
