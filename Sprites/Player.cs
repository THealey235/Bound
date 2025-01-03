using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Bound.Sprites
{
    public class Player : Sprite
    {
        private Input _keys;
        public Player(Texture2D texture, Input Keys) : base(texture)
        {
            _keys = Keys;
        }

        public Player(Dictionary<string, Animation> animations, Input Keys) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _keys.Update();

            if (_keys.IsPressed("Up", false))
            {

            }
        }
    }
}
