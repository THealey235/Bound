using Bound.Models;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Managers
{
    public class Background : Component
    {
        private List<Layer> _layers;
        private List<(Texture2D Texture, Vector2 Position, float Scale, float Layer)> _objects;

        public Color ObjectColour = Color.White;

        public Background(List<Layer> layers, List<(Texture2D Texture, Vector2 Position, float Scale, float Layer)>? objects = null)
        {
            _layers = layers;
            _objects = objects ?? new List<(Texture2D Texture, Vector2 Position, float Scale, float Layer)>();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var layer in _layers)
                layer.Draw(gameTime, spriteBatch);
            if (_objects != null)
            foreach (var obj in _objects)
                spriteBatch.Draw(obj.Texture, obj.Position * Game1.ResScale, null, ObjectColour, 0f, Vector2.Zero, obj.Scale * Game1.ResScale, SpriteEffects.None, obj.Layer);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var layer in _layers)
                layer.Update(gameTime);
        }

        public void Reset()
        {
            foreach (var layer in _layers)
                layer.Reset();
        }
    }
}
