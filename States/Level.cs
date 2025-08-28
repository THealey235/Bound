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
using System.Security.Cryptography.Pkcs;


namespace Bound.States
{
    public class Level : State
    {
        protected List<Block> _blocks;
        protected List<Rectangle> _blockRects;
        protected List<Sprite> _sprites;
        protected HeadsUpDisplay _HUD;
        protected (float Min, float Max) _mapBounds;
        protected (Vector2 Min, Vector2 Max) _cameraBounds;

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

            _mapBounds = (0, (_levelMap.Select(x => x.Count).Aggregate(0, (a, x) => (x > a) ? x : a) - 1) * _game.Textures.BlockWidth * _scale);
            _cameraBounds = (
                new Vector2(0.5f * Game1.ScreenWidth, 0.5f * Game1.ScreenHeight),
                new Vector2(
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenWidth + _player.Rectangle.Width * Game1.ResScale,
                    _mapBounds.Max * Game1.ResScale - 0.5f * Game1.ScreenHeight + _player.Rectangle.Height * Game1.ResScale)
            );

            _sprites = new List<Sprite>()
            {
                _player
            };

            _HUD = new HeadsUpDisplay(_game, _content);
            _HUD.LoadContent();
        }

        //pad bottom with rows of tiles so that it looks nice.
        protected void PadBottom(List<(Textures.Blocks Block, Color Colour)> rows)
        {
            for (int row = 0; row < rows.Count; row++)
            {
                for (int i = 0; i < _levelMap[^1].Count; i++)
                {
                    _blocks.Add(new Block(
                        _game.Textures,
                        _game.GraphicsDevice,
                        (int)rows[row].Block,
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

            foreach (var block in _blocks)
                block.Draw(gameTime, spriteBatch);

            _player.Draw(gameTime, spriteBatch);

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

            _player.Update(gameTime, _blockRects);
            _HUD.Update(gameTime);

            foreach (var sprite in _sprites)
                sprite.Position = new Vector2(Math.Clamp(sprite.Position.X, _mapBounds.Min, _mapBounds.Max), sprite.Position.Y);
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
    }
}
