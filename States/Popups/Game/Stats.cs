using Bound.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Bound.States.Popups.Game
{
    public class Stats : Popup
    {
        private List<Component> _components;
        private bool _enableEscape = false;
        private List<(string Text, Vector2 Position)> _playerStats = new List<(string Text, Vector2 Position)>();

        public float Layer;

        
        public Stats(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics) : base(game, content, parent, graphics)
        {
            Name = Game1.StateNames.StatsWindow;
            Layer = 0.79f;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var attribute in _playerStats)
                spriteBatch.DrawString(_game.Textures.Font, attribute.Text, attribute.Position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var statsBoxPos = new Vector2(eigthWidth + Game1.V2Transform.X, eigthHeight + Game1.V2Transform.Y);

            var statsBackground = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Color.BlanchedAlmond,
                statsBoxPos,
                Layer,
                eigthWidth * 3,
                eigthHeight  * 6
            );

            var border = 5 * Game1.ResScale;
            var textStartingPosition = new Vector2(statsBoxPos.X + border, statsBoxPos.Y + border);
            _playerStats.Add(( $"Name- {_game.Player.SaveState.PlayerName}", textStartingPosition ));

            var i = 2;
            var spacing = _game.Textures.Font.MeasureString("t").Y + 1 * Game1.ResScale;
            foreach (var attribute in _game.Player.Attributes)
            {
                _playerStats.Add(
                    (String.Format($"{attribute.Value.Name}- {attribute.Value.Value}"),
                    new Vector2(textStartingPosition.X, textStartingPosition.Y + spacing * i)));
                i++;
            }
            _components = new List<Component>() { statsBackground };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);


            if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape) 
                && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape))
                Button_Discard_Clicked(new object(), new EventArgs());

        }

        private void Button_Discard_Clicked(object v, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

    }
}
