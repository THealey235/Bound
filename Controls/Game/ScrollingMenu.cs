using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Bound.Controls.Game
{
    public class ScrollingMenu : ChoiceBox
    {
        #region Properties/Fields
        private List<Component> _components;
        private Vector2 _position;

        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private BorderedBox _cursor;
        private int _cursorWidth;
        private bool useInformationWindow = false;
        private bool _isPressed = false;
        private int _height;
        private int _width;
        private List<ChoiceBox> _items;
        private int _listStart;
        private int _itemHeight;
        private int _maxItemsInView;
        private Game1 _game;


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

        #endregion

        #region Constructor/Inherited Methods
        public ScrollingMenu(Game1 game, string name, Vector2 position, int width, int height, float layer, List<ChoiceBox> items, int componentHeight )
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
            _maxItemsInView = (height - 10) / componentHeight;
            BarWidth = (int)(5 * Game1.ResScale);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var item in _items)
                item.Draw(gameTime, spriteBatch);
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


            //if it has just been clicked on and left click is not being long pressed
            if (mouseRectangle.Intersects(new Rectangle(0, 0, 1, 1)) &&
                _currentMouse.LeftButton == ButtonState.Pressed &&
                _previousMouse.LeftButton == ButtonState.Released ||
                _isPressed)
            {
                _isPressed = true;
            }
            if (_currentMouse.LeftButton == ButtonState.Released)
                _isPressed = false;
        }


        public override void LoadContent(Game1 game, BorderedBox background, float allignment)
        {
            Load(_game);
        }

        private void Load(Game1 _game)
        {
            _components = new List<Component>
            {
                //the background of the scroll bar
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.White,
                    new Vector2(Position.X + _width - BarWidth, Position.Y),
                    Layer,
                    BarWidth,
                    _height
                ),
            };

            var pos = new Vector2(Position.X, Position.Y + 5 * Game1.ResScale);
            var ySpacing = _itemHeight + 5 * Game1.ResScale;
            for (int i = 0; i < _maxItemsInView && i < _items.Count; i++)
            {
                _items[i].UpdatePosition(pos);
                pos.Y += ySpacing;
            }
        }

        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
            Load(_game);
        }

        #endregion

        #region Other Methods

        #endregion
    }
}
