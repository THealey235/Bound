using Bound.Controls;
using Bound.Controls.Game;
using Bound.Managers;
using Bound.Models;
using Bound.Models.Items;
using Bound.Sprites;
using Bound.States;
using Bound.States.Popups;
using CameraFollowingSprite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

//A "State" is the current level of the game this can be the main menu, character creation or even a boss arena
//A "Component" is the abstract class of all objects which will be drawn/updated
//For more info refer to their respective .cs files

namespace Bound
{
    //Main game class where everything is routed through
    //TODO: Refactor the attributes so that they are not static/public unless they are properties for encapsulation.
    public class Game1 : Game
    {
        #region Fields & Properties

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Rectangle _viewport;

        public Player Player;
        private List<string> _statesWithoutPlayer = new List<string>()
        {
            Names.MainMenu, Names.CharacterInit
        };
        public int DefaultHeight = 360;

        public static int ScreenHeight
        {
            get { return _screenHeight; }
            set { 
                    _screenHeight = value;
                    _viewport.Height = value;
                }
        }
        public static int ScreenWidth
        {
            get { return _screenWidth; }
            set
            {
                _screenWidth = value;
                _viewport.Width = value;
            }
        }

        public static float ResScale;
        public static bool InDebug = false;
        public static Color DebugColour = Color.White;
        public static Names Names = new Names();
        public static Color[] MenuColorPalette = new Color[] { new Color(60, 60, 60), Color.LightGray, Color.White };
        private BorderedBox _mouse;

        public SettingsManager Settings;
        public Input PlayerKeys;
        public static Dictionary<string, string> SettingsStates;
        public SaveManager SavesManager;
        public Camera Camera;
        public int RecentSave;

        private static int _screenHeight;
        private static int _screenWidth;
        private static Vector2 _V2Transform;
        private State _currentState;
        private State _nextState;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        public static Random Random;

        public Dictionary<string, Item> Items;

        public int SaveIndex;

        public State CurrentState
        {
            get { return _currentState; }
        }

        public static Rectangle Viewport
        {
            get { return _viewport; }
        }

        public TextureManager Textures;

        public static Vector2 V2Transform
        {
            get { return _V2Transform; }
            set
            {
                _V2Transform = value;
                _viewport.X = (int)value.X;
                _viewport.Y = (int)value.Y;
            }
        }

        public GraphicsDeviceManager GraphicsManager
        {
            get { return _graphics; }
        }

        public string CurrentStateName
        {
            get
            {
                return _currentState.Name;
            }
        }

        public Inventory CurrentInventory
        {
            get
            {
                return SavesManager.ActiveSave.Inventory;
            }
        }

        public Save ActiveSave
        {
            get
            {
                return SavesManager.ActiveSave;
            }
        }

        #endregion

        #region Constructor/Inherited

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferHalfPixelOffset = true;
            _graphics.PreferMultiSampling = true;
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
            ResScale = (float)ScreenHeight / (float)DefaultHeight;

            RecentSave = SaveIndex = int.Parse(Settings.Settings.General["MostRecentSave"]);

            Random = new Random();

            SavesManager = new SaveManager(this);

            var keys = Input.SpecialKeyMap.Keys.ToList();
            var values = Input.SpecialKeyMap.Values.ToList();
            for (int i = 0; i < Input.SpecialKeyMap.Count; i++)
            {
                Input.KeysFromSpecialKey.Add(values[i], keys[i]);
            }

            PlayerKeys = new Input(Settings.Settings.InputValues, this);
            Player = new Player(Textures.Sprites["Player"].Statics["Standing"], PlayerKeys, this)
            {
                Layer = 0.75f,
                Position = new Vector2(100, 0),
            };

            BuffInfo.SetRedXTexture(Textures.RedX);

            foreach (var save in SavesManager.Saves)
            {
                if (save != null) 
                    save.Inventory.Owner = Player;
            }

            _mouse = new BorderedBox(Textures.BaseBackground, GraphicsDevice, Color.Purple, new Vector2(0, 0), 0.9f, 2, 2);

            Camera = new Camera();

            V2Transform = Vector2.Zero;

            ResetState();
        }

        //Loads textures and initial state (main menu)
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Textures = new TextureManager(Content, this);

            Items = Textures.LoadItems();

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
            PlayerKeys.Update();

            ChangeFullscreenMode();
            ChangeDebugMode();

            _currentState.Update(gameTime);

