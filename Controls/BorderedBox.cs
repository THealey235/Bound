using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bound.Controls
{
    public class BorderedBox : Component
    {
        private Texture2D _texture;
        private Texture2D _border;
        public List<Texture2D> _borderTextures;
        private int _width;
        private GraphicsDevice _graphics;
        private Vector2 _position;

        private int _barWidth
        {
            get
            {
                return (int)(1 * Game1.ResScale);
            }
        }
        private Vector2 _scale
        {
            get
            {
                return new Vector2((float)Width / (float)_texture.Width, (float)Height / (float)_texture.Height); 
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                _border.Dispose();
                SetRectangleTexture();
            }
        }

        public float Layer;
        public Color Colour;
        public int Height;
        public bool IsBordered;
        public Color BorderColor;
        public bool IgnoreCameraTransform = false;


        public Vector2 Position
        {
            get
            {
                if (IgnoreCameraTransform)
                    return _position + Game1.V2Transform;
                return _position;
            }
            set
            {
                _position = value;
            }
        }

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
            Colour = color;
            _position = position;
            Layer = layer;
            _width = width;
            Height = height;
            _graphics = graphicsDevice;

            IsBordered = true;

            BorderColor = new Color(0, 0, 0, 255);
            SetRectangleTexture();
        }


        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height, Color borderColor)
            : this(texture, graphicsDevice, color, position, layer, width, height)
        {
            BorderColor = borderColor;
            SetRectangleTexture();
        }

        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height, List<Texture2D> _borders)
            : this(texture, graphicsDevice, color, position, layer, width, height)
        {
            _borderTextures = _borders;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Base background
            spriteBatch.Draw(_texture, Position, null, Colour, 0f, Vector2.Zero, _scale, SpriteEffects.None, Layer);

            if (IsBordered)
            {
                spriteBatch.Draw(_border, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.0001f);
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        #region Other Methods

        public void SetRectangleTexture()
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

            _border = new Texture2D(_graphics, Width, Height);
            _border.SetData<Color>(colours.ToArray());
        }
        #endregion
    }
}
