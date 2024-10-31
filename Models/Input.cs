using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Bound.Models
{
    public class Input
    {
        public Dictionary<string, Keys> Keys;

        public Input(Dictionary<string, Keys> keys)
        {
            Keys = keys;
        }
    }
}
