using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls.Settings
{
    public class MultiChoiceBox : ChoiceBox
    {
        private Texture2D _texture;
        private BorderedBox _box;
        private SpriteFont _font;
        private List<Component> _components;
        private float _centerOfArrows; //the x co-ordinate of the center

        private Vector2 _textPosition;
        private Vector2 _leftArrowPosition;
        private Vector2 _rightArrowPosition;
        private BorderedBox _parentBackground;
        private Game1 _game;
        private float _allignment;

        public List<string> Choices;
        public string Text;
        public Vector2 Position;
        public float Layer;
        public float TextureScale;
        public int FullWidth;
        public int FullHeight;
        public Color PenColour;
        public Color ChoicePenColour;
        public int CurIndex;
        public int Order;
        public int yOffset;
        public string Type;

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, FullWidth, FullHeight);
            }
        }

        public Vector2 _choicePosition
        {
            get
            {
                return new Vector2(_centerOfArrows - _font.MeasureString(Choices[CurIndex]).X / 2, _textPosition.Y);
            }
        }

        public MultiChoiceBox(Texture2D texture, Texture2D arrow, SpriteFont font, int index)
        {
            _texture = arrow;
            _font = font;

            Choices = new List<string>()
            {
                "Null"
            };

            TextureScale = 0.2f;
            PenColour = ChoicePenColour = Color.Black;

            CurIndex = index;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (!string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(_font, Text, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            }

            var x = _leftArrowPosition.X + _texture.Width * (_components[1] as Button).Scale;

            spriteBatch.DrawString(_font, Choices[CurIndex], _choicePosition, ChoicePenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        public override void LoadContent(Game1 game, BorderedBox background, float allignment)
        {
            _parentBackground = background;
            _game = game;
            _allignment = allignment;
            Load(background, allignment);
        }

        private void Load(BorderedBox background, float allignment)
        {
            int longestChoice;
            float fullScale, arrowLength, gap;
            SetValues(out longestChoice, out fullScale, out arrowLength, out gap);

            Position = new Vector2
            (
                background.Position.X + allignment,
                background.Position.Y + background.Height / 6 - FullHeight / 2 + (FullHeight + 10 * Scale) * Order + yOffset * Scale
            );

            SetComponents(longestChoice, fullScale, arrowLength, gap);
        }

        private void SetComponents(int longestChoice, float fullScale, float arrowLength, float gap)
        {
            var dims = _font.MeasureString("X");

            _box = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Color.White,
                Position,
                Layer,
                FullWidth,
                FullHeight
            );

            _components.Add(_box);

            _textPosition = new Vector2(Position.X + 10, Position.Y + (_box.Height - dims.Y) / 2);

            _leftArrowPosition = new Vector2(_textPosition.X + _font.MeasureString(Text).X + gap, Position.Y + (FullHeight - _texture.Height * fullScale) / 2f);
            _rightArrowPosition = new Vector2(_leftArrowPosition.X + arrowLength + gap / 2 + longestChoice + gap / 2, _leftArrowPosition.Y);

            _components.Add(
                new Button(_texture, _font)
                {
                    Text = "",
                    Position = _leftArrowPosition,
                    Click = new EventHandler(LeftArrow_Clicked),
                    Layer = Layer + 0.1f,
                    TextureScale = TextureScale
                }
            );

            _components.Add(
                new Button(_texture, _font)
                {
                    Text = "",
                    Position = _rightArrowPosition,
                    Click = new EventHandler(RightArrow_Clicked),
                    Layer = Layer + 0.1f,
                    TextureScale = TextureScale,
                    Effect = SpriteEffects.FlipHorizontally
                }
            );

            _centerOfArrows = (_leftArrowPosition.X + _texture.Width * (_components[1] as Button).Scale + _rightArrowPosition.X) / 2;
        }

        private void SetValues(out int longestChoice, out float fullScale, out float arrowLength, out float gap)
        {
            longestChoice = Choices.Aggregate(0, (a, c) => (int)(a > _font.MeasureString(c).X ? a : _font.MeasureString(c).X));
            _components = new List<Component>();
            FullHeight = (int)(_font.MeasureString(Text).Y + 4 * Game1.ResScale);
            TextureScale = _font.MeasureString(Text).Y / (Game1.ResScale * _texture.Height);

            fullScale = TextureScale * Game1.ResScale;
            arrowLength = (float)(_texture.Width * 2.8 * fullScale);
            gap = 8f * Game1.ResScale;
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            FullWidth = (int)(_font.MeasureString(Text).X + gap + arrowLength * 2 + longestChoice + gap);
        }

        public void LoadContent(Game1 game, Vector2 center)
        {
            int longestChoice;
            float fullScale, arrowLength, gap;
            SetValues(out longestChoice, out fullScale, out arrowLength, out gap);

            _game = game;

            Position = new Vector2
            (
                center.X - (FullWidth / 2),
                center.Y - FullHeight / 2
            );

            SetComponents(longestChoice, fullScale, arrowLength, gap);
        }


        private void LeftArrow_Clicked(object sender, EventArgs e)
        {
            CurIndex--;
            if (CurIndex < 0)
                CurIndex = Choices.Count - 1;
        }
        private void RightArrow_Clicked(object sender, EventArgs e)
        {
            CurIndex++;
            if (CurIndex > Choices.Count - 1)
                CurIndex = 0;
        }

        public override void UpdatePosition(Vector2 position)
        {
            Position = position;
            Load(_parentBackground, _allignment);
        }
    }
}
