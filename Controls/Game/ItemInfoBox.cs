using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Controls.Game
{
    public class ItemInfoBox : ChoiceBox
    {
        private List<Component> _components;
        private SpriteFont _font;
        private BorderedBox _background;
        private Vector2 _position;
        private Game1 _game;
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

        private BorderedBox _informationBackground;
        private List<Component> _itemDescriptionComponents = new List<Component>();
        private List<(string Text, Vector2 Position)> _itemDescription = new List<(string Text, Vector2 Position)>();
        private float _descriptionTextScale = 0.5f;
        private Vector2 _descriptionImagePos;

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

        public ItemInfoBox(Game1 game, Item item, Vector2 position, BorderedBox infoBackground, int width, int height, float layer, BorderedBox background, EventHandler onClick, int index)
        {
            _item = item;
            _game = game;
            Width = width;
            Height = height;
            OnApply = onClick;
            Layer = layer;
            _parent = background;
            _position = position;
            _informationBackground = infoBackground;
            _font = game.Textures.Font;
            _sourceRectangle = game.Textures.GetSourceRectangle(item.Type, Bound.Managers.Textures.ItemTextureType.Icon);
            _itemName = new List<string>() { _item.Name };
            TextureScale = 0.75f;

            _bgColours = new Color[] { Game1.MenuColorPalette[1], Game1.BlendColors(Game1.MenuColorPalette[1], Color.Gray, 125) };

            _index = index;

            LoadContent(_game, background);
        }

        public ItemInfoBox(Game1 game, Item item, Vector2 position, BorderedBox infoBackground, int width, int height, float layer, BorderedBox background, EventHandler onClick, int index, ScrollingMenu container)
            : this (game, item, position, infoBackground, width, height, layer, background, onClick, index)
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

                        if (_background.Colour == _bgColours[1])
                            Container.ChangeSelected(_index, this);
                        else
                            Container.ChangeSelected(-1, this);
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

            var bg = _informationBackground;
            var border = 2 * Game1.ResScale;
            var layer = bg.Layer + 0.00001f;
            var frame = new BorderedBox(
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                bg.Colour,
                new Vector2(bg.Position.X + border, bg.Position.Y + border),
                layer,
                (int)((_sourceRectangle.Width + 0.25f) * Game1.ResScale),
                (int)((_sourceRectangle.Height + 0.25f) * Game1.ResScale)
            );

            _itemDescriptionComponents = new List<Component>
            {
                bg,
                frame,
            };

            var description = _item.Description.Split(' ');
            var unit = _font.MeasureString("X") * _descriptionTextScale;
            var maxUnits = (int)((bg.Width - 2 * border) / unit.X);
            var topTextPos = new Vector2(frame.Position.X, frame.Position.Y + frame.Height + border * 3);

            var chunks = new List<string>() { "" };
            foreach (var word in description)
            {
                if (chunks[^1].Length + word.Length + 1 > maxUnits) //+1 to account for the space between words
                    chunks.Add("");

                if (chunks[^1] == "")
                    chunks[^1] += word;
                else
                    chunks[^1] += (" " + word);
            }

            for (int i = 0; i < chunks.Count; i++)
                _itemDescription.Add((chunks[i], new Vector2(topTextPos.X, topTextPos.Y + (unit.Y + 2 * Game1.ResScale) * i)));

            if (_item.Attributes != null && _item.Attributes.Count > 0)
            {
                var attrs = _item.Attributes.Values.ToList();
                for (int i = 0; i < _item.Attributes.Count; i++)
                    _itemDescription.Add((attrs[i].ToString(), new Vector2(topTextPos.X, topTextPos.Y + (unit.Y + 2 * Game1.ResScale) * ( chunks.Count + 1 + i))));
            }

            _descriptionImagePos = new Vector2(frame.Position.X + 0.125f * Game1.ResScale, frame.Position.Y + 0.125f * Game1.ResScale);
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

        public void DrawInfo(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var frame = _itemDescriptionComponents[1] as BorderedBox;

            foreach (var c in _itemDescriptionComponents)
                c.Draw(gameTime, spriteBatch);

            foreach (var line in _itemDescription)
                spriteBatch.DrawString(_font, line.Text, line.Position, Color.Black, 0f, Vector2.Zero, _descriptionTextScale, SpriteEffects.None, frame.Layer);

            spriteBatch.Draw(_item.Texture, _descriptionImagePos, _sourceRectangle, Color.White, 0f, Vector2.Zero, Game1.ResScale, SpriteEffects.None, frame.Layer + 0.001f);

        }
    }
}
