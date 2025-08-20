using Bound.Controls;
using Bound.Controls.Game;
using Bound.Managers;
using Bound.Models;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.States.Popups.Game
{
    public class HeadsUpDisplay : State
    {
        #region Attributes

        private List<HotbarSlot> _hotbarSlots;
        private List<BorderedBox> _statusBoxes; //0: Health, 1: Stamina, 3: MP
        private List<int> _previousStatusBoxesWidth;
        private List<int> _currentStatusBoxesWidth;
        private Save _save;
        private List<int> _statusBoxesBlacklist = new List<int>();

        public float Layer;

        #endregion

        public HeadsUpDisplay(Game1 game, ContentManager content) : base(game, content)
        {
            Layer = _game.Player.Layer + 0.01f;
            _save = game.SavesManager.ActiveSave;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var slot in _hotbarSlots)
                slot.Draw(gameTime, spriteBatch);

            for (int i = 0; i < _statusBoxes.Count; i++)
            {
                if (_statusBoxesBlacklist.Contains(i)) 
                    continue;

                (_statusBoxes[i]).Draw(gameTime, spriteBatch);
            }
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

            var barScale = 1f;
            var basePos = new Vector2(20 * barScale * Game1.ResScale, 10 * Game1.ResScale);
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
                c.ToCenter = true;

            _currentStatusBoxesWidth = _statusBoxes.Select(x => x.Width).ToList();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            _previousStatusBoxesWidth = new List<int>(_currentStatusBoxesWidth);
            _currentStatusBoxesWidth = (new List<float>() { _save.Health, _save.Mana, _save.Stamina}).Select(x => (int)(x * Game1.ResScale)).ToList();

            for (int i = 0; i < _hotbarSlots.Count; i++)
            {
                var hbSlot = _hotbarSlots[i];
                if (i == _game.Player.HotbarSlot - 1)
                    hbSlot.IsSelected = true;
                else hbSlot.IsSelected = false;
            }

            _statusBoxesBlacklist.Clear();
            for (int i = 0; i < _statusBoxes.Count; i++)
            {
                if (_currentStatusBoxesWidth[i] == 0)
                {
                    _statusBoxesBlacklist.Add(i); continue;
                }

                if (_previousStatusBoxesWidth[i] != _currentStatusBoxesWidth[i])
                    _statusBoxes[i].Width = _currentStatusBoxesWidth[i];

            }

            foreach (var c in _statusBoxes)
                c.Update(gameTime);
        }

        public void UpdateHotbarSlot(Item item, int index)
        {
            if (index > _hotbarSlots.Count)
                return;
            _hotbarSlots[index].Item = item;
        }
    }
}
