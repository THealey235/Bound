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
        private TextureCollection _textures;
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }

        public Dictionary<string, Attribute> Attributes;
        public TextureManager.ItemType Type { get; }
        public TextureCollection Textures { get { return _textures; } }

        public int Quantity = 1;

        public Item(TextureCollection textures, int id, string name, string description, TextureManager.ItemType type)
        {
            _textures = textures;
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Attributes = new Dictionary<string, Attribute>();
        }

        public Item(TextureCollection textures, int id, string name, string description, string attributes, TextureManager.ItemType type) : this(textures, id, name, description, type)
        {
            foreach (var attribute in attributes.Split(", ").ToList())
            {
                var attr = attributes.Split(" ");
                Attributes.Add(attr[0], new Attribute(attr[0], int.Parse(attr[1])));
            }
        }

        public Item Clone()
        {
            var output = new Item(_textures, Id, Name, Description, Type);
            output.Attributes = Attributes;
            output.Quantity = Quantity;

            return output;
        }

        public void Use()
        {
            return;
        }
    }
}