            //if you aren't in the main menu you may press escape to access settings and return to the main menu
            if (_currentState.Popups.Count == 0 && _currentState.Name != Names.MainMenu)
            {
                if (_currentKeys.IsKeyDown(Keys.Escape) && _previousKeys.IsKeyUp(Keys.Escape))
                {
                    var settings = new Settings(this, Content, _currentState, _graphics);
                    _currentState.Popups.Add(settings);
                    settings.LoadContent();
                    settings.LoadMenuButton();
                }
                else if (_currentState.Name != Names.CharacterInit && PlayerKeys.IsPressed("Menu", false))
                {
                    var options = new States.Popups.Game.Options(this, Content, _currentState);
                    _currentState.Popups.Add(options);
                    options.Layer = Player.Layer + 0.1f;
                    options.LoadContent();
                }
            }


            CenterCamera();

            base.Update(gameTime);

            if (InDebug)
                _mouse.Position = PlayerKeys.MousePosition + Game1.V2Transform;
        }

        private void CenterCamera()
        {
            Camera.Follow(Player);
            if (_currentState.Name.Contains("level"))
            {
                var cameraBounds = ((Level)_currentState).CameraBounds;
                if (Player.ScaledPosition.X <= cameraBounds.Min.X)
                    V2Transform = new Vector2(0, V2Transform.Y);
                else if (Player.ScaledPosition.X >= cameraBounds.Max.X)
                    V2Transform = new Vector2(cameraBounds.Max.X - 0.5f * ScreenWidth, V2Transform.Y);
            }
            if (_statesWithoutPlayer.Contains(_currentState.Name))
                V2Transform = Vector2.Zero;
        }

        public bool ToCull(Rectangle rect)
        {
            return Viewport.Left > rect.Right || 
                Viewport.Right < rect.Left ||
                Viewport.Bottom < rect.Top|| 
                Viewport.Top > rect.Bottom;
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        //Draws the textures on to the screen.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MidnightBlue);

            //Front to back means that textures with a lower Layer value will be drawn behind textures with higher Layer values.
            //By setting sampler state to PointClamp, no interpolation occurs when accessing Texture2D files, this was especially bad
            //when using my texture atlases as they would be prone to texture bleeding due to interpolation.
            //Transform matrix is to keep the character at the center of the screen.
            if (_currentState.Name.Contains(""))
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateTranslation(-V2Transform.X, -V2Transform.Y, 0));
            else
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

            _currentState.Draw(gameTime, _spriteBatch);

            if (InDebug)
                _mouse.Draw(gameTime, _spriteBatch);

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
        //the textures and popups have correctly scaled textures since LoadContent is only typically ran once
        public void ResetState()
        {
            ResScale = (float)ScreenHeight / (float)DefaultHeight;

            if (!_statesWithoutPlayer.Contains(_currentState.Name))
                CenterCamera();

            _currentState.LoadContent();

            foreach (var state in _currentState.Popups)
                state.LoadContent();

            if (_currentState.Name != Names.MainMenu && _currentState.Popups.Count > 0)
            {
                CenterCamera(); //This may seem redundant but when changing up resolution, without this, the settings menu becomes off center (to the right)
                Player.Reset();
                if (_currentState.Popups.Count > 0 && _currentState.Popups[^1].Name == Names.Settings )
                {
                    var settings = (_currentState.Popups[^1] as Settings);
                    settings.LoadContent();
                    settings.LoadMenuButton();
                }
            }

            Player.Reset();
        }

        public List<List<int>> RetrieveLevelMap(int level)
        {
            List<List<int>> map;
            using (var reader = new System.IO.StreamReader($"Content/Levels/Level{level}.txt"))
            {
                map = reader.ReadToEnd().Split("\n").Select(x => x.Split(',').Select(y => ProcessIndex(y)).ToList()).ToList();
            }
            return map;
        }

        private int ProcessIndex(string x) => int.TryParse(x, out var index) ? index : -1;

        public static Color BlendColors(Color color1, Color color2, float alpha)
        {
            alpha = MathHelper.Clamp(alpha, 0f, 1f);

            int r = (int)(color1.R * (1 - alpha) + color2.R * alpha);
            int g = (int)(color1.G * (1 - alpha) + color2.G * alpha);
            int b = (int)(color1.B * (1 - alpha) + color2.B * alpha);
            int a = (int)(color1.A * (1 - alpha) + color2.A * alpha);

            return new Color(r, g, b, a);
        }
        #endregion
    }
}
