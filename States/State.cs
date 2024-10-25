using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Bound.States.Popups;

namespace Bound.States
{
    public abstract class State
    {
        protected Game1 _game;

        protected ContentManager _content;

        public List<State> Popups;

        public State(Game1 game, ContentManager content)
        {
            _game = game;

            _content = content;

            Popups = new List<State>();
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void PostUpdate(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }
}
