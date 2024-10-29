﻿using Bound.Controls;
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
            var bbWidth = (int)(eigthWidth * 4);
            var bbHeight = (int)(eigthHeight * 4);
            var font  = _game.Font;

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
                    new Vector2(eigthWidth * 2, eigthHeight * 2),
                    0.6f,
                    bbWidth,
                    bbHeight
                );


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
                    Order = 0
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
                    Order = 1
                },
                new ScrollBox(font, "MasterVolume", 100f, "%")
                {
                    Text = "Master Volume",
                    Layer = 0.8f,
                    OnApply = new EventHandler(MasterVolume_Apply),
                    Order = 2,
                }

            };

            Button comp;

            comp = _components[1] as Button;
            float buttonHeight = _game.Button.Height * comp.Scale;
            float buttonWidth = _game.Button.Width * comp.Scale;
            //possibly spaghetti code
            comp.CustomPosition = new Vector2(((bbWidth - buttonWidth) / 2) - (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

            comp = _components[2] as Button;
            comp.CustomPosition = new Vector2(((bbWidth - buttonWidth) / 2) + (buttonWidth + (5 * comp.Scale)), bbHeight - (buttonHeight + (15 * comp.Scale)));

            for(int i = 0; i < _multiBoxes.Count; i++ )
            {
                _multiBoxes[i].LoadContent(_game, background);
            }

        }

        

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            foreach (var box in _multiBoxes)
                box.Update(gameTime);

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
                box.OnApply?.Invoke(this, EventArgs.Empty);

            _graphics.ApplyChanges();
        }
        private void Resolution_Apply(object sender, EventArgs e)
        {
            var box = _multiBoxes[0] as MultiChoiceBox;
            var resolution = box.Choices[box.CurIndex].Split('x').Select(x => int.Parse(x)).ToList();

            Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = resolution[0];
            Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = resolution[1];
        }

        private void Fullscreen_Apply(object sender, EventArgs e)
        {
            var box = _multiBoxes[1] as MultiChoiceBox;
            var isFullscreen = (box.Choices[box.CurIndex] == "Yes") ? true : false;

            _graphics.IsFullScreen = isFullscreen;

            if (isFullscreen)
            {
                Game1.ScreenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Game1.ScreenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
        }

        private void MasterVolume_Apply(object sender, EventArgs e)
        {
            var box = _multiBoxes[2] as ScrollBox;
            Game1.SettingsStates["MasterVolume"] = box.CurValue.Substring(0, box.CurValue.Length - 1);
        }

        #endregion
    }
}
