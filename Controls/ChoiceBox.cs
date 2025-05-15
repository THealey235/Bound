using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls
{
    public abstract class ChoiceBox : Component
    {
        public EventHandler OnApply;
        public string Name = string.Empty;

        //Probably should be in children not here
        public abstract void LoadContent(Game1 game, BorderedBox background, float allignment = 0f);

        public abstract void UpdatePosition(Vector2 position);
    }
}
