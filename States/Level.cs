using Bound.Managers;
using Bound.Models;
using Bound.Models.Items;
using Bound.Sprites;
using Bound.States.Popups.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Bound.States
{
    public class Level : State
    {
        public enum TriggerType
        {
            None,
            Position,
            Time
        }

        protected List<Block> _blocks;
        protected List<Rectangle> _blockRects;
        protected List<Sprite> _mobs;
        protected List<Sprite> _damagesMobs;
        protected List<Sprite> _sprites;
        protected List<Sprite> _projectiles;
        protected List<(Sprite Sprite, bool DamagedByPlayer)> _spritesToAdd = new List<(Sprite Sprite, bool DamagedByPlayer)>();
        protected List<UnspawnedMob> _unspawnedMobs = new List<UnspawnedMob>();
        protected HeadsUpDisplay _HUD;
        protected (float Min, float Max) _mapBounds;
        protected (Vector2 Min, Vector2 Max) _cameraBounds;
        private bool hasBeenLoaded = false;

        public List<Sprite> ToKill = new List<Sprite>();

        public HeadsUpDisplay HUD
        {
            get { return _HUD;}
        }

        public (float Min, float Max) MapBounds
        {
            get { return _mapBounds; }
        }

        public (Vector2 Min, Vector2 Max) CameraBounds
        {
            get { return  _cameraBounds; }
        }

        public Level(Game1 game, ContentManager content, int levelNum) : base(game, content)
        {
            game.ResetPlayer();
            Name = Game1.Names.Level0;
            _player = game.Player;
            _game = game;
            _levelMap = _game.RetrieveLevelMap(levelNum);
            _scale = Game1.BlockScale;

            _player.UpdateAttributes(game.SaveIndex);
            _player.Save = game.SavesManager.ActiveSave;
            _game.Player.Buffs = _game.ActiveSave.Buffs;
        }

        public override void LoadContent()
        {
            _blocks = new List<Block>();
            _blockRects = new List<Rectangle>();
            for (int i = 0; i < _levelMap.Count; i++)
            {
                for (int j = 0; j < _levelMap[i].Count; j++)
                {
                    var index = _levelMap[i][j];
                    if (index == -1)
                        continue;
                    var position = new Vector2(_game.Textures.BlockWidth * j * _scale, _game.Textures.BlockWidth * i * _scale);
                    _blocks.Add(new Block(_game.Textures, _game.GraphicsDevice, "common/" + _game.Textures.CommonBlocksKey[index], position, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.1f));
                }
            }
            _blockRects = UpdateBlockRects();

            _mapBounds = (0, (_levelMap.Select(x => x.Count).Aggregate(0, (a, x) => (x > a) ? x : a) - 1) * _game.Textures.BlockWidth * _scale);
            _cameraBounds = (
                new Vector2(0.5f * Game1.ScreenWidth, 0.5f * Game1.ScreenHeight),
                new Vector2(
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenWidth + _player.Rectangle.Width * Game1.ResScale,
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenHeight + _player.Rectangle.Height * Game1.ResScale)
            );

            if (hasBeenLoaded)
            {
                //if it has been loaded before, this means that it is being loaded again due to a change in resolution
                //so, not everything needs to be reset.

                foreach (var sprite in _sprites)
                    sprite.ResetScaling();

                _HUD.LoadContent();
            }
            else
            {
                _mobs = new List<Sprite>();

                _damagesMobs = new List<Sprite>() { _player };

                _sprites = new List<Sprite>();
                _sprites.AddRange(_mobs);
                _sprites.AddRange(_damagesMobs);

                _player.Level = this;

                _HUD = new HeadsUpDisplay(_game, _content, this);
                _HUD.LoadContent();

                hasBeenLoaded = true;
            }

            
        }

        //pad bottom with rows of tiles so that it looks nice.
        protected void PadBottom(List<(List<string> Block, Color Colour)> rows)
        {
            var index = 0;
            for (int row = 0; row < rows.Count; row++)
            {
                for (int i = 0; i < _levelMap[^1].Count; i++)
                {
                    if (rows[row].Block.Count == 1)
                        index = 0;
                    else
                        index = Game1.Random.Next(0, rows[row].Block.Count - 1);

                    _blocks.Add(new Block(
                        _game.Textures,
                        _game.GraphicsDevice,
                        rows[row].Block[index],
                        new Vector2(_game.Textures.BlockWidth * i * _scale, _game.Textures.BlockWidth * (_levelMap.Count + row) * _scale),
                        rows[row].Colour,
                        0f,
                        Vector2.Zero,
                        _scale,
                        SpriteEffects.None,
                        0.1f
                    ));
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (var p in Popups)
                p.Draw(gameTime, spriteBatch);

            var toDraw = _blocks.Select(block => _game.ToCull(block.ScaledRectangle) ? null : block).ToList();
            toDraw.RemoveAll(x => x == null);
            foreach (var block in toDraw)
                block.Draw(gameTime, spriteBatch);

            foreach (var sprite in _damagesMobs)
                sprite.Draw(gameTime, spriteBatch);

            foreach (var sprite in _mobs)
                sprite.Draw(gameTime, spriteBatch);

            _HUD.Draw(gameTime, spriteBatch);

            
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _unspawnedMobs.Count; i++)
            {
                var sprite = _unspawnedMobs[i];
                if (sprite.CheckSpawn(gameTime.ElapsedGameTime.TotalSeconds, _player))
                {
                     _spritesToAdd.Add((sprite.Sprite, true));
                    _unspawnedMobs.RemoveAt(i);
                    i--;
                }
            }

            if (_spritesToAdd.Count > 0)
            {
                foreach (var i in _spritesToAdd)
                {
                    if (i.DamagedByPlayer)
                        _mobs.Add(i.Sprite);
                    else _damagesMobs.Add(i.Sprite);

                    _sprites.Add(i.Sprite);
                }
                _spritesToAdd.Clear();
            }

            for (int i = 0; i < _sprites.Count; i++)
                if (_sprites[i].Health <= 0)
                    _sprites[i].Kill(this); // don't immediately remove it in case there is a death animation/process

            foreach (var sprite in ToKill)
            {
                _sprites.Remove(sprite);
                _mobs.Remove(sprite);
                _damagesMobs.Remove(sprite);
            }
            ToKill.Clear();

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
            
            if (_game.PlayerKeys.IsPressed("Menu", false) && Popups.Count == 0)
            {
                var options = new Options(_game, _game.Content, this);
                Popups.Add(options);
                options.Layer = _game.Player.Layer + 0.01f;
                options.LoadContent();
            }

            _HUD.Update(gameTime);

            var item = _HUD.HeldItem;
            if (item != null)
                item.Update(gameTime, _mobs);

            _player.Position = new Vector2(Math.Clamp(_player.Position.X, _mapBounds.Min, _mapBounds.Max), _player.Position.Y);
            foreach (var sprite in _mobs)
                sprite.Position = new Vector2(Math.Clamp(sprite.Position.X, _mapBounds.Min, _mapBounds.Max), sprite.Position.Y);

            foreach (var sprite in _damagesMobs)
                sprite.Update(gameTime, _blockRects, _sprites, _mobs);
            foreach (var sprite in _mobs)
                sprite.Update(gameTime, _blockRects, _sprites, _damagesMobs);
        }

        protected List<Rectangle> UpdateBlockRects()
        {
            List<Rectangle> rects = new List<Rectangle>();
            foreach (var block in _blocks)
            {
                if (_game.Textures.GhostBlocks.Contains(block.Name))
                    continue;
                rects.Add(block.Rectangle);
            }
            return rects;
        }

        public void RemoveMob(Sprite mob) => ToKill.Add(mob);

        public void AddProjectile(Projectile projectile, bool damagesPlayer)
        {
            _spritesToAdd.Add((projectile, damagesPlayer));
        }

        protected void AddMob(Sprite sprite, Vector2 spawnPosition, TriggerType type, (Vector2, Vector2)? positionBounds = null, float spawnDelay = 0)
        {
            sprite.Position = spawnPosition;
            _unspawnedMobs.Add(new UnspawnedMob(sprite, type, positionBounds, spawnDelay));
        }

        protected void AddContainer(string textureName, List<(string ItemName, int Count)> inventory, Vector2 blockwisePosition)
        {
            var blockWidth = _game.Textures.BlockWidth * _scale;
            _spritesToAdd.Add((   
                new Container(
                    _game,
                    textureName,
                    inventory,
                    new Vector2(blockwisePosition.X * blockWidth, blockwisePosition.Y * blockWidth)
                ),
                true
                )
            );
        }

        public void DropItem(Item item, Vector2 position)
        {
            _spritesToAdd.Add((new DroppedItem(_game, item, position), true));
        }

        protected void AddBlock(Block block)
        {
            _blocks.Add(block);
            _blockRects.Add(block.ScaledRectangle);
        }

        protected void AddBlock(Block block, Rectangle blockRectangle)
        {
            _blocks.Add(block);
            _blockRects.Add(blockRectangle);
        }

        protected void AddBlock(Block block, List<Rectangle> blockRectangles)
        {
            _blocks.Add(block);
            _blockRects.AddRange(blockRectangles);
        }
    }
}
