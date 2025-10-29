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
        protected enum Type
        {
            Stat, Effect
        };

        protected enum Attributes
        {
            Vigor, Mind, Endurance, Strength, Dexterity, AmmoHandling, Precision, Arcane,
            Focus, MovementSpeed, PhysicalAttack, PhysicalDefence, Stamina, Health, MoneyMultiplier,
        };//Vigor is max HP, Health is current HP

        protected enum Buffs
        {
            DoubleJump
        }

        protected string _name;
        protected int _value;

        public string Name
        {
            get { return _name; }
        }
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Attribute(string name, int value)
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
