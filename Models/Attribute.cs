using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models
{
    public class Attribute
    {
        protected string _name;
        protected float _value = 1f;

        public string Name
        {
            get { return _name; }
        }
        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Attribute(string name, float value)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return $"{Name} {Value.ToString()}";
        }
    }
}
