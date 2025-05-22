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
using System.ComponentModel.DataAnnotations;
using Bound.States.Popups.Game;
using System.Reflection;

namespace Bound.States.Popups
{
    public class ItemFinder : Popup
    {
        private List<Component> _components;
        private List<Item> _items;
        private List<Textures.ItemType> _filter;
        private ScrollingMenu _scrollingMenu;
        private object _creator;
        private string _slotID;

        public float Layer;
        public Color PenColor = Game1.MenuColorPalette[2];
        private List<ChoiceBox> _itemBoxes = new List<ChoiceBox>();

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, Textures.ItemType filter, string slotID, float layer, object creator) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.ItemFinder;
            Layer = layer;
            _filter = new List<Textures.ItemType>() { filter };
            _creator = creator;
            _slotID = slotID;
            LoadContent();
        }

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, List<Textures.ItemType> filter, string slotID, float layer, object creator) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.ItemFinder;
            Layer = layer;
            _filter = filter;
            _creator = creator;
            _slotID = slotID;
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
            var bgPos = new Vector2(Game1.ScreenWidth / 2 + Game1.V2Transform.X - (eigthWidth * 2), eigthHeight + Game1.V2Transform.Y);
            var spacing = 5 * Game1.ResScale;
            var bgDimensions = new Vector2((int)(eigthWidth * 4), (int)(eigthHeight * 6));
            var textureScale = 0.65f;
            var scale = Game1.ResScale * 0.75f;

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


            var xOffset = bgDimensions.X / 14;
            var yOffset = bgDimensions.Y / 10;
            var offsetForButtons = 5 * Game1.ResScale;
            var menuWidth = (int)(bgDimensions.X - 2 * xOffset);
            var menuHeight = (int)(bgDimensions.Y - 2 * yOffset - offsetForButtons);
            var menuPos = new Vector2(background.Position.X + xOffset, background.Position.Y + yOffset - offsetForButtons);
            var componentHeight = (int)(menuHeight / 9);

            _items = _game.CurrentInventory.GetParts(_filter);
            var equipped = _game.SavesManager.ActiveSave.EquippedItems;
            var equippedExists = equipped.ContainsKey(_slotID);
            for (int i = 0; i < _items.Count; i++)
            {
                if (equippedExists && equipped[_slotID].Contains(_items[i].Name))
                {
                    _items.RemoveAt(i);
                    i--;
                    continue;
                }
                _itemBoxes.Add(
                    new ItemInfoBox(
                        _game,
                        _items[i],
                        Vector2.Zero,
                        (int)(menuWidth - 10 * Game1.ResScale),
                        componentHeight,
                        Layer + 0.002f,
                        background,
                        new EventHandler(Button_ItemPressed),
                        i
                    )
                    { TextureScale = textureScale}
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
                componentHeight,
                (int)(background.Position.Y)
            )
            {
                TextureScale = 0.8f,
            };

            foreach (var i in _itemBoxes) ((ItemInfoBox)i).Container = _scrollingMenu;

            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(background.Width / 2 - (int)(_game.Textures.Button.Width * scale * 1.5) - spacing / 2, (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Equip",
                    Click = new EventHandler(Button_Equip_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(background.Width / 2 + spacing + (int)(_game.Textures.Button.Width * scale * 0.5f), (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Clear",
                    Click = new EventHandler(Button_Clear_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2(background.Width / 2 - (_game.Textures.Button.Width * scale ) / 2, (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                _scrollingMenu,
            };

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

        private void Button_Discard_Clicked(object sender, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }
        private void Button_Clear_Clicked(object sender, EventArgs e)
        {
            switch (_creator.GetType().Name)
            {
                case "InventoryMenu":
                    ((InventoryMenu)_creator).UpdateSlot(_filter[0], "Default"); break;
            }
            Parent.Popups.Remove(this);
        }

        private void Button_Equip_Clicked(object sender, EventArgs eventArgs)
        {
            var index = _scrollingMenu.SelectedIndex;
            var equippedItem = _itemBoxes[index];
            switch (_creator.GetType().Name)
            {
                case "InventoryMenu":
                    ((InventoryMenu)_creator).UpdateSlot(_items[index].Type, _items[index].Name); break;
            }
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
