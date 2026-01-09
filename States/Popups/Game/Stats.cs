using Bound.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;

namespace Bound.States.Popups.Game
{
    public class Stats : Popup
    {
        private List<Component> _components;
        private List<(string Text, Vector2 Position)> _playerStats = new List<(string Text, Vector2 Position)>();
        public float Layer;
        public Color PenColor = Game1.MenuColorPalette[2];
        
        public Stats(Game1 game, ContentManager content, State parent) : base(game, content, parent)
        {
            Name = Game1.Names.StatsWindow;
            Layer = 0.79f;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var attribute in _playerStats)
                spriteBatch.DrawString(_game.Textures.Font, attribute.Text, attribute.Position, PenColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
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
                Game1.MenuColorPalette[0],
                statsBoxPos,
                Layer,
                (int)(eigthWidth * 2.5),
                eigthHeight  * 6
            );

            var border = 5 * Game1.ResScale;
            var textStartingPosition = new Vector2(statsBoxPos.X + border, statsBoxPos.Y + border);
            var stringLength = (int)(statsBackground.Width / (_game.Textures.Font.MeasureString("X").X * 1.05));
            _playerStats.Add(( $"Name{_game.Player.Save.PlayerName.PadLeft(stringLength - 4)}", textStartingPosition ));

            var i = 2;
            var spacing = _game.Textures.Font.MeasureString("t").Y + 1 * Game1.ResScale;
            foreach (var attribute in _game.Player.Attributes.Dictionary)
            {
                _playerStats.Add(
                    ($"{attribute.Key}{attribute.Value.ToString().PadLeft(stringLength - attribute.Key.Length)}",
                    new Vector2(textStartingPosition.X, textStartingPosition.Y + spacing * i)));
                i++;
            }
            _components = new List<Component>() 
            { 
                statsBackground,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(statsBackground.Width / 2 - (int)(_game.Textures.Button.Width * Game1.ResScale * 0.75f) / 2, (statsBackground.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = statsBackground
                },
            };
            
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

    }
}
