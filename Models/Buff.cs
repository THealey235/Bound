using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Bound.Models
{
    public class Buff : Attribute
    {
        private Texture2D _icon;
        private string _source;
        private float _timer;

        public float SecondsRemaining
        {
            get { return _timer; }
        }

        public string Source
        {
            get { return _source; }
        }

        public Texture2D Icon
        {
            get { return _icon; }
        }

        public Buff(Texture2D icon, string source, string name, int value, float duration) : base(name, value)
        {
            _icon = icon;
            _source = source;
            _timer = duration;
        }

        public void DecrementTimer(float seconds) => _timer -= seconds;

        public override bool Equals(object obj)
        {
            var buff = obj as Buff;
            return _source == buff.Source && _name == buff.Name;
            
        }
    }
}
