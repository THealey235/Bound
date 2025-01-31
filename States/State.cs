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
        //protected means that children can access it

        //holds the game class which there is only 1 of ever created (unless i add multiple windows)
        protected Game1 _game;

        //used to load content but is actually fairly redundant now as i have moved all textures to the Texture.cs handler
        //however i cba to refact rn: future me issue
        //TODO: remove _content from state
        protected ContentManager _content;

        //all "substates" known as popups. i.e. Settings or SavesMenu
        public List<State> Popups;

        //i.e. "mainMenu"
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

        //all states much have these methods

        //loads controls or sprites as components (typically)
        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void PostUpdate(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
