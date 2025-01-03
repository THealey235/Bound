using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Bound.Controls
{
    public class ConfirmBox : Component
    {
        private List<Component> _components;
        private Game1 _game;
        private SpriteFont _font;

        public Vector2 Position;
        public Vector2 TextPosition;
        public Color Colour;
        public Color PenColour;
        public string Text;
        public float Layer;
        public bool IsConfirmed;
        public bool IsYes;
        public string ParentName;

        public ConfirmBox(Game1 game, SpriteFont font, string parentName)
        {
            _game = game;
            Colour = Color.White;
            PenColour = Color.Black;
            Layer = 0.9f;
            _font = font;
            Text = "Are you sure?";
            ParentName = parentName;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var c in _components)
                c.Draw(gameTime,spriteBatch);

            spriteBatch.DrawString(_font, Text, TextPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);

        }

        public void LoadContent()
        {
            var width = (int)((5f * Game1.ResScale) * 2 + _font.MeasureString(Text).X * 1.15f);
            var height = Game1.ScreenHeight / 8;
            var textureScale = 0.4f;
            var scale = textureScale * Game1.ResScale;
            //2f extra because it needs it for some reason
            var buttonWidth = _game.Textures.Button.Width * scale * 2f;

            Position = new Vector2(Game1.ScreenWidth / 2 - width / 2, Game1.ScreenHeight / 2 - height / 2);
            var centre = new Vector2(Position.X + (width / 2), Position.Y + (height / 2));
            TextPosition = new Vector2(centre.X - (_font.MeasureString(Text).X / 2) + 3f * Game1.ResScale, Position.Y + (5f * Game1.ResScale));

            _components = new List<Component>()
            {
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Colour,
                    Position,
                    Layer,
                    width,
                    height
                ),
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Yes",
                    Position = new Vector2(centre.X - (buttonWidth / 2) - (10f * scale) , centre.Y - (5f * scale)),
                    Click = new EventHandler(Button_Yes_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale,
                    PenColour = Color.Green,
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "No",
                    Position = new Vector2(centre.X + (10f * scale) , centre.Y - (5f * scale)),
                    Click = new EventHandler(Button_No_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale,
                    PenColour = Color.Red,
                }
            };
        }

        private void Button_No_Clicked(object sender, EventArgs e)
        {
            IsConfirmed = true;
            IsYes = false;
        }

        private void Button_Yes_Clicked(object sender, EventArgs e)
        {
            IsConfirmed = true;
            IsYes = true;
        }
    }

}
