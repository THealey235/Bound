using Bound.Controls;
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
    public class Settings : State
    {
        #region Properties and Fields

        private List<Component> _components;
        private List<MultiChoice> _multiBoxes;
        private List<KeyInput> _keyInputs;

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;
        private GraphicsDeviceManager _graphics;

        public State Parent;

        #endregion

        #region Constructor / Inherited
        public Settings(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content)
        {
            Parent = parent;
            _graphics = graphics;
        }

        public override void LoadContent()
        {
            var texture = _game.Textures.BaseBackground;
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bbWidth = (int)(eigthWidth * 6);
            var bbHeight = (int)(eigthHeight * 6);
            var font = _game.Textures.Font;

            var volumeOffset = 50;

            //LMAO SETTINGS.SETTINGS.SETTINGS WTF IS THIS
            var generalSettings = _game.Settings.Settings.General;

            int resBoxIndex = int.Parse(generalSettings["Resolution"].Split("x")[1]) switch
            {
                720 => 0,
                900 => 1,
                1080 => 2,
                1440 => 3,
                _ => 4
            };

            var background = new BorderedBox
                (
                    texture,
                    _game.GraphicsDevice,
                    Color.BlanchedAlmond,
                    new Vector2(eigthWidth, eigthHeight),
                    0.6f,
                    bbWidth,
                    bbHeight
                );


            //0 : left allignemt, 1: right allignment
            var allignments = new float[2] {15f, (eigthHeight / 2) + eigthHeight * 6};


            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                },
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Apply",
                    Click = new EventHandler(Button_Apply_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                },
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Reset",
                    Click = new EventHandler(Button_Reset_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                }

            };

            _multiBoxes = new List<MultiChoice>()
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
                    Layer = 0.8f,
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
                    Layer = 0.8f,
                    OnApply = new EventHandler(Fullscreen_Apply),
                    Order = 1,
                    Type = "Video"
                },
                //new MultiChoiceBox(texture, _game.ArrowLeft, font, _game.Settings.Settings.General["DefaultMouse"] == "Yes" ? 0 : 1)
                //{
                //    Text = "Custom Cursor",
                //    Choices = new List<string>()
                //    {
                //        "Yes",
                //        "No"
                //    },
                //    Layer = 0.8f,
                //    OnApply = new EventHandler(CustomCursor_Apply),
                //    Order = 2,
                //    Type = "Video"
                //},
                new ScrollBox(font, "MasterVolume", 100f, "%", _game)
                {
                    Text = "Master Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 2,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "MusicVolume", 100f, "%", _game)
                {
                    Text = "Music Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 3,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "EnemyVolume", 100f, "%", _game)
                {
                    Text = "Enemy Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 4,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "PlayerVolume", 100f, "%", _game)
                {
                    Text = "Player Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 5,
                    yOffset = volumeOffset,
                },

            };

            _keyInputs = new List<KeyInput>();

            var longestInput = (int)(_game.Settings.Settings.InputValues.Keys.Aggregate(0f, (a, c) => (a >= font.MeasureString(c).X) ? a : font.MeasureString(c).X));

            int acc = 0;
            foreach (var kvp in _game.Settings.Settings.InputValues)
            {
                _keyInputs.Add
                (
                     new KeyInput(font, kvp, longestInput)
                     {
                         Layer = 0.8f,
                         Order = acc,
                         OnApply = new EventHandler(Key_Apply)
                     }
                ); 
                acc++;
            }

            LoadNestedContent(bbWidth, bbHeight, background, allignments);

        }


        private void LoadNestedContent(int bbWidth, int bbHeight, BorderedBox background, float[] allignments)
        {
            var comp = _components[1] as Button;
            float buttonHeight = _game.Textures.Button.Height * comp.Scale;
            float buttonWidth = _game.Textures.Button.Width * comp.Scale;
            //possibly spaghetti code

            var thirdTotalSpace = (float)(((bbWidth - buttonWidth) / 2) + (buttonWidth * 1.5f + (10 * comp.Scale))) / 3f;

            for (int i = 1; i < _components.Count; i++)
            {
                comp = _components[i] as Button;
                comp.RelativePosition = new Vector2(((bbWidth - buttonWidth) / 2) - (buttonWidth * 1.5f + (10 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));
                comp.xOffset = (int)(thirdTotalSpace * (i - 1));
            }

            for (int i = 0; i < _multiBoxes.Count; i++)
            {
                _multiBoxes[i].LoadContent(_game, background, allignments[0]);
            }

            foreach (var key in _keyInputs)
                key.LoadContent(_game, background, allignments[1]);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            foreach (var box in _multiBoxes)
                box.Update(gameTime);

            foreach(var key in _keyInputs)
                key.Update(gameTime);

            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();
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

            foreach (var key in _keyInputs)
                key.Draw(gameTime, spriteBatch);
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
            _game.Settings.Settings.InputValues[ki.Keys.Key] = ki.Key;
        }

        private void CustomCursor_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;
            _game.UseDefaultMouse = (box.CurIndex == 1) ? true : false;
            _game.IsMouseVisible = (box.CurIndex == 1) ? true : false;
            _game.Settings.Settings.General["DefaultMouse"] = (box.CurIndex == 1) ? "Yes" : "No";
        }

        #endregion
    }
}
