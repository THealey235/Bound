using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using Bound.Sprites;

namespace Bound.States
{

    // I have not specifically commented the code in /State or /Popups so you may see some undescribed spaghetti code
    // or unuseful comments such as: TODO: Fix this
    public abstract class State
    {
        protected Game1 _game;
        //TODO: remove _content from state since all textures should be handled be Textures.cs
        protected ContentManager _content;

        public List<State> Popups;
        public string Name;

        protected Player _player;
        protected List<List<int>> _levelMap;
        protected float _scale;
        protected List<Rectangle> _surfaces;

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
