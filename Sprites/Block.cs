using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bound.Managers;
using Bound.Models;


namespace Bound.Sprites
{
    public class Block : Component
    {
        #region Fields & Properties

        private Texture2D _texture;
        private Rectangle _sourceRectangle;
        private SpriteEffects _spriteEffects;
        private Vector2 _origin = Vector2.Zero;
        private int _blockWidth;
        private DebugRectangle _debugRectangle;
        private float _scale;
        private string _name;
        private Rectangle _shapeRectangle;

        public float Layer;
        public float Rotation = 0f;
        public Color Colour = Color.White;
        public int Index;
        public Vector2 Position { get; set; }

        public string Name
        {
            get {  return _name; }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)(Position.X) + _shapeRectangle.X, (int)(Position.Y) + _shapeRectangle.Y, (int)(_shapeRectangle.Width * _scale), (int)(_shapeRectangle.Height * _scale));
            }
        }

        public Rectangle ScaledRectangle
        {
            get
            {
                return new Rectangle((int)(ScaledPosition.X), (int)(ScaledPosition.Y), (int)(_shapeRectangle.Width * Scale), (int)(_shapeRectangle.Height * Scale));
            }
        }

        public Vector2 ScaledPosition
        {
            get
            {
                return new Vector2((Position.X) * Game1.ResScale, (Position.Y) * Game1.ResScale);
            }
        }

        public float Scale
        {
            get
            {
                return _scale * Game1.ResScale;
            }
        }

        #endregion
        public Block(TextureManager textures, int index, Vector2 position, float scale, float layer, GraphicsDevice graphics)
        {
            _name = "block";

            _texture = textures.GetBlock("common", index);

            _sourceRectangle = new Rectangle(0, 0, textures.BlockWidth, textures.BlockWidth);

            Position = position;

            _scale = scale;

            Layer = layer;
            
            Index = index;

            _blockWidth = textures.BlockWidth;

            if (textures.SpecialShapeBlocks.ContainsKey(Index))
                _shapeRectangle = textures.SpecialShapeBlocks[Index];
            else _shapeRectangle = _sourceRectangle;

            if (!textures.GhostBlocks.Contains(Index))
                _debugRectangle = new DebugRectangle(ScaledRectangle, graphics, layer + 0.1f, Scale);

        }

        public Block(TextureManager textures, GraphicsDevice graphics, int index, Vector2 position, Color colour, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layer)
        {
            _name = "block";

            Position = position;

            _texture = textures.GetBlock("common", index);

            _sourceRectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);

            Colour = colour;

            Rotation = rotation;

            _origin = origin;

            _scale = scale;

            _spriteEffects = spriteEffects;

            Layer = layer;

            _blockWidth = textures.BlockWidth;

            Index = index;

            if (textures.SpecialShapeBlocks.ContainsKey(Index))
                _shapeRectangle = textures.SpecialShapeBlocks[Index];
            else _shapeRectangle = new Rectangle(0, 0, _sourceRectangle.Width, _sourceRectangle.Height);

            if (!textures.GhostBlocks.Contains(Index))
                _debugRectangle = new DebugRectangle(ScaledRectangle, graphics, layer + 0.1f, 1f);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, ScaledPosition, null, Colour, Rotation, _origin, Scale, _spriteEffects, Layer);
            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }


    }
}
