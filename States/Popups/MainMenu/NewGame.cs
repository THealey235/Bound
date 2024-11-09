using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.States.Popups
{
    public class NewGame : State
    {
        private List<Component> _components;

        public NewGame(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            var center = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);

            _components = new List<Component>();
            {

            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _game.GraphicsDevice.Clear(Color.Black);

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);  
        }
    }
}
