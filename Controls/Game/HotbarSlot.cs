using Bound.Managers;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Printing;

namespace Bound.Controls.Game
{
    public class HotbarSlot : Component
    {
        private Texture2D _background;
        private Texture2D _activeBackground;
        private Game1 _game;
        private Item _item;
        private Vector2 _position;
        private Vector2 _itemPosition;
        private Vector2 _quantityPosition;
        private string _quantity;
        private SpriteFont _font;
        private float _quantityScale = 0.5f;

        public float Layer;
        public float TextureScale;
        public bool IsSelected;

        public float Scale
        {
            get
            {
                return TextureScale * Game1.ResScale;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                _itemPosition = new Vector2
                (
                    _position.X + (_background.Width - _item.Texture.Width) * Scale / 2,
                    _position.Y + (_background.Height - _item.Texture.Height) * Scale / 2
                );
                SetQuantityPosition();
            }
        }

        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                SetQuantityPosition();
            }
        }


        public HotbarSlot(Texture2D neutralTexture, Texture2D activeTexture, Vector2 position, Game1 game, float layer, float textureScale, Item item)
        {
            TextureScale = textureScale;
            Layer = layer;
            _game = game;
            _font = _game.Textures.Font;
            _background = neutralTexture;
            _activeBackground = activeTexture;
            _item = item;
            Position = position;
            IsSelected = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw((IsSelected ? _activeBackground : _background), _position + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, Layer);

            spriteBatch.Draw(_item.Texture, _itemPosition + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, Layer + 0.0001f);
            if (_item.Quantity > 1)
                spriteBatch.DrawString(_font, _quantity, _quantityPosition + Game1.V2Transform, Color.Black, 0f, Vector2.Zero, _quantityScale, SpriteEffects.None, Layer + 0.0002f);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public void UpdateItem(Textures.ItemType type, string name)
        {
            _item = _game.SavesManager.ActiveSave.Inventory.GetItem(type, name);
            SetQuantityPosition();
        }

        private void SetQuantityPosition()
        {
            _quantity = $"x{_item.Quantity}";
            var qDims = _font.MeasureString(_quantity) * _quantityScale;
            _quantityPosition = new Vector2
            (
                _itemPosition.X + _item.Texture.Width * Scale - qDims.X / 2,
                _itemPosition.Y + _item.Texture.Height * Scale - qDims.Y / 2
            );
        }
    }
}
