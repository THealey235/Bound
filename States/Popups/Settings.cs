using Bound.Controls;
using Bound.Controls.Game;
using Bound.Controls.Settings;
using Bound.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.States.Popups
{
    public class Settings : Popup
    {
        #region Properties and Fields

        private List<Component> _components;
        private List<ChoiceBox> _multiBoxes;
        private List<KeyInput> _keyInputs;
        private BorderedBox _background;
        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;
        private ScrollingMenu _scrollingKeyInputs;
        private bool _enableEscape;

        #endregion

        #region Constructor / Inherited
        public Settings(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content, parent, graphics)
        {
            Parent = parent;
            _graphics = graphics;
            Name = Game1.Names.Settings;
            _enableEscape = false;
        }

        public override void LoadContent()
        {
            var texture = _game.Textures.BaseBackground;
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bbWidth = (int)(eigthWidth * 6);
            var bbHeight = (int)(eigthHeight * 6);
            var font = _game.Textures.Font;
            var layer = 0.79f;

            var volumeOffset = 40;

            var generalSettings = _game.Settings.Settings.General;

            int resBoxIndex = int.Parse(generalSettings["Resolution"].Split("x")[1]) switch
            {
                720 => 0,
                900 => 1,
                1080 => 2,
                1440 => 3,
                _ => 4
            };

            _background = new BorderedBox
                (
                    texture,
                    _game.GraphicsDevice,
                    Game1.MenuColorPalette[0],
                    new Vector2(eigthWidth + Game1.V2Transform.X, eigthHeight + Game1.V2Transform.Y),
                    layer,
                    bbWidth,
                    bbHeight
                );


            //0 : left allignemt, 1: right allignment
            var allignments = new float[2] {15f, (eigthHeight / 2) + eigthHeight * 6.75f};


            _components = new List<Component>()
            {
                _background,
                new Button(_game.Textures.Button, font, _background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = layer + 0.001f,
                    TextureScale = 0.6f,
                },
                new Button(_game.Textures.Button, font, _background)
                {
                    Text = "Apply",
                    Click = new EventHandler(Button_Apply_Clicked),
                    Layer = layer + 0.001f,
                    TextureScale = 0.6f,
                },
                new Button(_game.Textures.Button, font, _background)
                {
                    Text = "Reset",
                    Click = new EventHandler(Button_Reset_Clicked),
                    Layer = layer + 0.001f,
                    TextureScale = 0.6f,
                }

            };

            _multiBoxes = new List<ChoiceBox>()
            {
                new MultiChoiceBox(texture, _game.Textures.ArrowLeft, font, resBoxIndex)
                {
                    Text = "Resolution",
                    Choices = new List<string>()
                    {
                        "1280x720",
                        "1600x900",
                        "1920x1080",
                        "2560x1440",
                        "3840x2160"
                    },
                    Layer = layer + 0.001f,
                    OnApply = new EventHandler(Resolution_Apply),
                    Order = 0,
                    Type = "Video",
                },
                new MultiChoiceBox(texture,_game.Textures.ArrowLeft, font, _game.Settings.Settings.General["Fullscreen"] == "Yes" ? 0 : 1)
                {
                    Text = "Fullscreen",
                    Choices = new List<string>()
                    {
                        "Yes",
                        "No"
                    },
                    Layer = layer + 0.01f,
                    OnApply = new EventHandler(Fullscreen_Apply),
                    Order = 1,
                    Type = "Video"
                },
                new ScrollBox(font, "MasterVolume", 100f, "%", _game)
                {
                    Text = "Master Volume",
                    Layer = layer + 0.09f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 2,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "MusicVolume", 100f, "%", _game)
                {
                    Text = "Music Volume",
                    Layer = layer + 0.09f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 3,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "EnemyVolume", 100f, "%", _game)
                {
                    Text = "Enemy Volume",
                    Layer = layer + 0.09f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 4,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "PlayerVolume", 100f, "%", _game)
                {
                    Text = "Player Volume",
                    Layer = layer + 0.09f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 5,
                    yOffset = volumeOffset,
                },

            };

            _keyInputs = new List<KeyInput>();

            var longestInput = (int)(_game.Settings.Settings.InputValues.Keys.Aggregate(0f, (a, c) => (a >= font.MeasureString(c).X) ? a : font.MeasureString(c).X));

            var acc = 0;
            foreach (var kvp in _game.Settings.Settings.InputValues)
            {
                _keyInputs.Add
                (
                     new KeyInput(font, kvp, longestInput)
                     {
                         Layer = layer + 0.0001f,
                         Order = acc,
                         OnApply = new EventHandler(Key_Apply)
                     }
                );
                acc++;
            }

            var scrollingMenuPosition = new Vector2
            (
                _background.Position.X + allignments[1],
                _background.Position.Y + _background.Height * 0.05f
            );

            var gap = 5f * Game1.ResScale;

            _scrollingKeyInputs = new ScrollingMenu(
                _game,
                "keyInputs",
                scrollingMenuPosition,
                (int)(5 * Game1.ResScale + longestInput + gap + _game.Textures.Button.Width * _keyInputs[0].Scale + gap + 6f * Game1.ResScale),
                (int)(_background.Height * 0.8f),
                layer + 0.0001f,
                _keyInputs.Select(x => x as ChoiceBox).ToList(),
                (int)(_game.Textures.Button.Height * _keyInputs[0].Scale + 10 * _keyInputs[0].Scale + 5 * Game1.ResScale),
                (int)_background.Position.Y
            );

            LoadNestedContent(bbWidth, bbHeight, _background, allignments);
        }


        private void LoadNestedContent(int bbWidth, int bbHeight, BorderedBox background, float[] allignments)
        {
            SetButtonPositions(bbWidth, bbHeight);

            for (int i = 0; i < _multiBoxes.Count; i++)
            {
                _multiBoxes[i].LoadContent(_game, background, allignments[0]);
            }

            var acc = 0;
            foreach (var key in _keyInputs)
            {
                //in all honesty this is spaghetti
                key.LoadContent(_game, background, allignments[1]);
                acc++;
            }

            //This also invokes ScrollingMenu.Load()
            _scrollingKeyInputs.ElementYSpacing = _keyInputs[0].FullHeight + 2.5f * Game1.ResScale;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            foreach (var box in _multiBoxes)
                box.Update(gameTime);

            _scrollingKeyInputs.Update(gameTime);

            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();

            if (_enableEscape)
            {
                if (_currentKeys.IsKeyUp(Keys.Escape) && _previousKeys.IsKeyDown(Keys.Escape))
                    Button_Discard_Clicked(new object(), new EventArgs());
            }
            else
                if (_currentKeys.IsKeyUp(Keys.Escape)) _enableEscape = true;
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var box in _multiBoxes)
                box.Draw(gameTime, spriteBatch);

            _scrollingKeyInputs.Draw(gameTime, spriteBatch);
        }

        #endregion

        #region Clicked Methods
        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }
        private void Button_Apply_Clicked(object sender, EventArgs e)
        {
            ApplyChanges();

            _game.ResetState();

            if (_game.CurrentStateName != Game1.Names.MainMenu)
            {
                
            }
        }
        private void Button_Reset_Clicked(object sender, EventArgs e)
        {
            _game.Settings.Init();

            this.LoadContent();

            Button_Apply_Clicked(sender, e);
        }

        private void ApplyChanges()
        {
            foreach (var box in _multiBoxes)
                box.OnApply?.Invoke(box, EventArgs.Empty);

            foreach (var keyBox in _keyInputs)
                keyBox.OnApply?.Invoke(keyBox, EventArgs.Empty);

            _graphics.ApplyChanges();
            SettingsManager.Save(_game.Settings);
        }
        private void Resolution_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;
            var resolution = box.Choices[box.CurIndex].Split('x').Select(x => int.Parse(x)).ToList();

            Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = resolution[0];
            Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = resolution[1];
            _game.Settings.UpdateResolution(resolution[0], resolution[1]);
        }

        private void Fullscreen_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;

            _game.Settings.Settings.General["Fullscreen"] = box.Choices[box.CurIndex];
            var isFullscreen = (box.Choices[box.CurIndex] == "Yes") ? true : false;

            _graphics.IsFullScreen = isFullscreen;

            if (isFullscreen)
            {
                Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _game.Settings.UpdateResolution(Game1.ScreenWidth, Game1.ScreenHeight);
            }
        }

        private void Volume_Apply(object sender, EventArgs e)
        {
            var box = sender as ScrollBox;
            _game.Settings.Settings.General[box.Name] = box.CurValue.Substring(0, box.CurValue.Length - 1);
        }

        private void Key_Apply(object sender, EventArgs e)
        {
            var ki = sender as KeyInput;
            _game.Settings.Settings.InputValues[ki.Keys.Key] = _game.PlayerKeys.Keys[ki.Keys.Key] = ki.Key;
        }

        private void CustomCursor_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;
            _game.UseDefaultMouse = (box.CurIndex == 1) ? true : false;
            _game.IsMouseVisible = (box.CurIndex == 1) ? true : false;
            _game.Settings.Settings.General["DefaultMouse"] = (box.CurIndex == 1) ? "Yes" : "No";
        }
        private void Button_Quit_Clicked(object sender, EventArgs e)
        {
            _game.SavesManager.UploadSave(_game.RecentSave);
            _game.ChangeState(new MainMenu(_game, _content, _graphics));
        }
        #endregion

        #region Other Loads

        public void LoadMenuButton()
        {
            var model = _components[1] as Button;
            _components.Insert(1, new Button(_game.Textures.Button, _game.Textures.Font, _background)
            {
                Text = "Quit",
                Click = new EventHandler(Button_Quit_Clicked),
                Layer = model.Layer,
                TextureScale = model.TextureScale,
            });

            SetButtonPositions(_background.Width, _background.Height);
        }

        private void SetButtonPositions(int bbWidth, int bbHeight)
        {
            var buttonNum = _components.Count - 1;
            var comp = _components[1] as Button;
            float buttonHeight = _game.Textures.Button.Height * comp.Scale;
            float buttonWidth = _game.Textures.Button.Width * comp.Scale;
            
            var middleIndex = ((buttonNum - 1) / 2f) + 1; // add 1 since i start iterating through at 1 not 0
            var midPoint = _background.Width / 2;
            var gap = 15f;
            for (int i = 1; i < _components.Count; i++)
            {
                comp = _components[i] as Button;
                var d = Math.Round((double)middleIndex - i, MidpointRounding.AwayFromZero);
                var x = 0d;
                if (middleIndex % 2 == 0)
                    if (d >= 0)
                        x = midPoint - ((d * gap) + (0.5 * buttonWidth) + (d * buttonWidth));
                    else
                        x = midPoint + (0.5 * buttonWidth) + gap + ((d * -1 - 1) * buttonWidth);
                else
                    if (d > 0)
                        x = midPoint - ((0.5 * gap) + ((d - 1) * gap) + (d * buttonWidth));
                    else
                        x = midPoint + (0.5 * gap) + (d * -1 - 1) * (gap + buttonWidth);

                comp.RelativePosition = new Vector2((float)x, bbHeight - (buttonHeight + (15 * comp.Scale)));
            }
        }

        #endregion
    }
}
