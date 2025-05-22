using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace Bound.Controls.Game
{
    public class ItemInfoBox : ChoiceBox
    {
        private List<Component> _components;
        private List<Component> _infoBoxComponents;
        private SpriteFont _font;
        private BorderedBox _background;
        private Vector2 _position;
        private Game1 _game;
        private string _text;
        private Item _item;
        private Vector2 _itemTexturePos;
        private List<Vector2> _namePositions;
        private float _multiLineNameYSpacing;
        private Vector2 _ammountPos;
        private float _scale = Game1.ResScale;
        private float _fontScale = 0.75f;
        private bool _lockFontScale = false;
        private Rectangle _sourceRectangle;
        private List<string> _itemName;
        private Vector2 _ammountDim;
        private float _textureScale;
        private BorderedBox _parent;
        private Color[] _bgColours;
        private int _index;

        public float Layer;
        public int Width;
        public int Height;
        public Color BorderColor = Color.Black;
        public Color PenColor = Game1.MenuColorPalette[2];
        public ScrollingMenu Container;

        public float FontScale
        {
            get { return _fontScale; }
            set
            {
                _fontScale = value;
                _lockFontScale = true;
                LoadContent(_game, _parent);
            }
        }
        public float TextureScale
        {
            get { return _textureScale; }
            set
            {
                _textureScale = value;
                _scale = Game1.ResScale * value;
            }
        }
        public Item HeldItem
        {
            get
            {
                return _item;
            }
        }

        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)(_position.X - Game1.V2Transform.X), (int)(_position.Y - Game1.V2Transform.Y), Width, Height);
            }
        }

        public ItemInfoBox(Game1 game, Item item, Vector2 position, int width, int height, float layer, BorderedBox background, EventHandler onClick, int index)
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
            _sourceRectangle = game.Textures.GetSourceRectangle(item.Type, Bound.Managers.Textures.ItemTextureType.Icon);
            _itemName = new List<string>() { _item.Name };
            TextureScale = 0.75f;

            _bgColours = new Color[] { Game1.MenuColorPalette[1], Game1.BlendColors(Game1.MenuColorPalette[1], Color.Gray, 125) };

            _index = index;

            LoadContent(_game, background);
        }

        public ItemInfoBox(Game1 game, Item item, Vector2 position, int width, int height, float layer, BorderedBox background, EventHandler onClick, int index, ScrollingMenu container)
            : this (game, item, position, width, height, layer, background, onClick, index)
        { 
            Container = container;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var c in _components)
                c.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(_item.Texture, _itemTexturePos, _sourceRectangle, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, Layer + 0.0002f);
            for (int i = 0; i < _itemName.Count; i++)
                spriteBatch.DrawString(_font, _itemName[i], _namePositions[i], Color.Black, 0f, Vector2.Zero, _fontScale, SpriteEffects.None, Layer + 0.0001f);
            if (_item.Quantity > 1)
                spriteBatch.DrawString(_font, $"x{_item.Quantity}", _ammountPos, Color.Black, 0f, Vector2.Zero, _fontScale, SpriteEffects.None, Layer + 0.0001f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);

            var keys = _game.PlayerKeys;
            if (keys.MouseRectangle.Intersects(Rectangle) && keys.LeftClickReleased)
            {
                if (Container != null)
                    if (!Container.ItemBlacklist.Contains(_index))
                    {
                        _background.Colour = (_background.Colour == _bgColours[0]) ? _bgColours[1] : _bgColours[0];

                        Container.ChangeSelected(_index, this);
                    }
                    else
                        _background.Colour = (_background.Colour == _bgColours[0]) ? _bgColours[1] : _bgColours[0];
            }
        }
        
        public override void LoadContent(Game1 game, BorderedBox background, float allignment = 0f)
        {
            var itemTextureDimension = 32;

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

            _ammountDim = _font.MeasureString($"x{_item.Quantity}");
            var nameDim = _font.MeasureString($"{_item.Name}");
            _itemTexturePos = new Vector2(_position.X + 5 * Game1.ResScale, _position.Y + (Height - itemTextureDimension * _scale) / 2);
            var maxTextWidth = Width - (5 * Game1.ResScale + itemTextureDimension * _scale) - (5 * Game1.ResScale + 2 * Game1.ResScale);

            if (!_lockFontScale && nameDim.X > maxTextWidth)
            {
                _fontScale *= 0.9f;
                nameDim *= _fontScale;
                var firstLineText = new string(_item.Name.Take((int)((maxTextWidth / nameDim.X) * _item.Name.Length)).ToArray());
                _itemName = new List<string> {firstLineText, new string(_item.Name.Skip(firstLineText.Length).Take(_item.Name.Length - firstLineText.Length).ToArray())};
            }

            _namePositions = new List<Vector2>() { new Vector2(_itemTexturePos.X + 32 * _scale + 5 * Game1.ResScale, _position.Y + (int)((Height - _font.MeasureString("T").Y) / 2)) };
            _ammountPos = new Vector2(_position.X + Width - _ammountDim.X - 1 * Game1.ResScale, _position.Y + Height);
            _multiLineNameYSpacing = nameDim.Y;
            if (_itemName.Count > 1)
                _namePositions.Add(new Vector2(_namePositions[0].X + (_font.MeasureString(_itemName[0]).X * _fontScale - _font.MeasureString(_itemName[1]).X * _fontScale) / 2, _namePositions[0].Y + _multiLineNameYSpacing));

            _components = new List<Component>()
            {
                _background,
            };

            if (Game1.InDebug)
            {
                //Draws a rectangle around the text in the info box to double check for centering and other stuff
                _components.Add(new BorderedBox
                (
                    game.Textures.BaseBackground,
                    game.GraphicsDevice,
                    new Color(Color.Red, 20),
                    _namePositions[0],
                    Layer + 0.00001f,
                    (int)(_font.MeasureString(_itemName[0]).X * _fontScale),
                    (int)(_namePositions[1].Y + nameDim.Y - _namePositions[0].Y)
                ));
            }
        }

        public override void UpdatePosition(Vector2 position)
        {
            _background.Position = _position = position;
            _itemTexturePos = new Vector2(_position.X + 5 * Game1.ResScale, _position.Y + (Height - 32 * _scale) / 2);
            _namePositions = new List<Vector2>() { new Vector2(_itemTexturePos.X + 32 * _scale + 5 * Game1.ResScale, _position.Y + (int)((Height - _font.MeasureString("T").Y) / 2)) };
            if (_itemName.Count > 1)
                _namePositions.Add(new Vector2(_namePositions[0].X + (_font.MeasureString(_itemName[0]).X * _fontScale - _font.MeasureString(_itemName[1]).X * _fontScale) / 2, _namePositions[0].Y + _multiLineNameYSpacing));
            var nameDim = _font.MeasureString($"{_item.Name}");
            _ammountPos = new Vector2(_position.X + Width - _ammountDim.X - 1 * Game1.ResScale, _position.Y + Height - _ammountDim.Y);
        }

        public void ResetColour()
        {
            _background.Colour = _bgColours[0];
        }
    }
}
