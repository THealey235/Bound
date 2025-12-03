using Bound.Controls;
using Bound.Controls.Game;
using Bound.Models;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bound.States.Popups.Game
{
    public class HeadsUpDisplay : Popup
    {
        #region Attributes

        private List<HotbarSlot> _hotbarSlots;
        private List<BorderedBox> _statusBoxes; //0: Health, 1: Stamina, 3: MP
        private List<int> _previousStatusBoxesWidth;
        private List<int> _currentStatusBoxesWidth;
        private List<BuffInfo> _buffInfo = new List<BuffInfo>();
        private Save _save;
        private List<int> _statusBoxesBlacklist = new List<int>();
        private int _hotbarSelectedSlot;
        private float _buffInfoScale = 0.5f;

        public float Layer;

        public Item HeldItem
        {
            get { return _hotbarSlots[_hotbarSelectedSlot].Item; }
        }
       
        #endregion

        public HeadsUpDisplay(Game1 game, ContentManager content, State parent) : base(game, content, parent)
        {
            Layer = _game.Player.Layer + 0.01f;
            _save = game.SavesManager.ActiveSave;
            _player = game.Player;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var slot in _hotbarSlots)
                slot.Draw(gameTime, spriteBatch);

            if (_hotbarSlots[_hotbarSelectedSlot].Item != null)
                _hotbarSlots[_hotbarSelectedSlot].Item.Draw(gameTime, spriteBatch);

            for (int i = 0; i < _statusBoxes.Count; i++)
            {
                if (_statusBoxesBlacklist.Contains(i)) 
                    continue;

                (_statusBoxes[i]).Draw(gameTime, spriteBatch);
            }
            foreach (var c in _buffInfo)
                c.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent()
        {
            var hotbarBG = _game.Textures.HotbarBG;
            _hotbarSlots = new List<HotbarSlot>();
            var hotbarScale = 0.65f;
            var save = _game.SavesManager.ActiveSave;
            for (int i = 0; i < 3; i++)
            {
                _hotbarSlots.Add(
                    new HotbarSlot(
                        hotbarBG,
                        _game.Textures.HotbarSelectedSlot,
                        new Vector2((20 + (hotbarBG.Width + 20) * i) * hotbarScale * Game1.ResScale, Game1.ScreenHeight - 10 * Game1.ResScale - hotbarBG.Height * hotbarScale * Game1.ResScale),
                        _game,
                        Layer,
                        hotbarScale,
                        save.EquippedItems["hotbar"][i]
                    )
                );
            }
            _hotbarSelectedSlot = _game.Player.HotbarSlot - 1;
            _hotbarSlots[_hotbarSelectedSlot].IsSelected = true;


            var basePos = new Vector2(15 * Game1.ResScale, 10 * Game1.ResScale);
            var barHeight = (int)(5 * Game1.ResScale);
            var increment = new Vector2(0, barHeight + 3 * Game1.ResScale);
            _statusBoxes = new List<BorderedBox>()
            {
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.Red,
                    basePos,
                    Layer,
                    (int)(_game.ActiveSave.MaxHealth * Game1.ResScale),
                    barHeight
                ),
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.Green,
                    basePos + increment,
                    Layer,
                    (int)(_game.ActiveSave.MaxStamina * Game1.ResScale),
                    barHeight
                ),
                new BorderedBox
                (
                    _game.Textures.BaseBackground,
                    _game.GraphicsDevice,
                    Color.BlueViolet,
                    basePos + increment * 2,
                    Layer,
                    (int)(_game.ActiveSave.MaxMana * Game1.ResScale),
                    barHeight
                )
            };
            foreach (var c in _statusBoxes)
                c.IgnoreCameraTransform = true;

            foreach (var buff in _player.Save.Buffs)
                AddBuff(buff);

            _currentStatusBoxesWidth = _statusBoxes.Select(x => x.Width).ToList();

        }

        public override void Update(GameTime gameTime)
        {
            _previousStatusBoxesWidth = new List<int>(_currentStatusBoxesWidth);
            _currentStatusBoxesWidth = (new List<float>() { _player.Health, _player.Mana, _player.Stamina}).Select(x => (int)(x * Game1.ResScale)).ToList();

            if (_hotbarSelectedSlot != _game.Player.HotbarSlot - 1)
            {
                _hotbarSelectedSlot = _game.Player.HotbarSlot - 1;
                foreach (var slot in _hotbarSlots)
                    slot.IsSelected = false;
                _hotbarSlots[_hotbarSelectedSlot].IsSelected = true;
            }

            foreach (var slot in _hotbarSlots)
                slot.Update(gameTime);

            _statusBoxesBlacklist.Clear();
            for (int i = 0; i < _statusBoxes.Count; i++)
            {
                if (_currentStatusBoxesWidth[i] <= 0)
                    _statusBoxesBlacklist.Add(i);
                else if (_previousStatusBoxesWidth[i] != _currentStatusBoxesWidth[i])
                    _statusBoxes[i].Width = _currentStatusBoxesWidth[i];
            }

            foreach (var c in _statusBoxes)
                c.Update(gameTime);

            for (int i = 0; i < _buffInfo.Count; i++)
            {
                if (_buffInfo[i].SecondsRemaining <= 0)
                {
                    _buffInfo.RemoveAt(i);
                    i--;
                    SetBuffPositions();
                    continue;
                }
                _buffInfo[i].Update(gameTime);
            }

        }

        public void UpdateHotbarSlot(Item item, int index)
        {
            if (!(index > _hotbarSlots.Count))
                _hotbarSlots[index].Item = (item.Name == "Blank") ? null : item;
        }

        public bool UseItem()
        {
            var item = _hotbarSlots[_hotbarSelectedSlot].Item;
            if (item != null)
            {
                _hotbarSlots[_hotbarSelectedSlot].Item.Use();
                return true;
            }
            return false;
        }

        public void RemoveFromHotbar(string itemName)
        {
            for (int i = 0; i < _hotbarSlots.Count; i++)
            {
                var item = _hotbarSlots[i].Item;
                if (item != null && item.Name == itemName)
                {
                    _hotbarSlots[i].Item = null;
                }
            }
        }

        public void AddBuff(Buff buff)
        {
            _buffInfo.Add(new BuffInfo(buff, _game, Vector2.Zero, _buffInfoScale, Layer - 0.001f));
            SetBuffPositions();
        }

        private void SetBuffPositions()
        {
            var gap = 5f * Game1.ResScale;
            var spacing = new Vector2(gap, gap);
            if (_buffInfo.Count > 0)
                spacing += new Vector2(_buffInfo[0].Width, _buffInfo[0].Height);
            var position = new Vector2(15 * Game1.ResScale, 25 * Game1.ResScale + _statusBoxes[^1].Height + gap);
            for (int i = 0; i < _buffInfo.Count; i++)
            {
                _buffInfo[i].Position = position;
                if (i != 0 && i % 8 == 0)
                    position += new Vector2(-spacing.X * 8, spacing.Y);
                else position += new Vector2(spacing.X, 0);

            }
        }
    }
}
