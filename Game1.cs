﻿using System;
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
using System.Diagnostics;
using System.Text;
using CameraFollowingSprite.Core;
using System.Security.Cryptography.Xml;

//A "State" is the current level of the game this can be the main menu, character creation or even a boss arena
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
        private List<Sprite> _sprites;
        private List<string> _statesWithoutPlayer = new List<string>()
        {
            StateNames.MainMenu, StateNames.CharacterInit
        };

        private int _defaultHeight = 360;

        //can be accessed without having by a state/component without a game object accessible
        public static int ScreenHeight;
        public static int ScreenWidth;
        public static float ResScale;
        public static bool InDebug = false;
        public static Color DebugColour = Color.White;
        public static Vector2 V2Transform;
        public static StateNames StateNames = new StateNames();

        //all the managers of objects that will be stored on disk
        public SettingsManager Settings;
        public Input PlayerKeys;
        public static Dictionary<string, string> SettingsStates;
        public SaveManager SavesManager;
        public Camera Camera;

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

        public string CurrentStateName
        {
            get
            {
                return _currentState.Name;
            }
        }


        #endregion

        #region Constructor/Inherited

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferHalfPixelOffset = true;
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
            Player = new Player(Textures.PlayerStatic, PlayerKeys, this)
            {
                Layer = 0.75f,
                Position = new Vector2(100, 0),
            };

            Camera = new Camera();

            V2Transform = Vector2.Zero;

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

            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();

            //checks if F11 is pressed, if so: toggle fullscreen
            ChangeFullscreenMode();
            //check debug
            ChangeDebugMode();

            //if you aren't in the main menu you may press escape to access settings and return to the main menu
            if (_currentKeys.IsKeyDown(Keys.Escape) && _previousKeys.IsKeyUp(Keys.Escape) && _currentState.Popups.Count == 0 && _currentState.Name != StateNames.MainMenu)
            {
                var settings = new Settings(this, Content, _currentState, _graphics);
                _currentState.Popups.Add(settings);
                settings.LoadContent();
                settings.LoadMenuButton();
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            Camera.Follow(Player);

            if (_statesWithoutPlayer.Contains(_currentState.Name))
                V2Transform = Vector2.Zero;

            base.Update(gameTime);
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        //Draws the textures on to the screen.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Cornsilk);

            //Front to back means that textures with a lower Layer value will be drawn behind textures with higher Layer values.
            //By setting sampler state to PointClamp, no interpolation occurs when accessing Texture2D files, this was especially bad
            //when using my texture atlases as they would be prone to texture bleeding due to interpolation.
            //Transform matrix is to keep the character at the center of the screen.
            if (_currentState.Name[0] == 'l')
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: Camera.Transform);
            else
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

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

            if (_currentKeys.IsKeyDown(Keys.F11) && _previousKeys.IsKeyUp(Keys.F11))
            {
                ToggleFullScreen();
                SettingsManager.Save(Settings);
            }
        }

        private void ChangeDebugMode()
        {
            if (_currentKeys.IsKeyDown(Keys.F3) && _previousKeys.IsKeyUp(Keys.F3))
                InDebug = !InDebug;
            if (_currentKeys.IsKeyDown(Keys.F4) && _previousKeys.IsKeyUp(Keys.F4))
            {
                if (DebugColour == Color.White)
                    DebugColour = Color.Black;
                else DebugColour = Color.White;
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
            if (_currentState.Name != StateNames.MainMenu && _currentState.Popups.Count > 0)
            {
                var settings = (_currentState.Popups[^1] as Settings);
                Player.Reset();
                Camera.Follow(Player);
                settings.LoadContent();
                settings.LoadMenuButton();
            }

            Player.Reset();
        }

        public List<List<int>> RetrieveLevelMap(int level)
        {
            List<List<int>> map;
            using (var reader = new System.IO.StreamReader($"Content/Levels/Level{level}.txt"))
            {
                map = reader.ReadToEnd().Split("\r\n").Select(x => x.Split(',').Select(y => ProcessIndex(y)).ToList()).ToList();
            }
            return map;
        }

        private int ProcessIndex(string x) => int.TryParse(x, out var index) ? index : -1;

        public List<Rectangle> GenerateSurfaces(List<List<int>> levelMap, int scale)
        {
            var surfaces = new List<Rectangle>();
            var sideLength = Textures.BlockWidth * scale; //they are squares => height = width
            for (int i = 0; i < levelMap.Count; i++)
            {
                for (int j = 0; j < levelMap[i].Count; j++)
                {
                    if (levelMap[i][j] == -1)
                        continue;
                    surfaces.Add(new Rectangle(i * scale, j * scale, sideLength, sideLength));
                }
            }
            return surfaces;
        }
        #endregion
    }
}
