using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Controls.Settings
{
    public abstract class MultiChoice : Component
    {
        public EventHandler OnApply;
        public abstract void LoadContent(Game1 game, BorderedBox background, float allignment);
    }
}
