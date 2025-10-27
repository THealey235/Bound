using Bound.Controls.Game;
using Bound.Managers;
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
        protected List<Block> _blocks;
        protected List<Rectangle> _blockRects;
        protected List<Sprite> _mobs;
        protected List<Sprite> _damagesMobs;
        protected List<Sprite> _sprites;
        protected HeadsUpDisplay _HUD;
        protected (float Min, float Max) _mapBounds;
        protected (Vector2 Min, Vector2 Max) _cameraBounds;

        public List<Sprite> ToKill = new List<Sprite>();

        public HeadsUpDisplay HUD
        {
            get { return _HUD; }
        }

        public (float Min, float Max) MapBounds
        {
            get { return _mapBounds; }
        }

        public (Vector2 Min, Vector2 Max) CameraBounds
        {
            get { return  _cameraBounds; }
        }

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

            _mapBounds = (0, (_levelMap.Select(x => x.Count).Aggregate(0, (a, x) => (x > a) ? x : a) - 1) * _game.Textures.BlockWidth * _scale);
            _cameraBounds = (
                new Vector2(0.5f * Game1.ScreenWidth, 0.5f * Game1.ScreenHeight),
                new Vector2(
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenWidth + _player.Rectangle.Width * Game1.ResScale,
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenHeight + _player.Rectangle.Height * Game1.ResScale)
            );

            _mobs = new List<Sprite>()
            {
                new Mob(_game.Textures.Sprites["Zombie"], _game, 10f, 10f, 10f)
            };

            _damagesMobs = new List<Sprite>() { _player };

            _sprites = new List<Sprite>();
            _sprites.AddRange( _mobs );
            _sprites.AddRange( _damagesMobs );

            _player.Level = this;
            _mobs[0].Position = new Vector2(10, 100);

            _HUD = new HeadsUpDisplay(_game, _content, this);
            _HUD.LoadContent();
        }

        //pad bottom with rows of tiles so that it looks nice.
        protected void PadBottom(List<(List<TextureManager.CommonBlocks> Block, Color Colour)> rows)
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
                        (int)rows[row].Block[index],
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
            foreach (var sprite in ToKill)
            {
                _sprites.Remove(sprite);
                _mobs.Remove(sprite);
                _damagesMobs.Remove(sprite);
            }

            foreach (var p in Popups)
                p.Draw(gameTime, spriteBatch);

            var toDraw = _blocks.Select(x => _game.ToCull(x.ScaledRectangle) ? null : x).ToList();
            toDraw.RemoveAll(x => x == null);
            foreach (var block in toDraw)
                block.Draw(gameTime, spriteBatch);

            _player.Draw(gameTime, spriteBatch);

            foreach (var sprite in _mobs)
                sprite.Draw(gameTime, spriteBatch);

            _HUD.Draw(gameTime, spriteBatch);

            if (Game1.InDebug)
            {
                var pos = new Vector2(0, 0);
                var layer = 0.91f;
                var scale = 0.5f;

                spriteBatch.DrawString(
                    _game.Textures.Font,
                    $"Position: x: {Math.Round(_player.Position.X, 0, MidpointRounding.AwayFromZero)}" +
                        $", y: {Math.Round(_player.Position.Y, 0, MidpointRounding.AwayFromZero)}",
                    pos + Game1.V2Transform, Game1.DebugColour, 0f, Vector2.Zero, scale, SpriteEffects.None, layer
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

            _HUD.Update(gameTime);

            //_game.Items[item.Name] is necessary because if it is a sub-class of Item we want that class not the Item class returned by the HUD since it has been cast to an Item 
            var item = _HUD.HeldItem;
            if (item != null)
                item.Update(gameTime, _mobs);

            _player.Position = new Vector2(Math.Clamp(_player.Position.X, _mapBounds.Min, _mapBounds.Max), _player.Position.Y);
            foreach (var sprite in _mobs)
                sprite.Position = new Vector2(Math.Clamp(sprite.Position.X, _mapBounds.Min, _mapBounds.Max), sprite.Position.Y);

            _player.Update(gameTime, _blockRects, _sprites, _mobs);
            foreach (var sprite in _mobs)
                sprite.Update(gameTime, _blockRects, _sprites, _damagesMobs);

            for (int i = 0; i < _sprites.Count; i++)
                if (_sprites[i].Health <= 0)
                    _sprites[i].Kill(this);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }
        protected List<Rectangle> UpdateBlockRects()
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

        public void RemoveMob(Sprite mob) => ToKill.Add(mob);
    }
}
