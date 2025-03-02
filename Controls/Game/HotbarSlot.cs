using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound.Controls.Game
{
    public class HotbarSlot : Component
    {
        private Texture2D _background;
        private Texture2D _item;
        private Game1 _game;

        public Vector2 Position;
        public float Layer;
        public float Scale;
        public bool IsSelected;

        public HotbarSlot(Texture2D texture, Vector2 position, Game1 game, float layer, float scale)
        {
            _background = texture;
            Position = position;
            _game = game;
            Layer = layer;
            Scale = scale;
            IsSelected = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, Position + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, Game1.ResScale * Scale, SpriteEffects.None, Layer);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public void UpdateItem(string name)
        {
            _item = _game.Textures.Items[name];
        }
    }
}
