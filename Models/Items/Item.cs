using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bound.Models.Items
{
    public class Item
    { 
        protected TextureCollection _textures;
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public virtual Sprite User { get; set; }

        public Dictionary<string, Attribute> Attributes = new Dictionary<string, Attribute>();
        public TextureManager.ItemType Type { get; }
        public TextureCollection Textures { get { return _textures; } }

        public int Quantity = 1;

        public Item(TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string attributes = "")
        {
            _textures = textures;
            Id = id;
            Name = name;
            Description = description;
            Type = type;

            SetAttributes(attributes.ReplaceLineEndings().Replace(Environment.NewLine, string.Empty));
        }

        private void SetAttributes(string attributes)
        {
            if (attributes == "")
                return;
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

        public virtual void Use()
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
