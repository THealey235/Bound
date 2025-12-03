using Bound.Controls.Game;
using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;


namespace Bound.States.Game
{
    public class Level0 : Level
    {
        public Level0(Game1 game, ContentManager content) : base(game, content, 0)
        {
            Name = "level0";
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var rows = new List<(List<TextureManager.CommonBlocks>, Color)>()
            {
                (new List<TextureManager.CommonBlocks>{TextureManager.CommonBlocks.DirtGradient0, TextureManager.CommonBlocks.DirtGradient1, TextureManager.CommonBlocks.DirtGradient2 }, Color.White),
            };
            rows.AddRange(Enumerable.Repeat((new List<TextureManager.CommonBlocks> { TextureManager.CommonBlocks.BlankTile }, Color.Black), 5));
            PadBottom(rows);

            var sectionPositions = new List<Vector2>()
            {
                new Vector2(400, 175),
                new Vector2(900, 175),
                new Vector2(1200, 175)
            };

            for (int i = 0; i < 1; i++)
                AddMob(new Mob(_game, "Zombie"), sectionPositions[0], TriggerType.Position, GenerateTriggerBounds(sectionPositions[0], new Vector2(100, 100)), (0.3f * i));

            AddMob(new Boss(_game, "Galahad"), sectionPositions[2], TriggerType.Position, GenerateTriggerBounds(sectionPositions[2], new Vector2(-100, -50), new Vector2(100, 50)), 5f);
        }
    }
}
