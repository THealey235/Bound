using Microsoft.Xna.Framework.Graphics;
using Bound.Managers;
using Microsoft.Xna.Framework;
using Bound.Models.Items;
using System.Collections.Generic;

namespace Bound.Controls.Game
{
    public class InventorySlot : Button
    {
        private string _ID;
        private Game1 _game;
        private BorderedBox _background;
        private string _containedItem;
        private float _layer;
        private Color _bgDefaultColour;
        private Color _hoveringColour;
        private int _index;
        private Item _item;
        private string _quantity;
        private Vector2? _quantityPosition;
        private float _quantityScale = 0.4f;
        private bool _showQuantity = false;
        public int Index
        {
            get
            {
                return _index;
            }
        }
        
        public readonly TextureManager.ItemType ItemType;

        public string ContainedItem
        {
            get
            {
                return _containedItem;
            }
            set
            {
                _containedItem = value;
                _texture = value == "Default" ? _game.Textures.Blank :_game.Items[value].Textures.GetIcon();
                ChangeQuantity(value);
            }
        }

        public string ID
        {
            get
            {
                return _ID;
            }
        }

        public new float Layer
        {
            get
            {
                return _layer;
            }
            set
            {
                _layer = value;
                base.Layer = _layer;
                _background.Layer = value - 0.0000001f;
            }
        }

        public InventorySlot(Game1 game, SpriteFont font, string id, string containedItem, TextureManager.ItemType type) : base(game.Textures.EmptyBox, font)
        {
            _ID = id;
            _game = game;
            ItemType = type;
            _bgDefaultColour = Game1.MenuColorPalette[1];
            _hoveringColour = Game1.BlendColors(_bgDefaultColour, Color.Gray, 0.5f);
            _background = new BorderedBox(
                _game.Textures.BaseBackground,
                game.GraphicsDevice,
                _bgDefaultColour,
                new Vector2(0, 0),
                Layer,
                (int)((_texture.Width * 0.75f + 2) * Game1.ResScale),
                (int)((_texture.Height * 0.75f + 2) * Game1.ResScale)
            );

            ContainedItem = containedItem;
            ChangeQuantity(containedItem);
        }

        private void ChangeQuantity(string containedItem)
        {
            if (_containedItem != "Default")
            {
                _item = _game.SavesManager.ActiveSave.Inventory.GetItem(containedItem);
                _quantity = $"x{_item.Quantity}";
                _showQuantity = _item.Quantity > 1;
                _quantityPosition = null;
            }
            else
                _showQuantity = false ;
        }

        public InventorySlot(Game1 game, SpriteFont font, string id, string containedItem, TextureManager.ItemType type, int index)
            : this(game, font, id, containedItem, type)
        {
            _index = index;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            var colour = _hoveringColour;

            if (!_isHovering)
            {
                colour = _bgDefaultColour;
            }

            _background.Colour = colour;
            //runing this every draw is costly for no reason but this is the cascading issue with setting values by public and not by constructor
            //and to fix this i would have to refactor a ton of code so this is a problem for later
            _background.Position = new Vector2(Position.X - 1 * Game1.ResScale, Position.Y - 1 * Game1.ResScale); 
            _background.Draw(gameTime, spriteBatch);

            if (_showQuantity)
            {
                if (_quantityPosition == null)
                {
                    var offset = Game1.ScreenHeight switch
                    {
                        720 => -2f,
                        900 => -2f,
                        1080 => -1.45f,
                        1440 => -1f,
                        _ => 0f,
                    };

                    var qDims = _font.MeasureString(_quantity) * _quantityScale;
                    _quantityPosition = new Vector2(
                        _background.Position.X + _background.Width - qDims.X - offset * Game1.ResScale,
                        _background.Position.Y + _background.Height - qDims.Y - offset * Game1.ResScale
                    );
                }
                spriteBatch.DrawString(_font, _quantity, (Vector2)_quantityPosition, Color.Black, 0f, Origin, _quantityScale, SpriteEffects.None, Layer + 0.0002f);
            }
        }
    }
}
