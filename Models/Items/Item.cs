using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bound.Managers;

namespace Bound.Models.Items
{
    public class Item
    { 
        private Texture2D _texture;

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }

        public Dictionary<string, Attribute> Attributes;
        public Textures.ItemType Type { get; }
        public Texture2D Texture { get { return _texture; } }

        public int Quantity = 1;

        public Item(Texture2D texture, int id, string name, string description, Textures.ItemType type)
        {
            _texture = texture;
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Attributes = new Dictionary<string, Attribute>();
        }

        public Item(Texture2D texture, int id, string name, string description, string attributes, Textures.ItemType type) : this(texture, id, name, description, type)
        {
            foreach (var attribute in attributes.Split(", ").ToList())
            {
                var attr = attributes.Split(" ");
                Attributes.Add(attr[0], new Attribute(attr[0], int.Parse(attr[1])));
            }
        }

        public Item Clone()
        {
            var output = new Item(_texture, Id, Name, Description, Type);
            output.Attributes = Attributes;
            output.Quantity = Quantity;

            return output;
        }
    }
}
