using Bound.Controls;
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
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var texture = _game.BaseBackground;
            var bbWidth = (int)(eigthWidth * 6);
            var bbHeight = (int)(eigthHeight * 6);
            var font = _game.Font;

            var volumeOffset = 50;

            int resBoxIndex = Game1.ScreenHeight switch
            {
                720 => 0,
                900 => 1,
                1080 => 2,
                1440 => 3,
                2160 => 4,
                _ => 2
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
            var allignments = new float[2] {15f, (bbWidth / 4) * 3};


            _components = new List<Component>()
            {
                background,
                new Button(_game.Button, font, background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                    xOffset = 2
                },
                new Button(_game.Button, font, background)
                {
                    Text = "Apply",
                    Click = new EventHandler(Button_Apply_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.5f,
                    xOffset = (int)(2 * Game1.ResScale),
                },

            };

            _multiBoxes = new List<MultiChoice>()
            {
                new MultiChoiceBox(texture, _game.ArrowLeft, font, resBoxIndex)
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
                new MultiChoiceBox(texture,_game.ArrowLeft, font, resBoxIndex)
                {
                    Text = "Fullscreen",
                    Choices = new List<string>()
                    {
                        "Yes",
                        "No"
                    },
                    Layer = 0.8f,
                    OnApply = new EventHandler(Fullscreen_Apply),
                    CurIndex = _graphics.IsFullScreen ? 0 : 1,
                    Order = 1,
                    Type = "Video"
                },
                new ScrollBox(font, "MasterVolume", 100f, "%")
                {
                    Text = "Master Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 2,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "MusicVolume", 100f, "%")
                {
                    Text = "Music Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 3,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "EnemyVolume", 100f, "%")
                {
                    Text = "Enemy Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 4,
                    yOffset = volumeOffset,
                },
                new ScrollBox(font, "PlayerVolume", 100f, "%")
                {
                    Text = "Player Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(Volume_Apply),
                    Order = 5,
                    yOffset = volumeOffset,
                },

            };

            _keyInputs = new List<KeyInput>();

            var longestInput = (int)(_game.PlayerKeys.Keys.Keys.Aggregate(0f, (a, c) => (a >= font.MeasureString(c).X) ? a : font.MeasureString(c).X));

            int acc = 0;
            foreach (var kvp in _game.PlayerKeys.Keys)
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
            float buttonHeight = _game.Button.Height * comp.Scale;
            float buttonWidth = _game.Button.Width * comp.Scale;
            //possibly spaghetti code
            comp.RelativePosition = new Vector2(((bbWidth - buttonWidth) / 2) - (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

            comp = _components[2] as Button;
            comp.RelativePosition = new Vector2(((bbWidth - buttonWidth) / 2) + (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

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

        #region Other Methods
        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }
        private void Button_Apply_Clicked(object sender, EventArgs e)
        {
            ApplyChanges();

            _game.ResetState();
        }

        private void ApplyChanges()
        {
            foreach (var box in _multiBoxes)
                box.OnApply?.Invoke(box, EventArgs.Empty);

            foreach (var keyBox in _keyInputs)
                keyBox.OnApply?.Invoke(keyBox, EventArgs.Empty);

            _graphics.ApplyChanges();
        }
        private void Resolution_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;
            var resolution = box.Choices[box.CurIndex].Split('x').Select(x => int.Parse(x)).ToList();

            Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = resolution[0];
            Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = resolution[1];
        }

        private void Fullscreen_Apply(object sender, EventArgs e)
        {
            var box = sender as MultiChoiceBox;
            var isFullscreen = (box.Choices[box.CurIndex] == "Yes") ? true : false;

            _graphics.IsFullScreen = isFullscreen;

            if (isFullscreen)
            {
                Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
        }

        private void Volume_Apply(object sender, EventArgs e)
        {
            var box = sender as ScrollBox;
            Game1.SettingsStates[box.Name] = box.CurValue.Substring(0, box.CurValue.Length - 1);
        }

        private void Key_Apply(object sender, EventArgs e)
        {
            var ki= sender as KeyInput;
            _game.PlayerKeys.Keys[ki.Keys.Key] = ki.Key;
        }

        #endregion
    }
}
