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
using Bound.States.Popups.Game;
using System.Linq;

namespace Bound.States.Popups
{
    public class ItemFinder : Popup
    {
        private List<Component> _components;
        private List<Item> _items;
        private List<TextureManager.ItemType> _filter;
        private ScrollingMenu _scrollingMenu;
        private object _creator;
        private string _slotID;
        private List<ItemInfoBox> _itemBoxes = new List<ItemInfoBox>();
        private BorderedBox _itemInfoBG;

        public float Layer;
        public Color PenColor = Game1.MenuColorPalette[2];

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, TextureManager.ItemType filter, string slotID, float layer, object creator) : base(game, content, parent, graphics)
        {
            Name = Game1.Names.ItemFinder;
            Layer = layer;
            _filter = new List<TextureManager.ItemType>() { filter };
            _creator = creator;
            _slotID = slotID;
            LoadContent();
        }

        public ItemFinder(Game1 game, ContentManager content, State parent, GraphicsDeviceManager graphics, List<TextureManager.ItemType> filter, string slotID, float layer, object creator) : base(game, content, parent, graphics)
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

            if (_scrollingMenu.SelectedIndex != -1)
                _itemBoxes[_scrollingMenu.SelectedIndex].DrawInfo(gameTime, spriteBatch);

        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var spacing = 5 * Game1.ResScale;
            var bgDimensions = new Vector2((int)(eigthWidth * 3.25f), (int)(eigthHeight * 6));
            var bgPos = new Vector2((Game1.ScreenWidth - bgDimensions.X) / 2 + Game1.V2Transform.X, eigthHeight + Game1.V2Transform.Y); 
            var textureScale = 0.65f;
            var scale = Game1.ResScale * textureScale;

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

            _itemInfoBG = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[1],
                new Vector2(bgPos.X + background.Width + 2 * Game1.ResScale, bgPos.Y),
                Layer,
                (int)(eigthWidth * 1.5f),
                (int)(eigthHeight * 3.5f)
            );

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
                        _itemInfoBG,
                        (int)(menuWidth - 10 * Game1.ResScale),
                        componentHeight,
                        Layer + 0.002f,
                        background,
                        new EventHandler(Button_ItemPressed),
                        i
                    )
                    { TextureScale = textureScale }
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
                _itemBoxes.Select(x => x as ChoiceBox).ToList(),
                componentHeight,
                (int)(background.Position.Y)
            )
            {
                TextureScale = 0.8f,
            };

            foreach (var i in _itemBoxes) 
                i.Container = _scrollingMenu;

            _components = new List<Component>()
            {
                background,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(bgDimensions.X / 2 - _game.Textures.Button.Width * scale * 1.5f - spacing / 2, (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Clear",
                    Click = new EventHandler(Button_Clear_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(bgDimensions.X / 2 - (_game.Textures.Button.Width * scale ) / 2, (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                },
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Equip",
                    Click = new EventHandler(Button_Equip_Clicked),
                    Layer = Layer + 0.01f,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(bgDimensions.X / 2 + spacing + _game.Textures.Button.Width * scale * 0.5f, (background.Height - _game.Textures.Button.Height * scale) - 5 * Game1.ResScale),
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
            Close();
        }

        private void Button_Clear_Clicked(object sender, EventArgs e)
        {
            switch (_creator.GetType().Name)
            {
                case "InventoryMenu":
                    ((InventoryMenu)_creator).UpdateSlot(_filter[0], "Default"); break;
            }
            Close();
        }

        private void Button_Equip_Clicked(object sender, EventArgs eventArgs)
        {
            var index = _scrollingMenu.SelectedIndex;
            if (index != -1)
            {
                var equippedItem = _itemBoxes[index];
                switch (_creator.GetType().Name)
                {
                    case "InventoryMenu":
                        ((InventoryMenu)_creator).UpdateSlot(_items[index].Type, _items[index].Name); break;
                }
            }
            else
            {
                Button_Clear_Clicked(sender, eventArgs);
            }

            Close();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        private void Button_ItemPressed(object sender, EventArgs e)
        {

        }

        private void Close()
        {
            Parent.Popups.Remove(this);
        }
    }
}
