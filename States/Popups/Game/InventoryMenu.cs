using Bound.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Bound.Managers;
using Bound.Controls.Game;
using Bound.Models.Items;

namespace Bound.States.Popups.Game
{
    public class InventoryMenu : Popup
    {
        private List<Component> _components;
        public BorderedBox _background;
        private List<InventorySlot> _armour;
        private List<InventorySlot> _accessories;
        private List<InventorySlot> _hotbar;
        private List<InventorySlot> _skills;
        private List<(string Text, Vector2 Position)> _headings;
        private List<InventorySlot> _allSlots;
        private Vector2 _playerPosition;
        private int _selectedBoxIndex;
      
        public float Layer;

        public InventoryMenu(Game1 game, ContentManager content, State parent) : base(game, content, parent)
        {
            Name = Game1.Names.InventoryWindow;
            Layer = 0.79f;

            var bgWidth = Game1.ScreenWidth / 8 * 3;
            _headings = new List<(string Text, Vector2 Position)>()
            {
                ("Armour",  new Vector2(55 * Game1.ResScale, 5 * Game1.ResScale)),
                ("Accessories",  new Vector2(137 * Game1.ResScale, 5 * Game1.ResScale)),
                ("Hotbar",  new Vector2((bgWidth - _game.Textures.Font.MeasureString("Hotbar").X) / 2, 148 * Game1.ResScale)),
                ("Skills",  new Vector2((bgWidth - _game.Textures.Font.MeasureString("Skills").X) / 2, 190 * Game1.ResScale)),
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

           foreach (var pair in _headings)
                spriteBatch.DrawString(_game.Textures.Font, pair.Text, pair.Position + _background.Position, Game1.MenuColorPalette[2], 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);

            spriteBatch.Draw(_game.Textures.Sprites["Player"].Statics["Standing"], _playerPosition, null, Color.White, 0f, Vector2.Zero, 1.5f * Game1.ResScale, SpriteEffects.None, Layer + 0.001f);
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var bgPos = new Vector2(Game1.ScreenWidth / 2 + Game1.V2Transform.X - (eigthWidth * 3 / 2), eigthHeight + Game1.V2Transform.Y);

            _background = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[0],
                bgPos,
                Layer,
                eigthWidth * 3,
                eigthHeight * 6
            );

            _components = new List<Component>()
            {
                _background,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Discard_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = 0.75f,
                    RelativePosition = new Vector2((_background.Width - _game.Textures.Button.Width * Game1.ResScale * 0.75f) / 2, (_background.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = _background
                },
            };

            SetCustomiseButtons(_background);

        }


        private void SetCustomiseButtons(BorderedBox background)
        {
            var buttonTexture = _game.Textures.EmptyBox;
            var textureScale = 0.75f;
            var scale = textureScale * Game1.ResScale;
            var font = _game.Textures.Font;
            var layer = Layer + 0.001f;
            var ySpacing = (buttonTexture.Height * textureScale + 5) * Game1.ResScale;
            var xSpacing = (int)((buttonTexture.Width * textureScale + 5) * Game1.ResScale);

            var x = background.Width / 8;
            var y = background.Height / 12;

            _armour = new List<InventorySlot>();

            var armourIDs = new Dictionary<string, TextureManager.ItemType>() 
            { 
                {"headgear", TextureManager.ItemType.HeadGear },
                {"chestarmour", TextureManager.ItemType.ChestArmour },
                {"legarmour", TextureManager.ItemType.LegArmour },
                {"footwear", TextureManager.ItemType.Footwear },
            };

            _playerPosition = new Vector2(background.Position.X + (x + Game1.ResScale * 40), background.Position.Y + (y + Game1.ResScale * 25));

            var acc = 0;
            foreach (var kvp in armourIDs)
            {
                _armour.Add(new InventorySlot(_game, font, kvp.Key, _game.SavesManager.ActiveSave.GetEquippedItem(kvp.Key), kvp.Value, 0)
                {
                    Click = new EventHandler(Button_Armour_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x, y + ySpacing * acc),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
                acc++;
            }

            x *= 5;
            acc = 0;
            _accessories = new List<InventorySlot>();
            for (int i = 0; i < 8; i++)
            {
                if (i == 4) { acc = 0; x += xSpacing; }

                _accessories.Add(new InventorySlot(_game, font, "accessory", _game.SavesManager.ActiveSave.GetEquippedItem("accessory", i), TextureManager.ItemType.Accessory, i)
                {
                    Click = new EventHandler(Button_Accessory_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x, y + ySpacing * acc),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });

                acc++;
            }

            var hbSlots = 3;
            x = (int)((background.Width - ((hbSlots - 1) * (xSpacing) + (buttonTexture.Width * scale))) / 2);
            y = (int)(background.Height * (3f / 5f));
            _hotbar = new List<InventorySlot>();
            for (int i = 0; i < hbSlots; i++)
            {
                _hotbar.Add(new InventorySlot(_game, font, "hotbar", _game.SavesManager.ActiveSave.GetEquippedItem("hotbar", i), TextureManager.ItemType.HoldableItem, i)
                {
                    Click = new EventHandler(Button_Hotbar_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x + xSpacing * i, y),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
            }

            _skills = new List<InventorySlot>();
            var skillSlots = 3;
            x = (int)((background.Width - ((skillSlots - 1) * (xSpacing) + (buttonTexture.Width * scale))) / 2);
            y = (int)(background.Height * (4f / 5f) - 10 * Game1.ResScale);
            for (int i = 0; i < skillSlots; i++)
            {
                _skills.Add(new InventorySlot(_game, font, "skill", _game.SavesManager.ActiveSave.GetEquippedItem("skill", i), TextureManager.ItemType.Skill, i)
                {
                    Click = new EventHandler(Button_Skills_Clicked),
                    Layer = layer,
                    TextureScale = textureScale,
                    RelativePosition = new Vector2(x + xSpacing * i, y),
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = background
                });
            }

            _allSlots = new List<InventorySlot>();
            _allSlots.AddRange(_armour);
            _allSlots.AddRange(_accessories);
            _allSlots.AddRange(_hotbar);
            _allSlots.AddRange(_skills);

            _components.AddRange(_allSlots);

        }


        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape)
                && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape))
                Button_Discard_Clicked(new object(), new EventArgs());

        }

        public void UpdateSlot(TextureManager.ItemType type, string item)
        {
            var isHotbar = false;
            switch (type)
            {
                case TextureManager.ItemType.HeadGear:
                    _armour[0].ContainedItem = item; break;
                case TextureManager.ItemType.ChestArmour:
                    _armour[1].ContainedItem = item; break;
                case TextureManager.ItemType.LegArmour:
                    _armour[2].ContainedItem = item; break;
                case TextureManager.ItemType.Footwear:
                    _armour[3].ContainedItem = item; break;
                case TextureManager.ItemType.Accessory:
                    _accessories[_selectedBoxIndex].ContainedItem = item; break;
                case TextureManager.ItemType.Weapon:
                case TextureManager.ItemType.Consumable:
                    _hotbar[_selectedBoxIndex].ContainedItem = item; isHotbar = true; break;
                case TextureManager.ItemType.Skill:
                    _skills[_selectedBoxIndex].ContainedItem = item; break;
            }

            _game.SavesManager.ActiveSave.SetEquippedItems(GetSlotStates());
            
            if (isHotbar)
            {
                var level = (Level)(_game.CurrentState);
                level.HUD.UpdateHotbarSlot(_game.CurrentInventory.GetItem(type, item), _selectedBoxIndex);
            }

            _selectedBoxIndex = 0;
        }

        public string GetSlotStates()
        {
            var output = new List<string>();

            foreach (var slot in _allSlots)
                output.Add($"{slot.ID}: {slot.ContainedItem}");

            return String.Join(';', output);
        }

        #region Clicked Handlers
        private void Button_Discard_Clicked(object sender, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Armour_Clicked(object sender, EventArgs eventArgs)
        {
            var button = sender as InventorySlot;
            Parent.Popups.Add(new ItemFinder(_game, _content, Parent, button.ItemType, button.ID,  Layer + 0.01f, this));
            _selectedBoxIndex = button.Index;
        }

        private void Button_Accessory_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
            Parent.Popups.Add(new ItemFinder(_game, _content, Parent, button.ItemType, button.ID, Layer + 0.01f, this));
            _selectedBoxIndex = button.Index;
        }
        private void Button_Hotbar_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
            Parent.Popups.Add(new ItemFinder(_game, _content, Parent, new List<TextureManager.ItemType>() { TextureManager.ItemType.Weapon, TextureManager.ItemType.Consumable }, button.ID, Layer + 0.01f, this));
            _selectedBoxIndex = button.Index;
        }

        private void Button_Skills_Clicked(object sender, EventArgs e)
        {
            var button = sender as InventorySlot;
            Parent.Popups.Add(new ItemFinder(_game, _content, Parent, button.ItemType, button.ID, Layer + 0.01f, this));
            _selectedBoxIndex = button.Index;
        }

        #endregion

    }
}
