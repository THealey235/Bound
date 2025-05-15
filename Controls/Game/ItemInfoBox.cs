using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls.Game
{
    public class ItemInfoBox : ChoiceBox
    {
        private List<Component> _components;
        private List<Component> _infoBoxComponents;
        private Texture2D _texture;
        private SpriteFont _font;
        private BorderedBox _background;
        private Vector2 _position;
        private Game1 _game;
        private string _text;
        private Item _item;
        private Vector2 _itemTexturePos;
        private Vector2 _namePos;
        private Vector2 _ammountPos;
        private float _scale = Game1.ResScale;

        public float Layer;
        public int Width;
        public int Height;
        public Color BorderColor = Color.Black;
        public Color PenColor = Game1.MenuColorPalette[2];
        private BorderedBox _parent;
        public Item HeldItem
        {
            get
            {
                return _item;
            }
        }

        public ItemInfoBox(Game1 game, Item item, Vector2 position, int width, int height, float layer, BorderedBox background, EventHandler onClick)
        {
            _item = item;
            _game = game;
            Width = width;
            Height = height;
            OnApply = onClick;
            Layer = layer;
            _parent = background;
            _position = position;
            _font = game.Textures.Font;

            LoadContent(_game, background);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var c in _components)
                c.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(_item.Texture, _itemTexturePos, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, Layer + 0.0002f);
            spriteBatch.DrawString(_font, _item.Name, _namePos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.0001f);
            if (_item.Ammount > 1)
                spriteBatch.DrawString(_font, $"x{_item.Ammount}", _ammountPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.0001f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);
        }
        
        public override void LoadContent(Game1 game, BorderedBox background, float allignment = 0f)
        {
            var textureScale = 0.75f;
            var itemTextureDimension = 32;
            _scale = textureScale * Game1.ResScale;

            _background = new BorderedBox
            (
                game.Textures.BaseBackground,
                game.GraphicsDevice,
                Game1.MenuColorPalette[1],
                _position,
                Layer,
                Width,
                Height
            );

            _itemTexturePos = new Vector2(_position.X + 5 * Game1.ResScale, _position.Y + (Height - itemTextureDimension * _scale) / 2);
            _namePos = new Vector2(_itemTexturePos.X + 32 * _scale + 5 * Game1.ResScale, _position.Y + (int)((Height - _font.MeasureString("T").Y) / 2));
            var ammountDim = _font.MeasureString($"x{_item.Ammount}");
            _ammountPos = new Vector2(_position.X + Width - ammountDim.X - 1 * Game1.ResScale, _position.Y + Height  - ammountDim.Y - 1 * Game1.ResScale);


            _components = new List<Component>()
            {
                _background
            };
        }

        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
            LoadContent(_game, _parent);
        }
    }
}
