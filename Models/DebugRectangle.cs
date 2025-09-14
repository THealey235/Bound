using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Bound.Models
{
    public class DebugRectangle : Component
    {
        private GraphicsDevice _graphics;
        private Texture2D _borderTexture;
        private Rectangle _rectangle;
        private float _layer;
        public int BarThickness;
        public Color BorderColour;
        public bool IsCollided;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Origin = new Vector2 (0, 0);
        public SpriteEffects SpriteEffects;
        public float Scale;

        public Vector2 ScaledPosition
        {
            get
            {
                return new Vector2((int)(Position.X * Scale), (int)(Position.Y * Scale));
            }
        }

        public DebugRectangle(Rectangle rectangle, GraphicsDevice graphics, float layer, float scale)
        {
            _rectangle = rectangle;
            _graphics = graphics;

            BarThickness = 1;
            BorderColour = Color.White;
            Position = new Vector2(rectangle.X, rectangle.Y);
            SetRectangleTexture();
            Scale = scale;
            _layer = layer;
        }

        public override void Update(GameTime gameTime)
        {
            IsCollided = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_borderTexture != null)
                spriteBatch.Draw(_borderTexture, Position, null, (BorderColour == Color.White) ? Game1.DebugColour : BorderColour, Rotation, Origin, Scale, SpriteEffects, _layer);
        }

        private void SetRectangleTexture()
        {
            var emptyColour = new Color(0, 0, 0, 0);
            var colours = new List<Color>();

            for (int y = 0; y < _rectangle.Height; y++)
            {
                for (int x = 0; x < _rectangle.Width; x++)
                {
                    if (x < BarThickness || //left side
                    y < BarThickness || //top side
                       x > _rectangle.Width - (BarThickness + 1) ||  //right side
                       y > _rectangle.Height - (BarThickness + 1)) //bottom side
                    {
                        colours.Add(Color.White);
                    }
                    else
                    {
                        colours.Add(emptyColour);

                    }
                }
            }

            _borderTexture = new Texture2D(_graphics, _rectangle.Width, _rectangle.Height);
            _borderTexture.SetData<Color>(colours.ToArray());
        }
    }
}
