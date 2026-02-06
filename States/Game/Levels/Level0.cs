using Bound.Managers;
using Bound.Models;
using Bound.Sprites;
using Bound.Sprites.Bosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace Bound.States.Game
{
    public class Level0 : Level
    {
        private bool hasBeenLoaded;

        public Level0(Game1 game, ContentManager content) : base(game, content, 0)
        {
            Name = "level0";
        }


        public override void LoadContent()
        {
            base.LoadContent();

            var rows = new List<(List<string>, Color)>()
            {
                (new List<string>{"common/DirtGradient0", "common/DirtGradient1", "common/DirtGradient2"}, Color.White),
            };
            rows.AddRange(Enumerable.Repeat((new List<string> { "common/BlankTile"}, Color.Black), 5));
            PadBottom(rows);

            if(!hasBeenLoaded)
            {
                hasBeenLoaded = true;

                var sectionPositions = new List<(Vector2 TL, Vector2 BR)>()
                {
                    (new Vector2(409, 30), new Vector2(553, 269)),
                    (new Vector2(625, 200), new Vector2(937, 270)),
                    (new Vector2(1080, 214), new Vector2(1584, 270))
                };

                _unspawnedMobs.Clear();

                for (int i = 0; i < 1; i++)
                    AddMob(new Mob(_game, "Zombie"), new Vector2(510, 258), TriggerType.Position, sectionPositions[0], (0.3f * i));

                AddMob(new Dwarfroot(_game), new Vector2(1380, 150), TriggerType.Position, sectionPositions[2], 0f);

                AddContainer("Pot", new List<(string, int)>() { ("Throwing Dagger", 10) }, new Vector2(10, 12));
                AddContainer("Pot", new List<(string, int)>() { ("Throwing Dagger", 10) }, new Vector2(6, 12));
                AddContainer("Pot", new List<(string, int)>() { ("Throwing Dagger", 10) }, new Vector2(2, 12));
                AddContainer("Pot", new List<(string ItemName, int Count)> { ("Super Cool Epic Chest Piece", 1) }, new Vector2(20, 12));

                _background = GetBackground(_game, _game.Textures.BlockWidth * (_levelMap.Count - 1) * _scale);
            }
        }

        public static Background GetBackground(Game1 game, float bottom)
        {
           return new Background(
                new List<Layer>()
                {
                        new Layer(new List<(Texture2D Texture, float Scale)>() { (game.Textures.GetAtlasItem("level0/Trunk0"), 1f)}, 0.09f, bottom, 200, 0f),
                        new Layer(new List<(Texture2D Texture, float Scale)>() { (game.Textures.GetAtlasItem("level0/Trunk0"), 0.9f)}, 0.08f, bottom, 180, 0.05f),
                        new Layer(new List<(Texture2D Texture, float Scale)>() { (game.Textures.GetAtlasItem("level0/Trunk0"), 0.8f)}, 0.07f, bottom, 160, 0.06f),
                        new Layer(new List<(Texture2D Texture, float Scale)>() { (game.Textures.GetAtlasItem("level0/Trunk0"), 0.7f)}, 0.06f, bottom, 140,0.07f),
                }
            );
        }
    }
}
