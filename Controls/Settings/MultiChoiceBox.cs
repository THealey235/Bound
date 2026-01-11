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
        private List<Component> _arrows = new List<Component>();
        private float _centerOfArrows; //the x co-ordinate of the center

        private Vector2 _textPosition;
        private Vector2 _leftArrowPosition;
        private Vector2 _rightArrowPosition;
        private BorderedBox _parentBackground;
        private Game1 _game;
        private float _allignment;
        private bool _isBordered;
        private bool _isCarousel;

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

        public Vector2 ChoicePosition
        {
            get
            {
                return new Vector2(_centerOfArrows - _font.MeasureString(Choices[CurIndex]).X / 2, _textPosition.Y);
            }
        }

        public string CurrentChoice
        {
            get { return Choices[CurIndex]; }
        }

        public Color BackgroundColour
        {
            get { return _box.Colour; }
            set 
            {  
                if (_box != null)
                    _box.Colour = value; 
            }
        }

        public MultiChoiceBox(Texture2D bgTexture, Texture2D arrow, SpriteFont font, int index, bool border = true, bool isCarousel = false, Color? choiceColour = null, int fullWidth = 0)
        {
            _texture = arrow;
            _font = font;

            Choices = new List<string>()
            {
                "Null"
            };

            TextureScale = 0.2f;
            PenColour = Color.Black;
            ChoicePenColour = (choiceColour == null) ? Color.Black : (Color)choiceColour;

            CurIndex = index;
            _isBordered = border;
            _isCarousel = isCarousel;
            FullWidth = fullWidth;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (_isCarousel)
            {
                if (CurIndex != 0)
                    _arrows[0].Draw(gameTime, spriteBatch);
                if (CurIndex + 1 != Choices.Count)
                    _arrows[1].Draw(gameTime, spriteBatch);
            }
            else _arrows.ForEach(x => x.Draw(gameTime, spriteBatch));

            spriteBatch.DrawString(_font, Text, _textPosition, PenColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
            spriteBatch.DrawString(_font, Choices[CurIndex], ChoicePosition, (_isCarousel && CurIndex > 0 ? ChoicePenColour : PenColour), 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.01f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            if (_isCarousel)
            {
                if (CurIndex != 0)
                    _arrows[0].Update(gameTime);
                if (CurIndex + 1 != Choices.Count)
                    _arrows[1].Update(gameTime);
            }
            else _arrows.ForEach(x => x.Update(gameTime));
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
            )
            {
                IsBordered = _isBordered
            };

            _components = new List<Component>() { _box };

            _textPosition = new Vector2(Position.X + 3 * Game1.ResScale, Position.Y + (_box.Height - dims.Y) / 2);

            //positions calculated from the end of control in the case of a fixed full width which leads to the gap between the Text and arrows/choice to be variable
            _rightArrowPosition = Position + new Vector2(FullWidth - (gap / 1.5f) - _texture.Width * fullScale, (FullHeight - _texture.Height * fullScale) / 2f);
            _leftArrowPosition = new Vector2(_rightArrowPosition.X - (gap * 2) - (_texture.Width * fullScale) - longestChoice, _rightArrowPosition.Y);

            _arrows.Add(
                new Button(_texture, _font)
                {
                    Text = "",
                    Position = _leftArrowPosition,
                    Click = new EventHandler(LeftArrow_Clicked),
                    Layer = Layer + 0.1f,
                    TextureScale = TextureScale
                }
            );

            _arrows.Add(
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

            _centerOfArrows = (_leftArrowPosition.X + _texture.Width * (_arrows[0] as Button).Scale + _rightArrowPosition.X) / 2;
        }

        private void SetValues(out int longestChoice, out float fullScale, out float arrowLength, out float gap)
        {
            longestChoice = Choices.Aggregate(0, (a, c) => (int)(a > _font.MeasureString(c).X ? a : _font.MeasureString(c).X));
            _components = new List<Component>();
            FullHeight = (int)(_font.MeasureString(Text).Y + 4 * Game1.ResScale);
            TextureScale = _font.MeasureString(Text).Y / (Game1.ResScale * _texture.Height);

            fullScale = TextureScale * Game1.ResScale;
            arrowLength = (float)(_texture.Width * 2 * fullScale);
            gap = 8f * Game1.ResScale;
            if (Game1.ScreenHeight == 720)
                gap -= 3;

            if (FullWidth == 0)
                FullWidth = (int)(_font.MeasureString(Text).X + gap + arrowLength * 2 + longestChoice + gap);
        }

        public void LoadContent(Game1 game, Vector2 position, bool isCenter = false)
        {
            int longestChoice;
            float fullScale, arrowLength, gap;
            SetValues(out longestChoice, out fullScale, out arrowLength, out gap);

            _game = game;

            if (isCenter)
            {
                Position = new Vector2
                (
                    position.X - (FullWidth / 2),
                    position.Y - FullHeight / 2
                );
            }
            else Position = position;

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

        //the fact position is public here makes me sick
        public override void UpdatePosition(Vector2 position)
        {
            Position = position;
            Load(_parentBackground, _allignment);
        }
    }
}
