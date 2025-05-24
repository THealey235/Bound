using Bound.Controls.Game;
using Bound.Managers;
using Bound.Models.Items;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Bound.States
{
    public class Level : State
    {
        protected List<Block> _blocks;
        protected List<Rectangle> _blockRects;
        protected List<HotbarSlot> _hotbarSlots;

        public Level(Game1 game, ContentManager content, Player player, int levelNum) : base(game, content)
        {
            Name = Game1.Names.Level0;
            _player = player;
            _game = game;
            _levelMap = _game.RetrieveLevelMap(levelNum);
            _scale = 1.5f;

            _player.UpdateAttributes(game.SaveIndex);
            _player.Save = game.SavesManager.ActiveSave;

            LoadContent();
        }


        public override void LoadContent()
        {
            _blocks = new List<Block>();
            _blockRects = new List<Rectangle>();
            _surfaces = _game.GenerateSurfaces(_levelMap, (int)(_scale * Game1.ResScale));
            for (int i = 0; i < _levelMap.Count; i++)
            {
                for (int j = 0; j < _levelMap[i].Count; j++)
                {
                    var index = _levelMap[i][j];
                    if (index == -1)
                        continue;
                    var position = new Vector2(_game.Textures.BlockWidth * j * _scale, _game.Textures.BlockWidth * i * _scale);
                    _blocks.Add(new Block(_game.Textures, _game.GraphicsDevice, index, position, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.1f));
                }
            }
            _blockRects = UpdateBlockRects();

            var hotbarBG = _game.Textures.HotbarBG;
            _hotbarSlots = new List<HotbarSlot>();
            var hotbarScale = 0.5f;
            var save = _game.SavesManager.ActiveSave;
            for (int i = 0; i < 3; i++)
            {
                _hotbarSlots.Add(
                    new HotbarSlot(
                        hotbarBG,
                        _game.Textures.HotbarSelectedSlot,
                        new Vector2((20 + (hotbarBG.Width + 10) * i * hotbarScale) * Game1.ResScale, 10 * Game1.ResScale),
                        _game,
                        _game.Player.Layer + 0.001f,
                        hotbarScale,
                        save.Inventory.GetItem(Textures.ItemType.HoldableItem, save.EquippedItems["hotbar"][i])
                    )
                );
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var p in Popups)
                p.Draw(gameTime, spriteBatch);

            foreach (var block in _blocks)
                block.Draw(gameTime, spriteBatch);

            _player.Draw(gameTime, spriteBatch);

            foreach (var slot in _hotbarSlots)
                slot.Draw(gameTime, spriteBatch);

            if (Game1.InDebug)
            {
                var pos = new Vector2(0, 0);
                var layer = 0.91f;
                var scale = 0.5f;

                spriteBatch.DrawString(
                    _game.Textures.Font,
                    $"Position: x: {Math.Round(_player.Position.X, 0, MidpointRounding.AwayFromZero)}" +
                        $", y: {Math.Round(_player.Position.Y, 0, MidpointRounding.AwayFromZero)}",
                    pos + _game.Camera.V2Transform, Game1.DebugColour, 0f, Vector2.Zero, scale, SpriteEffects.None, layer
                );
            }
        }

        public override void Update(GameTime gameTime)
        {
            var count = Popups.Count;
            if (count > 0)
            {
                Popups[^1].Update(gameTime);
                _game.Player.UpdateWhileStatic(gameTime);

                return;
            }
            for (int i = 0; i < count; i++)
            {
                Popups[i].Update(gameTime);
                if (count != Popups.Count)
                {
                    count = Popups.Count;
                    i--;
                }
            }
            _player.Update(gameTime, _blockRects);

            for (int i = 0; i < _hotbarSlots.Count; i++)
            {
                var hbSlot = _hotbarSlots[i];
                if (i == _game.Player.HotbarSlot - 1)
                    hbSlot.IsSelected = true;
                else hbSlot.IsSelected = false;
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        private List<Rectangle> UpdateBlockRects()
        {
            List<Rectangle> rects = new List<Rectangle>();
            foreach (var block in _blocks)
            {
                if (_game.Textures.GhostBlocks.Contains(block.Index))
                    continue;
                rects.Add(block.Rectangle);
            }
            return rects;
        }

        public void UpdateHotbarSlot(Item item, int index)
        {
            if (index > _hotbarSlots.Count)
                return;
            _hotbarSlots[index].Item = item;
        }
    }
}
