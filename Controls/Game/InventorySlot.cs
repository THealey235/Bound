using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bound.Managers;
using Microsoft.Xna.Framework;

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

        public int Index
        {
            get
            {
                return _index;
            }
        }
        
        public Textures.ItemType _itemType;
        public string ContainedItem
        {
            get
            {
                return _containedItem;
            }
            set
            {
                _containedItem = value;
                _texture = _game.Textures.GetItemTexture(value, _itemType, Textures.ItemTextureType.Icon);
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

        public InventorySlot(Game1 game, SpriteFont font, string id, string containedItem, Textures.ItemType type) : base(game.Textures.EmptyBox, font)
        {
            _ID = id;
            _game = game;
            _itemType = type;
            _bgDefaultColour = Game1.MenuColorPalette[1];
            _hoveringColour = Game1.BlendColors(_bgDefaultColour, Color.Gray, 0.5f);
            _background = new BorderedBox(
                _game.Textures.BaseBackground,
                game.GraphicsDevice,
                _bgDefaultColour,
                Position,
                Layer,
                (int)(_texture.Width * Game1.ResScale * 0.75f),
                (int)(_texture.Height * Game1.ResScale * 0.75f)
            );

            ContainedItem = containedItem;
        }

        public InventorySlot(Game1 game, SpriteFont font, string id, string containedItem, Textures.ItemType type, int index)
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
            _background.Position = Position;
            _background.Draw(gameTime, spriteBatch);
        }
    }
}
