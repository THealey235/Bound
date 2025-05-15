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

        public Vector2 _choicePosition
        {
            get
            {
                return new Vector2(_centerOfArrows - _font.MeasureString(Choices[CurIndex]).X / 2, Position.Y + 5);
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
            //TODO: rewrite BorderdBox constructor so i may rewrite this method
            //Note: this code is some hot garbage

            var longestChoice = Choices.Aggregate(0, (a, c) => (int)(a > _font.MeasureString(c).X ? a : _font.MeasureString(c).X));

            _components = new List<Component>();

            var fullScale = TextureScale * Game1.ResScale;
            var arrowLength = (float)(_texture.Width * 2.8 * fullScale);
            var gap = 8f * Game1.ResScale;
            //Dont ask me why it needs this, idc anymore: who even uses 720p anyway?
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            var longestName = 0;
            if (Type == "Video")
                longestName = (int)_font.MeasureString(Text).X;

            FullWidth = (int)(longestName + gap + arrowLength * 2 + longestChoice + gap);
            FullHeight = (int)(_font.MeasureString(Text).Y + 10);
            Position = new Vector2
            (
                background.Position.X + allignment,
                background.Position.Y + background.Height / 6 - FullHeight / 2 + (FullHeight + 10 * Scale) * Order + yOffset * Scale
            );

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

            _textPosition = new Vector2(Position.X + 10, Position.Y + 5);

            _leftArrowPosition = new Vector2(_textPosition.X + _font.MeasureString(Text).X + gap, Position.Y + (FullHeight - _texture.Height * fullScale) / 2f);
            _rightArrowPosition = new Vector2(_leftArrowPosition.X + arrowLength + gap + longestChoice + gap, Position.Y + (FullHeight - _texture.Height * fullScale) / 2f);

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

        public void LoadContent(Game1 _game, Vector2 center)
        {

            var longestChoice = Choices.Aggregate(0, (a, c) => (int)(a > _font.MeasureString(c).X ? a : _font.MeasureString(c).X));

            _components = new List<Component>();
            var fullScale = TextureScale * Game1.ResScale;
            var arrowLength = _texture.Width * 2 * fullScale;
            var gap = 8f * fullScale;
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            var longestName = (int)_font.MeasureString(Text).X;

            FullWidth = (int)(longestName + gap + arrowLength * 2 + longestChoice + gap);
            FullHeight = (int)(_font.MeasureString(Text).Y + 10);
            Position = new Vector2
            (
                center.X - (FullWidth / 2),
                center.Y - FullHeight / 2
            );

            _box = new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.White,
                    Position,
                    Layer - 0.1f,
                    FullWidth,
                    FullHeight
                );

            _components.Add(_box);

            _textPosition = new Vector2(Position.X + 10, Position.Y + 5);

            _leftArrowPosition = new Vector2(_textPosition.X + _font.MeasureString(Text).X + gap, Position.Y + (FullHeight - _texture.Height * fullScale) / 2f);
            _rightArrowPosition = new Vector2(_leftArrowPosition.X + arrowLength + gap + longestChoice + gap, Position.Y + (FullHeight - _texture.Height * fullScale) / 2f);

            _components.Add(
                new Button(_texture, _font)
                {
                    Text = "",
                    Position = _leftArrowPosition,
                    Click = new EventHandler(LeftArrow_Clicked),
                    Layer = Layer + 0.1f,
                    TextureScale = TextureScale,
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
