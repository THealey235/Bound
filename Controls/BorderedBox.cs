using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bound.Controls
{
    public class BorderedBox : Component
    {
        private Texture2D _texture;
        private Color _colour;
        private float _layer;
        private Texture2D _border;
        private GraphicsDevice _graphicsDevice;
        private List<Texture2D> _borderTextures;

        private int _barWidth
        {
            get
            {
                return (int)(3 * Game1.ResScale);
            }
        }
        private Vector2 _scale
        {
            get
            {
                return new Vector2((float)Width / (float)_texture.Width, (float)Height / (float)_texture.Height); 
            }
        }

        public int Width;
        public int Height;
        public bool IsBordered;
        public Color BorderColor;
        public Vector2 Position;

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
        }

        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height)
        {
            _texture = texture;
            _colour = color;
            Position = position;
            _layer = layer;
            Width = width;
            Height = height;

            _graphicsDevice = graphicsDevice;

            IsBordered = true;

            BorderColor = new Color(0, 0, 0, 255);
            SetRectangleTexture(_graphicsDevice, _texture);

        }

        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height, Color borderColor)
            : this(texture, graphicsDevice, color, position, layer, width, height)
        {
            BorderColor = borderColor;
            SetRectangleTexture(_graphicsDevice, _texture);
        }

        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height, List<Texture2D> _borders)
            : this(texture, graphicsDevice, color, position, layer, width, height)
        {
            _borderTextures = _borders;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Base background
            spriteBatch.Draw(_texture, Position, null, _colour, 0f, Vector2.Zero, _scale, SpriteEffects.None, _layer);

            if (IsBordered)
            {
                spriteBatch.Draw(_border, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, _layer + 0.01f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        #region Other Methods

        public void SetRectangleTexture(GraphicsDevice graphics, Texture2D texture)
        {

            var colours = new List<Color>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x < _barWidth || //left side
                       y < _barWidth || //top side
                       x > Width - (_barWidth + 1) ||  //right side
                       y > Height - (_barWidth + 1)) //bottom side
                    {
                        colours.Add(BorderColor);
                    }
                    else
                    {
                        colours.Add(new Color(0, 0, 0, 0));

                    }
                }
            }

            _border = new Texture2D(graphics, Width, Height);
            _border.SetData<Color>(colours.ToArray());
        }

        //public void SavingForARainyDay(GraphicsDevice graphics, Texture2D texture)
        //{
        //    var height = (int)(_texture.Height * _scale.Y);
        //    var width = (int)(_texture.Width * _scale.X);

        //    var coloursSide = Enumerable.Repeat(new Color(225, 225, 225), _barWidth * height).ToArray();
        //    var coloursTop = Enumerable.Repeat(new Color(225, 225, 225), _barWidth * (width - _barWidth)).ToArray();

        //    _borders = new List<Texture2D>()
        //    {
        //        new Texture2D(graphics, _barWidth, height),
        //        new Texture2D(graphics, (width - _barWidth), _barWidth),
        //    };

        //    _borders[0].SetData<Color>(coloursSide);
        //    _borders[1].SetData<Color>(coloursTop);
        //}

        #endregion
    }
}
