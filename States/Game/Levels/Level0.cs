using Bound.Controls.Game;
using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Bound.States.Game
{
    public class Level0 : Level
    {
        public Level0(Game1 game, ContentManager content, Player player) : base(game, content, player, 0)
        {
            Name = "level0";
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var rows = new List<(List<Textures.Blocks>, Color)>()
            {
                (new List<Textures.Blocks>{Textures.Blocks.DirtGradient0, Textures.Blocks.DirtGradient1, Textures.Blocks.DirtGradient2 }, Color.White),
            };
            rows.AddRange(Enumerable.Repeat((new List<Textures.Blocks> { Textures.Blocks.BlankTile }, Color.Black), 5));
            PadBottom(rows);
        }
    }
}
