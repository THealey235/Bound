using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls
{
    public class PopupWindow : Component
    {
        private Texture2D _texture;
        private Color _colour;
        private float _layer;

        public int _width;
        private int _height;

        private Vector2 _position;

        private Vector2 _scale
        {
            get
            {
                return Game1.ResScale * new Vector2(_width / _texture.Width, _height / _texture.Height); 
            }
        }
            

        public PopupWindow(Texture2D texture, Color color, Vector2 position, float layer, int width, int height)
        {
            _texture = texture;
            _colour = color;
            _position = position;
            _layer = layer;
            _width = width;
            _height = height;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, _colour, 0f, Vector2.Zero, _scale, SpriteEffects.None, _layer);
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
