using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Bound.States.Popups
{
    public abstract class Popup : State
    {
        public State Parent;

        public Popup(Game1 game, ContentManager content, State parent) : base(game, content)
        {
            Parent = parent;
        }
    }
}
