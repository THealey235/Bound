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

            AddMob(new Mob(_game, "Zombie"), new Vector2(10, 100), TriggerType.Position, (new Vector2(300, 0), new Vector2(400, 400)));
            AddMob(new Boss(_game, "Galahad"), new Vector2(200, 300), TriggerType.Time, null, 5f);
        }
    }
}
