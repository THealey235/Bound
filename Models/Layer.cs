using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models
{
    public class Layer : Component
    {
        private Dictionary<Texture2D, float> _scales;
        private List<Element> _elements = new List<Element>();
        private float _bottom;
        private float _constantSpeed;
        private float _sparsity;
        private float _layer;
        private float _previousViewportX;

        public float BottomVariance = 0;
        public Color Colour = Color.White;

        private class Element
        {
            public Vector2 Position;
            public Texture2D Texture;
            public float ScrollSpeedMult;

            public Element(Texture2D texture, Vector2 position, float scrollSpeedMult)
            {
                Position = position;
                Texture = texture;
                ScrollSpeedMult = scrollSpeedMult;
            }
        }


        public Layer(List<(Texture2D Texture, float Scale)> textures, float layer, float bottom, float spacing, (float Min, float Max) scrollingSpeedMultiplier, float constantSpeed = 0f)
        {
            _scales = textures.ToDictionary(x => x.Texture, x => x.Scale);
            _layer = layer;
            _bottom = bottom;
            _sparsity = spacing;
            _constantSpeed = constantSpeed;

            var viewport = Game1.UnscaledViewport;
            var x = 0f;
            var SSM = scrollingSpeedMultiplier;
            while (x < viewport.Width)
            {
                var texture = textures[Game1.Random.Next(_scales.Count)].Item1;
                var xPos = x + (x == 0 ? _sparsity / 2 : _sparsity) * (float)Game1.Random.NextDouble();

                _elements.Add(new Element(
                    texture,
                    new Vector2(xPos + viewport.X, _bottom - (texture.Height * _scales[texture]) + ((Game1.Random.Next(3) - 1) * BottomVariance * (float)Game1.Random.NextDouble())),
                    SSM.Min + ((SSM.Max - SSM.Min) * (float)Game1.Random.NextDouble())
                ));
                x = xPos + texture.Width * _scales[texture] + 1;
            }
            _previousViewportX = Game1.UnscaledViewport.X;
        }

        public Layer(List<(Texture2D Texture, float Scale)> scales, List<(Texture2D Texture, Vector2 Position)> textures, float layer, float bottom, float spacing, float scrollingSpeedMultiplier, float constantSpeed = 0f)
        {
            _scales = scales.ToDictionary(x => x.Texture, x => x.Scale);
            _layer = layer;
            var xTranslation = Game1.UnscaledViewport.X;
            _elements = textures.Select(x => new Element(x.Texture, new Vector2(x.Position.X + xTranslation, x.Position.Y), scrollingSpeedMultiplier)).ToList();
        }

        public Layer(List<(Texture2D Texture, float Scale)> textures, float layer, float bottom, float spacing, float scrollingSpeedMultiplier, float constantSpeed = 0f)
            :this(textures, layer, bottom, spacing, (scrollingSpeedMultiplier, scrollingSpeedMultiplier), constantSpeed)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var element in _elements)
                spriteBatch.Draw(element.Texture, element.Position * Game1.ResScale, null, Color.White, 0f, Vector2.Zero, _scales[element.Texture] * Game1.ResScale, SpriteEffects.None, _layer);
        }

        public override void Update(GameTime gameTime)
        {
            var viewport = Game1.UnscaledViewport;
            var deltaX = viewport.X - _previousViewportX;

            foreach (var element in _elements)
            {
                element.Position.X += _constantSpeed - (deltaX * element.ScrollSpeedMult); //- so that it goes left when we go right

                //"Reflect" position it across the viewport if it is outside of it
                if (element.Position.X + (element.Texture.Width * _scales[element.Texture]) < viewport.Left)
                    element.Position.X = viewport.Right + (viewport.Left - (element.Position.X + element.Texture.Width * _scales[element.Texture]));
                else if (element.Position.X > viewport.Right)
                    element.Position.X -= viewport.Width + (2 * (element.Position.X - viewport.Right)) + (element.Texture.Width * _scales[element.Texture]);
            }

            _previousViewportX =  viewport.X;
        }
    }
}
