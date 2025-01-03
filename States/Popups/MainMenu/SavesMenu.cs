using Bound.Controls;
using Bound.Managers;
using Bound.States.Game;
using Bound.States.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;



namespace Bound.States
{
    public class SavesMenu : Popup
    {

        private List<Component> _components;

        public SavesMenu(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content, parent, graphics)
        {
            Name = "savesmenu";
        }

        public override void LoadContent()
        {
            var button = _game.Textures.Button;
            var font = _game.Textures.Font;
            var bbWidth = (int)(Game1.ScreenWidth / 4 * 1.5f);
            var bbHeight = (int)(Game1.ScreenHeight / 8 * 6);

            var background = new BorderedBox
            (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.BlanchedAlmond,
                    new Vector2((Game1.ScreenWidth  - bbWidth) / 2 , (Game1.ScreenHeight  - bbHeight) / 2 ),
                    0.6f,
                    bbWidth,
                    bbHeight
            );

            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, font, background)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = 0.8f,
                    TextureScale = 1.75f,
                    RelativePosition = new Vector2(
                        bbWidth / 2  - _game.Textures.Button.Width * (1.75f * Game1.ResScale) / 2,
                        bbHeight - _game.Textures.Button.Height * (1.75f * Game1.ResScale) - (15f * Game1.ResScale) ),
                }
            };

            for (int i = 1; i < 6; i++)
            {
                _components.Add(new SaveInterface(_game.Textures.BaseBackground, font, _game)
                {
                    Text = "Save " + i.ToString(),
                    Layer = 0.8f,
                    Play = new EventHandler(Button_Play_Clicked)
                });
                var ss = _components[^1] as SaveInterface;
                ss.LoadContent(_game, background, i - 1);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in _components)
                comp.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = _components.Count - 5; i < _components.Count; i++)
            {
                var comp = _components[i] as SaveInterface;
                if (comp.StopUpdate)
                {
                    comp.Update(gameTime);
                    return;
                }
            }

            foreach(var comp in _components)
                comp.Update(gameTime);
        }

        #region Clicked Methods

        private void Button_Discard_Clicked(object sender, EventArgs e)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Play_Clicked(object sender, EventArgs e)
        {
            var sen = sender as SaveInterface;
            _game.RecentSave = sen.Index;
            _game.Settings.Settings.General["MostRecentSave"] = sen.Index.ToString();
            SettingsManager.Save(_game.Settings);
            _game.ChangeState(_game.SavesManager.GetState(sen.Index, _game, _content, _graphics));
        }

        #endregion
    }
}
