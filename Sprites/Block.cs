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
        private SpriteEffects _spriteEffects;
        private Vector2 _origin = Vector2.Zero;
        private DebugRectangle _debugRectangle;
        private float _scale;
        private string _name;
        private Rectangle _shapeRectangle;

        public float Layer;
        public float Rotation = 0f;
        public Color Colour = Color.White;
        
        public Vector2 Position { get; set; }

        public string Name
        {
            get {  return _name; }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)(Position.X) + (int)(_shapeRectangle.X * _scale), (int)(Position.Y) + (int)(_shapeRectangle.Y * _scale), (int)(_shapeRectangle.Width * _scale), (int)(_shapeRectangle.Height * _scale));
            }
        }

        public Rectangle ScaledRectangle
        {
            get
            {
                return new Rectangle((int)(ScaledPosition.X + _shapeRectangle.X * Scale), (int)(ScaledPosition.Y + _shapeRectangle.Y * Scale), (int)(_shapeRectangle.Width * Scale), (int)(_shapeRectangle.Height * Scale));
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
        public Block(TextureManager textures, GraphicsDevice graphics, string blockName, Vector2 position, float scale, float layer, bool isBlockwisePos = false)
        {
            SetValues(textures, graphics, blockName, position, scale, layer, isBlockwisePos);

        }

        public Block(TextureManager textures, GraphicsDevice graphics, string blockName, Vector2 position, Color colour, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layer, bool isBlockwisePos = false)
        {

            Colour = colour;

            Rotation = rotation;

            _origin = origin;

            _spriteEffects = spriteEffects;

            SetValues(textures, graphics, blockName, position, scale, layer, isBlockwisePos);
        }

        private void SetValues(TextureManager textures, GraphicsDevice graphics, string blockName, Vector2 position, float scale, float layer, bool isBlockwisePos)
        {
            _name = blockName;

            _texture = textures.GetAtlasItem(blockName);

            Position = position;
            if (isBlockwisePos)
                Position *= textures.BlockWidth * Game1.ResScale * Game1.BlockScale;

            _scale = scale;

            Layer = layer;

            if (textures.SpecialShapeBlocks.ContainsKey(_name))
                _shapeRectangle = textures.SpecialShapeBlocks[_name];
            else _shapeRectangle = new Rectangle(0, 0, _texture.Width, _texture.Width);

            if (!textures.GhostBlocks.Contains(_name))
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
