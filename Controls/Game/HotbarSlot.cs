using Bound.Managers;
using Bound.Models;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing.Printing;

namespace Bound.Controls.Game
{
    public class HotbarSlot : Component
    {
        private Texture2D _background;
        private Texture2D _activeBackground;
        private Game1 _game;
        private Item _item;
        private Vector2 _hotbarPosition;
        private Vector2 _itemPosition;
        private Vector2 _quantityPosition;
        private string _quantity;
        private SpriteFont _font;
        private float _quantityScale = 0.55f;

        public float Layer;
        private float _hotbarScale;
        public float _itemScale;
        public bool IsSelected;

        public float HotbarScale
        {
            get
            {
                return _hotbarScale * Game1.ResScale;
            }
        }

        public float ItemScale
        {
            get
            {
                return _itemScale * Game1.ResScale;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _hotbarPosition;
            }
            set
            {
                _hotbarPosition = value;
                if (_item != null)
                { 
                    _itemPosition = new Vector2
                    (
                        _hotbarPosition.X + (_background.Width * HotbarScale - (_item.Textures.GetIcon().Width) * ItemScale) / 2,
                        _hotbarPosition.Y + (_background.Width * HotbarScale - (_item.Textures.GetIcon().Width) * ItemScale) / 2
                    );
                    SetQuantityPosition();
            }
                }
        }

        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                Position = _hotbarPosition;
                if (value != null)
                    SetQuantityPosition();
            }
        }


        public HotbarSlot(Texture2D neutralTexture, Texture2D activeTexture, Vector2 position, Game1 game, float layer, float textureScale, string item)
        {
            _hotbarScale = _itemScale = textureScale;
            _hotbarScale *= 1.2f;
            Layer = layer;
            _game = game;
            _font = _game.Textures.Font;
            _background = neutralTexture;
            _activeBackground = activeTexture;
            if (item != "Default")
                _item = _game.SavesManager.ActiveSave.Inventory.GetItem(TextureManager.ItemType.HoldableItem, item);
            Position = position;
            IsSelected = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw((IsSelected ? _activeBackground : _background), _hotbarPosition + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, HotbarScale, SpriteEffects.None, Layer);

            if (_item != null)
            {
                spriteBatch.Draw(_item.Textures.GetIcon(), _itemPosition + Game1.V2Transform, null, Color.White, 0f, Vector2.Zero, ItemScale, SpriteEffects.None, Layer + 0.0001f);
                if (_item.Quantity > 1)
                    spriteBatch.DrawString(_font, _quantity, _quantityPosition + Game1.V2Transform, Color.Black, 0f, Vector2.Zero, _quantityScale, SpriteEffects.None, Layer + 0.0002f);
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public void UpdateItem(TextureManager.ItemType type, string name)
        {
            _item = _game.SavesManager.ActiveSave.Inventory.GetItem(type, name);
            Position = _hotbarPosition;
            if (_item != null)
                SetQuantityPosition();
        }

        private void SetQuantityPosition()
        {
            _quantity = $"x{_item.Quantity}";
            var qDims = _font.MeasureString(_quantity) * _quantityScale * 1.01f;
            // so it doens't overlap with the hotbar background
            var offset = 1 * HotbarScale;
            _quantityPosition = new Vector2
            (
                _hotbarPosition.X + _background.Width * HotbarScale - qDims.X - offset,
                _hotbarPosition.Y + _background.Height * HotbarScale - qDims.Y - offset
            );

            if (_item.Quantity > 1)
            {
                //_itemPosition -= new Vector2(3, 3) * ItemScale;
            }
        }
    }
}
