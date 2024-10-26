﻿using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Bound
{
    public class Game1 : Game
    {
        #region Fields & Properties

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _defaultHeight = 1080;
        private int _defaultWidth = 1920;

        public static int ScreenHeight = 1080;
        public static int ScreenWidth = 1920;

        public static float ResScale = 1f;

        private State _currentState;
        private State _nextState;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public Random Random;

        #endregion

        #region Textures

        public Texture2D Button;
        public List<SpriteFont> Fonts;
        public Texture2D BaseBackground;

        #endregion

        #region Main Methods

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.HardwareModeSwitch = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.ApplyChanges();

            base.Initialize();

            ScreenHeight = _graphics.PreferredBackBufferHeight = _defaultHeight;
            ScreenWidth = _graphics.PreferredBackBufferWidth = _defaultWidth;
            _graphics.ApplyChanges();

            IsMouseVisible = true;

            ResScale = ScreenHeight / _defaultHeight;

            Random = new Random();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Button = Content.Load<Texture2D>("Controls/Button");
            Fonts = new List<SpriteFont>()
            {
                Content.Load<SpriteFont>("Fonts/JX-720"),
                Content.Load<SpriteFont>("Fonts/JX-1080"),
                Content.Load<SpriteFont>("Fonts/JX-1440"),
                Content.Load<SpriteFont>("Fonts/JX-2160"),
            };
            BaseBackground = Content.Load<Texture2D>("Backgrounds/BaseBackground");

            _currentState = new MainMenu(this, Content);
            _currentState.LoadContent();

            _nextState = null;

        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                var _previousState = _currentState;
                _currentState = _nextState;
                _nextState = null;

                _currentState.LoadContent();
            }

            ChangeFullscreenMode();

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MidnightBlue);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            _currentState.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Other Methods

        private void ChangeFullscreenMode()
        {
            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();

            if (_currentKeys.IsKeyDown(Keys.F11) && _previousKeys.IsKeyUp(Keys.F11))
            {
                _graphics.ToggleFullScreen();
                if (_graphics.IsFullScreen)
                {
                    ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                }
                else
                {
                    ScreenHeight = _graphics.PreferredBackBufferHeight = _defaultHeight;
                    ScreenWidth = _graphics.PreferredBackBufferWidth = _defaultWidth;
                }

                _graphics.ApplyChanges();

                ResScale = ScreenHeight / _defaultHeight;

                _nextState = new MainMenu(this, Content);
            }
        }

        public SpriteFont SetSpriteFont()
        {
            if (ResScale < 1f)
                return Fonts[0];
            else if (ResScale == 1f)
                return Fonts[1];
            else if (ResScale == 2f)
                return Fonts[3];
            else
                return Fonts[2];
        }

        #endregion
    }
}