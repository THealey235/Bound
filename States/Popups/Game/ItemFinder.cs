using Bound.Controls.Game;
using Bound.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;
using Bound.Managers;
using Bound.Models.Items;

namespace Bound.States.Popups
{
    public class ItemFinder : Popup
    {
        private List<Component> _components;
        private List<Item> _items;
        private List<Textures.ItemType> _filter;
        private ScrollingMenu _scrollingMenu;

        public float Layer;
        public Color PenColor = Game1.MenuColorPalette[2];
        private List<ChoiceBox> _itemBoxes = new List<ChoiceBox>();

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, Textures.ItemType filter, float layer) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.ItemFinder;
            Layer = layer;
            _filter = new List<Textures.ItemType>() { filter };
            LoadContent();
        }

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, List<Textures.ItemType> filter, float layer) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.ItemFinder;
            Layer = layer;
            _filter = filter;
            LoadContent();
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bgPos = new Vector2(Game1.ScreenWidth / 2 + Game1.V2Transform.X - (eigthWidth * 3 / 2), eigthHeight + Game1.V2Transform.Y);
            var spacing = 5 * Game1.ResScale;
            var bgDimensions = new Vector2((int)(eigthWidth * 3), (int)(eigthHeight * 6));
            var TextureScale = 1f;

            var background = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[0],
                bgPos,
                Layer,
                (int)bgDimensions.X,
                (int)bgDimensions.Y
            );


            var componentHeight = (int)(10 * Game1.ResScale * TextureScale);
            var xOffset = bgDimensions.X / 8;
            var yOffset = bgDimensions.Y / 10;
            var offsetForButtons = 5 * Game1.ResScale;
            var menuWidth = (int)(bgDimensions.X - 2 * xOffset);
            var menuHeight = (int)(bgDimensions.Y - 2 * yOffset - offsetForButtons);
            var menuPos = new Vector2(background.Position.X + xOffset, background.Position.Y + yOffset - offsetForButtons);

            _items = _game.SavesManager.ActiveSave.Inventory.GetParts(_filter);

            foreach (var item in _items)
            {
                _itemBoxes.Add(
                    new ItemInfoBox(
                        _game, 
                        item,
                        Vector2.Zero,
                        (int)(menuWidth - 10 * Game1.ResScale),
                        menuHeight / 6, Layer + 0.002f,
                        background,
                        new EventHandler(Button_ItemPressed)
                    )
                );
            }

            _scrollingMenu = new ScrollingMenu
            (
                _game,
                $"ItemFinder, Filter: {String.Join(", ", _filter)}",
                menuPos,
                menuWidth,
                menuHeight,
                Layer + 0.001f,
                _itemBoxes,
                componentHeight
            );

            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(background.Width / 2 - (int)(_game.Textures.Button.Width * Game1.ResScale * 0.75f) - spacing / 2, (background.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Equip",
                    Click = new EventHandler(Button_Equip_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(background.Width / 2 + spacing / 2, (background.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                _scrollingMenu,
            };

            _components.AddRange(_itemBoxes);


            _scrollingMenu.LoadContent(_game, background, 0f);
        }


        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);


            if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape)
                && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape))
                Button_Discard_Clicked(new object(), new EventArgs());

        }

        private void Button_Discard_Clicked(object v, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Equip_Clicked(object v, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        private void Button_ItemPressed(object sender, EventArgs e)
        {

        }

    }
}
