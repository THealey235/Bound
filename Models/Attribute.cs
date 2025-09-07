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

        private enum Type
        {
            Stat, Effect
        };

        public enum Attributes
        {
            Vigor, Mind, Endurance, Strength, Dexterity, AmmoHandling, Precision, Arcane,
            Focus, MovementSpeed, PhysicalAttack, PhysicalDefence, Stamina, Health, MoneyMultiplier,

            DoubleJump
        };//Vigor is max HP, Health is current HP

        public enum AttributeAbreviations
        {
            HP, MP, END, STR, DEX, AMMH, PRC, ARC, FOC, MVMS, PATK, PDEF, STAM, HEAL, MONEYMULT,
            DJMP
        }

        public string Name;
        public int Value;

        public Attribute(string name, int value)
        {
            Name = name;
            Value = value;
            var x = Attributes.AmmoHandling.ToString();
        }

        public override string ToString()
        {
            return $"{Name} {Value.ToString()}";
        }
    }
}
