using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Bound.States.Popups;
using Bound.Models.Items;
using System.Linq;

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
        private bool _isPressed = false;
        private int _height;
        private int _width;
        private List<ChoiceBox> _items;
        private int _itemHeight;
        private int _maxItemsInView;
        private Game1 _game;
        private float _scrollAmmount = 0f;//how far the user has scrolled down the list. Between 0 and 1
        private List<Vector2> _itemPositions;
        private float _extraItemsHeight;
        private int _parentTopEdge;
        private List<int> _itemBlacklist = new List<int>(); //indexes to not draw when drawing items
        private float _cursorScaling = 1f;
        private float _mouseInitialY;
        private float _scrollInitialAmmount;
        private float _scrollableHeight;
        private bool _enableCursor = true;
        private int _selectedItemIndex = -1; //nothing selected as default
        private float _elementYSpacing;
        private float _minimumCursorHeight = 15f * Game1.ResScale;
        

        // the "cover" is the Bordered Boxes used to cover the top and bottom of the items when they go out of range (declared in LoadContent())
        // this adds extra height to them so that you can't see the item boxes disappear when they go out of range
        private float _coverPadding; 

        public Color PenColour;
        public float TextureScale = 1f;
        public int MaxShown;
        public float Layer;
        public int BarWidth;

        public float ElementYSpacing
        {
            get { return _elementYSpacing; }
            set
            {
                _elementYSpacing = value;
                Load(_game);
            }

        }
       

        public List<int> ItemBlacklist
        {
            get { return _itemBlacklist; }
        }      

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
                var pos = new Vector2(_bar.Position.X, _bar.Position.Y + (_scrollAmmount * _scrollableHeight));
                pos.Y = MathHelper.Clamp(pos.Y, _bar.Position.Y, _bar.Position.Y + _bar.Height - _cursorHeight);
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

        public int SelectedIndex
        {
            get { return _selectedItemIndex; }
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
            _parentTopEdge = topEdgeY;
            _coverPadding = 3 * Game1.ResScale;
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
            if (_enableCursor && mouseRectangle.Intersects(_cursorRectangle) &&
                _currentMouse.LeftButton == ButtonState.Pressed &&
                _previousMouse.LeftButton == ButtonState.Released ||
                _isPressed)
            {
                if (_previousMouse.LeftButton == ButtonState.Released)
                {
                    _mouseInitialY = _currentMouse.Y;
                    _scrollInitialAmmount = _scrollAmmount;
                }
                _isPressed = true;
            }
            if (_currentMouse.LeftButton == ButtonState.Released)
                _isPressed = false;

            if (_isPressed)
                FollowCursor();
        }


        public override void LoadContent(Game1 game, BorderedBox background, float allignment) => Load(game);

        private void Load(Game1 game)
        {
            var elementGap = 5 * Game1.ResScale;
            var pos = new Vector2(Position.X, Position.Y + elementGap);
            if (_elementYSpacing == 0)
                _elementYSpacing = _itemHeight + elementGap;

            _itemPositions = new List<Vector2>();
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].UpdatePosition(pos);
                _itemPositions.Add(pos);
                pos.Y += _elementYSpacing;
            }

            _maxItemsInView =  Math.Abs((int)((_height - (_items.Count - 1) * (_elementYSpacing - _itemHeight)) / _itemHeight));
            var extraItems = _items.Count - _maxItemsInView;
            _extraItemsHeight = extraItems > 0 ? (extraItems) * _elementYSpacing + elementGap : 0;

            var defaultPos = new Vector2(Position.X + _width - BarWidth, Position.Y);

            _cursorHeight = (int)_minimumCursorHeight;
            if (_extraItemsHeight == 0)
                _enableCursor = false;
            else if (_extraItemsHeight > _height - _minimumCursorHeight)
            {
                _scrollableHeight = (_height - _minimumCursorHeight);
                _cursorScaling = _extraItemsHeight / _scrollableHeight;
            }
            else
            {
                _scrollableHeight = _extraItemsHeight;
                _cursorHeight = _height - (int)_scrollableHeight;
            }

            _bar = new BorderedBox
            (
                game.Textures.BaseBackground,
                game.GraphicsDevice,
                Color.White,
                defaultPos,
                Layer,
                BarWidth,
                _height
            );

            _cursor = new BorderedBox
            (
                game.Textures.BaseBackground,
                game.GraphicsDevice,
                Color.Black,
                new Vector2(Position.X + _width - BarWidth, Position.Y),
                Layer + 0.0001f,
                BarWidth,
                _cursorHeight
            );


            _components = new List<Component>
            {
                //Top cover
                new BorderedBox
                (
                    game.Textures.BaseBackground,
                    game.GraphicsDevice,
                    Game1.MenuColorPalette[0],
                    new Vector2(Position.X, _parentTopEdge + (int)(1.5 * Game1.ResScale)),
                    Layer + 0.003f,
                    _width,
                    (int)(Position.Y - _parentTopEdge + _coverPadding)
                ) { IsBordered = false},
                //Bottom cover
                new BorderedBox
                (
                    game.Textures.BaseBackground,
                    game.GraphicsDevice,
                    Game1.MenuColorPalette[0],
                    new Vector2(Position.X, Position.Y + _height - _coverPadding - 2),
                    Layer + 0.003f,
                    _width,
                    (int)(Position.Y - _parentTopEdge + _coverPadding)
                ) { IsBordered = false},
            };

            if (Game1.InDebug)
            {
                _components.Add(new BorderedBox
                (
                    game.Textures.BaseBackground,
                    game.GraphicsDevice,
                    Color.Red,
                    Position,
                    Layer - 0.000001f,
                    _width,
                    _height
                ));
            }


            if (_enableCursor)
            {
                _components.Add(_bar);
                _components.Add(_cursor);
            }

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
            var y = MathHelper.Clamp(_mouseInitialY - _currentMouse.Y, -1 * _scrollableHeight, _scrollableHeight);
            y /= (_scrollableHeight);
            _scrollAmmount = Math.Clamp(_scrollInitialAmmount - y, 0, 1);
            _cursor.Position = CursorPositon;
            SetItemProperties();
        }

        private void SetItemProperties()
        {
            var translation = new Vector2(0, _scrollAmmount * _scrollableHeight);
            _itemBlacklist.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].UpdatePosition(_itemPositions[i] - translation);
                if (_items[i].Rectangle.Bottom < Position.Y + _coverPadding || _items[i].Rectangle.Top > Position.Y + _height - _coverPadding)
                    _itemBlacklist.Add(i);
            }
        }

        public void ChangeSelected(int index, object sender)
        {
            _selectedItemIndex = index;
            var x = sender.GetType();
            var list = new List<ChoiceBox>(_items);
            if (index != -1)
                list.RemoveAt(index);

            switch (x.Name)
            {
                case "ItemInfoBox":
                    foreach (var i in list)
                        ((ItemInfoBox)i).ResetColour();
                    break;
            }

        }
        #endregion
    }
}
