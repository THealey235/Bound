using Bound.Models.Items;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static Bound.Models.Items.Consumable;

namespace Bound.Models
{
    public class Buff
    {
        private Texture2D _icon;
        private Consumable _source;
        private float _timer;
        private List<Attribute> _attributes;

        public float SecondsRemaining
        {
            get { return _timer; }
        }

        public string Source
        {
            get { return _source.Name; }
        }

        public Texture2D Icon
        {
            get { return _icon; }
        }

        public List<Attribute> Attributes
        {
            get { return _attributes; }
        }

        public ConsumableTypes Type
        {
            get { return _source.ConsumableType; }
        }

        public Buff(Texture2D icon, Consumable source, List<Attribute> attributes, float duration)
        {
            _icon = icon;
            _source = source;
            _timer = duration;
            _attributes = attributes;
        }

        //Used when loading a new save from disk
        public Buff (Game1 game, string source, float duration)
        {
            _source = (Consumable) game.Items[source];
            _icon = _source.Textures.GetIcon();
            _timer = duration;
            _attributes = _source.Attributes.Values.ToList();
        }

        public void DecrementTimer(float seconds) => _timer -= seconds;

        public override bool Equals(object obj)
        {
            var buff = obj as Buff;
            return _source.Name == buff.Source;
        }

        public override int GetHashCode()
        {
           return base.GetHashCode();
        }

        public void ResetTimer(float time) => _timer = time;
    }
}
