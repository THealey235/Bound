using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bound.Managers;
using Bound.Models;


namespace Bound.Sprites
{
    public class Block
    {
        #region Fields & Properties

        private TextureManager _textures;
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
                return new Rectangle((int)(ScaledPosition.X), (int)(ScaledPosition.Y), _shapeRectangle.Width, _shapeRectangle.Height);
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
        public Block(TextureManager textures, int index, Rectangle source, Vector2 position, float scale, float layer, GraphicsDevice graphics)
        {
            _name = "block";

            _sourceRectangle = source;

            Position = position;

            _scale = scale;

            Layer = layer;
            
            Index = index;

            _blockWidth = _textures.BlockWidth;

            if (_textures.SpecialShapeBlocks.ContainsKey(Index))
                _shapeRectangle = _textures.SpecialShapeBlocks[Index];
            else _shapeRectangle = new Rectangle(0, 0, _blockWidth, _blockWidth);

            if (!_textures.GhostBlocks.Contains(Index))
                _debugRectangle = new DebugRectangle(ScaledRectangle, graphics, layer + 0.1f, Scale);

        }

        public Block(TextureManager textures, GraphicsDevice graphics, int index, Vector2 position, Color colour, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layer)
        {
            _name = "block";
            _textures = textures;
            var rowLength = _textures.BlockAtlas.Width / _textures.BlockWidth;
            var column = index / rowLength;
            var row = index % rowLength;

            Position = position;

            _sourceRectangle = new Rectangle(_textures.BlockWidth * row, _textures.BlockWidth * column, _textures.BlockWidth, _textures.BlockWidth);

            Colour = colour;

            Rotation = rotation;

            _origin = origin;

            _scale = scale;

            _spriteEffects = spriteEffects;

            Layer = layer;

            _blockWidth = _textures.BlockWidth;

            Index = index;

            if (_textures.SpecialShapeBlocks.ContainsKey(Index))
                _shapeRectangle = _textures.SpecialShapeBlocks[Index];
            else _shapeRectangle = new Rectangle(0, 0, _blockWidth, _blockWidth);

            if (!_textures.GhostBlocks.Contains(Index))
                _debugRectangle = new DebugRectangle(ScaledRectangle, graphics, layer + 0.1f, Scale);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textures.BlockAtlas, ScaledPosition, _sourceRectangle, Colour, Rotation, _origin, Scale, _spriteEffects, Layer);
            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }


    }
}
