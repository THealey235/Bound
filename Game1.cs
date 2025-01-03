using System;
using Bound.Models;
using Bound.States;
using Bound.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Bound.States.Popups;
using Bound.Models.Items;
using Bound.Sprites;

//A "State" is the current level of the game this can be the main menu, character creation or even a boss arena;
//A "Component" is the abstract class of all objects which will be drawn/updated
//For more info refer to their respective .cs files

namespace Bound
{
    //Main game class where everything is routed through
    public class Game1 : Game
    {
        #region Fields & Properties

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Player Player;

        private int _defaultHeight = 360;

        //can be accessed without having by a state/component without a game object accessible
        public static int ScreenHeight;
        public static int ScreenWidth;
        public static float ResScale;

        //all the managers of objects that will be stored on disk
        public SettingsManager Settings;
        public Input PlayerKeys;
        public static Dictionary<string, string> SettingsStates;
        public SaveManager SavesManager;

        //most recent save accesssed to know which save to load when the user presses "continue" on the main menu
        public int RecentSave;

        //used for changing state so that you do not change a state before the full Update/Draw loop has finished
        private State _currentState;
        private State _nextState;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public static Random Random;

        public bool UseDefaultMouse;

        public Dictionary<int, Item> Items;
        public Dictionary<string, int> ItemCodes;

        public int SaveIndex;

        //Holds all textures to be used
        public Textures Textures;

        #endregion

        #region Constructor/Inherited

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //Root diretcory for the texture files
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //reads settings.txt or creates a new settings file if missing
            Settings = SettingsManager.Load();

            //makes it "Borderless Windowed" and enables high pixel count textures for 4k monitors and advanced shaders
            _graphics.HardwareModeSwitch = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.ApplyChanges();

            //calls the inhereited method
            base.Initialize();

            //applys video settings from Settings.Settings.General
            var resolution = Settings.Settings.General["Resolution"].Split("x").Select(x => int.Parse(x)).ToList();

            ScreenHeight = _graphics.PreferredBackBufferHeight = resolution[1];
            ScreenWidth = _graphics.PreferredBackBufferWidth = resolution[0];
            _graphics.IsFullScreen = (Settings.Settings.General["Fullscreen"] == "Yes") ? true : false;

            _graphics.ApplyChanges();

            //Scale used to change the size of textures based on the resolution. Defalut = 640x360.
            ResScale = (float)ScreenHeight / (float)_defaultHeight;

            RecentSave = int.Parse(Settings.Settings.General["MostRecentSave"]);

            Random = new Random();

            SavesManager = new SaveManager(this);

            var keys = Input.SpecialKeyMap.Keys.ToList();
            var values = Input.SpecialKeyMap.Values.ToList();
            for (int i = 0; i < Input.SpecialKeyMap.Count; i++)
            {
                Input.KeysFromSpecialKey.Add(values[i], keys[i]);
            }

            PlayerKeys = new Input(Settings.Settings.InputValues);
            Player = new Player(Textures.PlayerStatic, PlayerKeys);

            ResetState();
        }

        //Loads textures and initial state (main menu)
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Textures = new Textures(Content);

            var itemDicts = Textures.LoadItems();
            Items = itemDicts.Item1;
            ItemCodes = itemDicts.Item2;

            _currentState = new MainMenu(this, Content, _graphics);
            _currentState.LoadContent();

            _nextState = null;
        }

        //Update loop called every frame
        protected override void Update(GameTime gameTime)
        {
            //if we need to change state _nextState holds the value of the new state
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;

                _currentState.LoadContent();
            }

            //checks if F11 is pressed, if so: toggle fullscreen
            ChangeFullscreenMode();

            //if you aren't in the main menu you may press escape to access settings and return to the main menu
            if (_currentKeys.IsKeyDown(Keys.Escape) && _previousKeys.IsKeyUp(Keys.Escape) && _currentState.Popups.Count == 0 && _currentState.Name != "mainmenu")
            {
                var settings = new Settings(this, Content, _currentState, _graphics);
                _currentState.Popups.Add(settings);
                settings.LoadContent();
                settings.LoadMenuButton();
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        //Draws the textures on to the screen.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Front to back means that textures with a lower Layer value will be drawn behind textures with higher Layer values
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            _currentState.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Other Methods

        private void ChangeKeys(Dictionary<string, string> Keys)
        {

        }
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
             //Gets Default screen resolution and applies it
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

        //Used to reload all current popups and states so that when the res changes
        //the textures and popups have correctly scaled textures since LoadContent is only typically run once
        public void ResetState()
        {
            ResScale = (float)ScreenHeight / (float)_defaultHeight;

            _currentState.LoadContent();

            foreach (var state in _currentState.Popups)
                state.LoadContent();

            //if it is the main menu remove the "quit" button from settings to return to the main menu
            if (_currentState.Name != "mainmenu" && _currentState.Popups.Count > 0)
            {
                (_currentState.Popups[^1] as Settings).LoadMenuButton();
            }
        }

        #endregion
    }
}
