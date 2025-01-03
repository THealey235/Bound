using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Bound.States.Game
{
    public class Level0 : State
    {
        private Player _player;

        public Level0(Game1 game, ContentManager content, Player player) : base(game, content)
        {
            Name = "levelzero";
            _player = player;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var p in Popups)
                p.Draw(gameTime, spriteBatch);
            _player.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent()
        {
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var p in Popups)
                p.Update(gameTime);
            _player.Update(gameTime);
        }
    }
}
