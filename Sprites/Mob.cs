using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bound.Sprites
{
    public class Mob : Sprite
    {
        public Mob(Models.TextureCollection textures, Game1 game) : base(textures, game)
        {
            _speed = 0.75f;
            if (textures.Sheets.Count > 0)
            {
                //set animationManager and animations
            }
        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites)
        { 
            base.Update(gameTime, surfaces, collideableSprites);  
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
