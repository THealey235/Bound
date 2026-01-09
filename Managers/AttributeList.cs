using System.Collections.Generic;
using System.Linq;


namespace Bound.Managers
{
    public class AttributeList
    {
        private Dictionary<string, float> _attributes = new Dictionary<string, float>();

        public Dictionary<string, float> Dictionary
        {
            get { return new Dictionary<string, float>(_attributes); }
        }

        public AttributeList(Dictionary<string, float> attributes)
        {
            _attributes = attributes;
        }

        //kvps such that: "STR: 10; MP: 1.0;VIT ...."
        public AttributeList(string kvps)
        {
            _attributes = kvps.Split(';').Select(x => x.Split("; ")).ToDictionary(x => x[0], x => float.Parse(x[1]));
        }

        public AttributeList(Dictionary<string, Models.Attribute> attributes)
        {
            _attributes = attributes.Select(x => (x.Key, x.Value.Value)).ToDictionary(x => x.Key, x => x.Value);
        }

        public float this[string key]
        {
            get => _attributes[key];
            set => _attributes[key] = value;
        }

        public void Add(string key, float value)
        { 
            if (_attributes.ContainsKey(key))
                _attributes[key] += value;
            else _attributes[key] = value; 
        }

        public bool TryGetValue(string key, out float value) => _attributes.TryGetValue(key, out value);

        public bool ContainsKey(string key) => _attributes.ContainsKey(key);


        public static Dictionary<string, float> operator +(AttributeList a, AttributeList b)
        {
            var x = a.Dictionary;

            foreach (var kvp in x)
            {
                if (b.TryGetValue(kvp.Key, out float val))
                    x[kvp.Key] += val;
            }

            foreach (var kvp in b.Dictionary)
            {
                if (!x.ContainsKey(kvp.Key))
                    x[kvp.Key] = kvp.Value;
            }

            return x;
        }
    }
}
