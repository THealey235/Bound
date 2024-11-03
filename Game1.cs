using Bound.Managers;
using Bound.Models;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Bound
{
    public class Game1 : Game
    {
        #region Fields & Properties

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _defaultHeight = 1080;

        public static int ScreenHeight;
        public static int ScreenWidth;
        public static float ResScale;
        public SettingsManager Settings;
        public Input PlayerKeys;
        public static Dictionary<string, string> SettingsStates;
        public bool UseDefaultMouse;


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
        public Texture2D RedX;
        public Texture2D ArrowLeft;
        public Texture2D MouseOutline;
        public Texture2D MouseFill;

        public SpriteFont Font
        {
            get
            {
                return Game1.ScreenHeight switch
                {
                    720 => Fonts[0],
                    900 => Fonts[1],
                    1080 => Fonts[2],
                    1440 => Fonts[3],
                    2160 => Fonts[4],
                    _ => Fonts[2],
                };
            }
        }

        #endregion

        #region Constructor/Inherited

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Settings = SettingsManager.Load();

            _graphics.HardwareModeSwitch = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.ApplyChanges();

            base.Initialize();

            var resolution = Settings.Settings.General["Resolution"].Split("x").Select(x => int.Parse(x)).ToList();

            ScreenHeight = _graphics.PreferredBackBufferHeight = resolution[1];
            ScreenWidth = _graphics.PreferredBackBufferWidth = resolution[0];
            _graphics.IsFullScreen = (Settings.Settings.General["Fullscreen"] == "Yes") ? true : false;

            _graphics.ApplyChanges();

            var defaultMouse = (Settings.Settings.General["DefaultMouse"] == "Yes") ? true : false ;

            ResScale = (float)ScreenHeight / (float)_defaultHeight;

            Random = new Random();


            ResetState();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Button = Content.Load<Texture2D>("Controls/Button");
            Fonts = new List<SpriteFont>()
            {
                Content.Load<SpriteFont>("Fonts/JX-720"),
                Content.Load<SpriteFont>("Fonts/JX-900"),
                Content.Load<SpriteFont>("Fonts/JX-1080"),
                Content.Load<SpriteFont>("Fonts/JX-1440"),
                Content.Load<SpriteFont>("Fonts/JX-2160"),
            };
            BaseBackground = Content.Load<Texture2D>("Backgrounds/BaseBackground");
            RedX = Content.Load<Texture2D>("Controls/RedX");
            ArrowLeft = Content.Load<Texture2D>("Controls/ArrowLeft");
            MouseOutline = Content.Load<Texture2D>("Mouse/MouseOutline");
            MouseFill = Content.Load<Texture2D>("Mouse/MouseFill");

            _currentState = new MainMenu(this, Content, _graphics);
            _currentState.LoadContent();

            _nextState = null;

        }

        public void ReloadTextures()
        {
            
            Button = Content.Load<Texture2D>("Controls/Button");
            BaseBackground = Content.Load<Texture2D>("Backgrounds/BaseBackground");
            RedX = Content.Load<Texture2D>("Controls/RedX");
            ArrowLeft = Content.Load<Texture2D>("Controls/ArrowLeft");
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
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

            var mousePoint = Mouse.GetState().Position;
            var mousePosition = new Vector2(mousePoint.X, mousePoint.Y);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            //if (!UseDefaultMouse)
            //{
            //    _spriteBatch.Draw(MouseOutline, mousePosition, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            //    _spriteBatch.Draw(MouseFill, mousePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            //}

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
                ToggleFullScreen();
            }
        }

        private void ToggleFullScreen()
        {
            _graphics.ToggleFullScreen();
            if (_graphics.IsFullScreen)
            {
                ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
            else
            {
                ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }

            _graphics.ApplyChanges();

            ResetState();
        }

        public void ResetState()
        {
            ResScale = (float)ScreenHeight / (float)_defaultHeight;

            _currentState.LoadContent();

            foreach (var state in _currentState.Popups)
                state.LoadContent();
        }

        #endregion
    }
}
