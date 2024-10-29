using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bound.Controls
{
    public class ScrollBox : MultiChoice
    {
        private Texture2D _texture;
        private Color _color;
        private SpriteFont _font;
        private List<Component> _components;

        private Vector2 _textPosition;
        private int _barLength;
        private int _barHeight;

        public string Name;
        public string Text;
        public Vector2 Position;
        public float Layer;
        public float TextureScale;
        public int FullWidth;
        public int FullHeight;
        public Color PenColour;
        public string CurValue;
        public int Order;

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        public ScrollBox(SpriteFont font, string name)
        {
            _font = font;
            _components = new List<Component>();

            Name = name;
            TextureScale = 1f;
            PenColour = Color.Black;

            CurValue = Game1.SettingsStates[name] + "%";
            int value;

            if (int.TryParse(CurValue, out value) && value > 100)
                CurValue = "100%";

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (!string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(_font, Text, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);
        }

        public override void LoadContent(Game1 _game, BorderedBox background)
        {
            var gap = 10f * Game1.ResScale;
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            TextureScale = 1f;

            _barLength = (int)(400 * Scale);



            FullHeight = (int)((_font.MeasureString(Text).Y + (10)));
            FullWidth = (int)(_font.MeasureString(Text).X + gap + _barLength + gap + _font.MeasureString(CurValue).X);

            Position = new Vector2((background.Position.X + (background.Width / 2)) - (FullWidth / 2), (background.Position.Y + (background.Height / 6) - (FullHeight / 2)) + (FullHeight + (10 * Scale)) * Order);
            _textPosition = new Vector2(Position.X + 2, Position.Y + 2);


            var barPos = new Vector2(Position.X + _font.MeasureString(Text).X + gap, Position.Y + (FullHeight / 4));
            _barHeight = (int)(FullHeight / 2);

            var bgBox = new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.White,
                    Position,
                    Layer - 0.1f,
                    FullWidth,
                    FullHeight
                );

            var scrollBox = new BorderedBox
                (
                    _game.BaseBackground,
                    _game.GraphicsDevice,
                    Color.Red,
                    barPos,
                    Layer + 0.1f,
                    _barLength,
                    _barHeight
                );

            _components.Add(bgBox);
            _components.Add(scrollBox);


        }
    }
}
