using Microsoft.Xna.Framework.Graphics;
using Bound.Models;
using Bound.States;
using Microsoft.Xna.Framework;
using Bound.Managers;
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
        public int RecentSave;
        public SaveManager SavesManager;


        private State _currentState;
        private State _nextState;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public static Random Random;

        #endregion

        #region Textures

        public Textures Textures;

        

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

            ResScale = (float)ScreenHeight / (float)_defaultHeight;

            RecentSave = int.Parse(Settings.Settings.General["MostRecentSave"]);

            Random = new Random();

            SavesManager = new SaveManager();

            ResetState();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Textures = new Textures(Content);

            _currentState = new MainMenu(this, Content, _graphics);
            _currentState.LoadContent();

            _nextState = null;

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
                SettingsManager.Save(Settings);
            }
        }

        private void ToggleFullScreen()
        {
            _graphics.ToggleFullScreen();
            if (_graphics.IsFullScreen)
            {
                ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Settings.Settings.General["Fullscreen"] = "Yes";
            }
            else
            {
                ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Settings.Settings.General["Fullscreen"] = "No";
            }

            _graphics.ApplyChanges();
            Settings.UpdateResolution(Game1.ScreenWidth, Game1.ScreenHeight);

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
