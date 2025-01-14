using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace Bound.States.Game
{
    public class Level0 : State
    {
        private Player _player;
        private Game1 _game;
        private float deltaTime;
        private float pTime = 0f;
        private List<List<int>> _levelMap;
        private float _scale;

        public Level0(Game1 game, ContentManager content, Player player) : base(game, content)
        {
            Name = "levelzero";
            _player = player;
            _game = game;
            _levelMap = _game.RetrieveLevelMap(0);
            _scale = 1.5f;
            
            LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var p in Popups)
                p.Draw(gameTime, spriteBatch);

            for (int i = 0; i < _levelMap.Count; i++)
            {
                for (int j = 0; j < _levelMap[i].Count; j++)
                {
                    var index = _levelMap[i][j];
                    if (index == -1)
                        continue;
                    var position = new Vector2(_game.Textures.BlockWidth * j * _scale, _game.Textures.BlockWidth * i * _scale);
                    _game.Textures.DrawBlock(spriteBatch, index, position, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f, j);
                }
            }

            _player.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent()
        {
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            var count = Popups.Count;
            if (count > 0 && Popups[^1].Name == "settings")
            {
                Popups[^1].Update(gameTime);
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
            _player.Update(gameTime);
        }
    }
}
