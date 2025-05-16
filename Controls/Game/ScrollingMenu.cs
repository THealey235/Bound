using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Bound.States.Popups;

namespace Bound.Controls.Game
{
    public class ScrollingMenu : ChoiceBox
    {
        #region Properties/Fields
        private List<Component> _components;
        private Vector2 _position;

        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private BorderedBox _bar;
        private BorderedBox _cursor;
        private int _cursorHeight;
        private bool useInformationWindow = false;
        private bool _isPressed = false;
        private int _height;
        private int _width;
        private List<ChoiceBox> _items;
        private int _listStart;
        private int _itemHeight;
        private int _maxItemsInView;
        private Game1 _game;
        private float _scrollAmmount = 0f;//how far the user has scrolled down the list. Between 0 and 1
        private List<Vector2> _itemPositions;
        private float _extraItemsHeight;
        private int _parentTopEdge;
        private List<int> _itemBlacklist = new List<int>(); //indexes to not draw when drawing items

        // the "cover" is the Bordered Boxes used to cover the top and bottom of the items when they go out of range (declared in LoadContent())
        // this adds extra height to them so that you can't see the item boxes disappear when they go out of range
        private float _coverPadding; 

        public Color PenColour;
        public float TextureScale = 1f;
        public int MaxShown;
        public float Layer;
        public int BarWidth;

        

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
        }

        //"curosor" refers to the black box which sets the current value. NOT the mouse curosr
        private Rectangle _cursorRectangle
        {
            get
            {
                return new Rectangle((int)_cursor.Position.X, (int)_cursor.Position.Y, BarWidth, _cursorHeight);
            }
        }
        public Vector2 CursorPositon
        {
            get
            {
                var pos = new Vector2(_bar.Position.X, _bar.Position.Y + (_bar.Height * _scrollAmmount) - _cursor.Height / 2);
                MathHelper.Clamp(pos.Y, _bar.Position.Y, _bar.Height);
                return pos;
            }
        }

        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
            }
        }

        #endregion

        #region Constructor/Inherited Methods
        public ScrollingMenu(Game1 game, string name, Vector2 position, int width, int height, float layer, List<ChoiceBox> items, int componentHeight, int topEdgeY)
        {
            _components = new List<Component>();

            Name = name;
            Layer = layer;
            TextureScale = 1f;
            PenColour = Color.Black;
            _game = game;
            _height = height;
            _width = width;
            _items = items;
            _itemHeight = componentHeight;
            _position = position;
            _maxItemsInView = ((height - 10) / componentHeight) - 1;
            _parentTopEdge = topEdgeY;
            _coverPadding = 1 * Game1.ResScale;
            BarWidth = (int)(5 * Game1.ResScale);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            for (int i = 0; i < _items.Count; i++)
            {
                if (_itemBlacklist.Contains(i))
                    continue;
                _items[i].Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in _components)
                c.Update(gameTime);
            
            foreach (var item in _items)
                item.Update(gameTime);

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X + (int)Game1.V2Transform.X, _currentMouse.Y + (int)Game1.V2Transform.Y, 1, 1);


            //if it has just been clicked on or left click is still pressed
            if (mouseRectangle.Intersects(_cursorRectangle) &&
                _currentMouse.LeftButton == ButtonState.Pressed &&
                _previousMouse.LeftButton == ButtonState.Released ||
                _isPressed)
            {
                _isPressed = true;
            }
            if (_currentMouse.LeftButton == ButtonState.Released)
                _isPressed = false;

            if (_isPressed)
                FollowCursor();
            
        }


        public override void LoadContent(Game1 game, BorderedBox background, float allignment)
        {
            Load(_game);
        }

        private void Load(Game1 _game)
        {
            var defaultPos = new Vector2(Position.X + _width - BarWidth, Position.Y);
            _cursorHeight = (int)(12 * Game1.ResScale);
            _bar = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Color.White,
                defaultPos,
                Layer,
                BarWidth,
                _height
            );

            _cursor = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Color.Black,
                new Vector2(Position.X + _width - BarWidth, Position.Y),
                Layer + 0.0001f,
                BarWidth,
                _cursorHeight
            );


            _components = new List<Component>
            {
                _bar,
                _cursor,
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Game1.MenuColorPalette[0],
                    new Vector2(Position.X, _parentTopEdge + (int)(1.5 * Game1.ResScale)),
                    Layer + 0.003f,
                    _width,
                    (int)(Position.Y - _parentTopEdge + _coverPadding)
                ) { IsBordered = false},
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Game1.MenuColorPalette[0],
                    new Vector2(Position.X, Position.Y + _height - _coverPadding),
                    Layer + 0.003f,
                    _width,
                    (int)(Position.Y - _parentTopEdge + _coverPadding)
                ) { IsBordered = false},
            };

            var pos = new Vector2(Position.X, Position.Y + 5 * Game1.ResScale);
            var ySpacing = _itemHeight + 5 * Game1.ResScale;
            _itemPositions = new List<Vector2>();
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].UpdatePosition(pos);
                _itemPositions.Add(pos);
                pos.Y += ySpacing;
            }

            var extraItems = _items.Count - _maxItemsInView;
            _extraItemsHeight = extraItems > 0 ? extraItems * _itemHeight + (extraItems - 1) * ySpacing : 0;

            SetItemProperties();

        }

        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
            Load(_game);
        }

        #endregion

        #region Other Methods

        private void FollowCursor()
        {
            var barY = _bar.Position.Y - Game1.V2Transform.Y;
            var y = MathHelper.Clamp(_currentMouse.Y, barY + _cursor.Height / 2, barY + _bar.Height - _cursor.Height / 2);
            y = (float)(y - barY) / _bar.Height;
            y = y > 99f ? 100f : y;
            _scrollAmmount = y;
            _cursor.Position = CursorPositon;
            SetItemProperties();
        }

        private void SetItemProperties()
        {
            var translation = new Vector2(0, _scrollAmmount * _extraItemsHeight);
            _itemBlacklist.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].UpdatePosition(_itemPositions[i] - translation);
                if (_items[i].Rectangle.Bottom < Position.Y - Game1.V2Transform.Y + _coverPadding || _items[i].Rectangle.Top > Position.Y + _height - Game1.V2Transform.Y - _coverPadding)
                    _itemBlacklist.Add(i);
            }
        }

        #endregion
    }
}
