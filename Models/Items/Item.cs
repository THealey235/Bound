using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models.Items
{
    public class Item
    { 
        private Texture2D _texture;

        public int Id;
        public string Name;
        public string Description;
        public Dictionary<string, Attribute> Attributes;

        public Item(Texture2D texture, int id, string name, string description)
        {
            _texture = texture;
            Id = id;
            Name = name;
            Description = description;
            Attributes = new Dictionary<string, Attribute>();
        }

        public Item(Texture2D texture, int id, string name, string description, string attributes) : this(texture, id, name, description)
        {
            foreach (var attribute in attributes.Split(", ").ToList())
            {
                var attr = attributes.Split(" ");
                Attributes.Add(attr[0], new Attribute(attr[0], int.Parse(attr[1])));
            }
        }
    }
}
