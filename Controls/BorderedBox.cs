using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls
{
    public class BorderedBox : Component
    {
        private Texture2D _texture;
        private Color _colour;
        private float _layer;
        private Texture2D _border;
        private GraphicsDevice _graphicsDevice;
        private int _width;
        private int _height;
        private Vector2 _position;
        private int _barWidth
        {
            get
            {
                return (int)(3 * Game1.ResScale);
            }
        }

        public bool IsBordered;
        


        private Vector2 _scale
        {
            get
            {
                return new Vector2(_width / _texture.Width, _height / _texture.Height); 
            }
        }
            

        public BorderedBox(Texture2D texture, GraphicsDevice graphicsDevice, Color color, Vector2 position, float layer, int width, int height)
        {
            _texture = texture;
            _colour = color;
            _position = position;
            _layer = layer;
            _width = width;
            _height = height;

            _graphicsDevice = graphicsDevice;

            IsBordered = true;
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Base background
            spriteBatch.Draw(_texture, _position, null, _colour, 0f, Vector2.Zero, _scale, SpriteEffects.None, _layer);

            if (IsBordered)
            {
                SetRectangleTexture(_graphicsDevice, _texture);
                spriteBatch.Draw(_border, _position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, _layer + 0.01f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        #region Other Methods

        private void SetRectangleTexture(GraphicsDevice graphics, Texture2D texture)
        {

            var colours = new List<Color>();
            


            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (x < _barWidth || //left side
                       y < _barWidth || //top side
                       x > _width - (_barWidth + 1) ||  //right side
                       y > _height - (_barWidth + 1)) //bottom side
                    {
                        colours.Add(new Color(255, 255, 255, 255));
                    }
                    else
                    {
                        colours.Add(new Color(0, 0, 0, 0));

                    }
                }
            }

            _border = new Texture2D(graphics, _width, _height);
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
