using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Bound.Sprites
{
    public class Mob : Sprite
    {
        public Mob(Models.TextureCollection textures, Game1 game, float health, float stamina, float mana) : base(textures, game)
        {
            _speed = 75f;
            _spriteType = SpriteType.Mob;
            if (textures.Sheets.Count > 0)
            {
                //set animationManager and animations
            }
            _health = health;
            _stamina = stamina;
            _mana = mana;
            _knockbackDamageDealtOut = 15f;
        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> sprites, List<Sprite> dealsKnockabck)
        { 
            base.Update(gameTime, surfaces, sprites, dealsKnockabck);  
        }

        protected override void HandleMovements(ref bool inFreefall)
        {
            var distance = (_game.Player.Position - Position).Length();

            if ((int)_game.Player.Position.X < (int)Position.X)
                Velocity -= new Vector2(_speed, 0);
            else if ((int)_game.Player.Position.X > (int)Position.X)
                Velocity += new Vector2(_speed, 0);
        }
    }
}
