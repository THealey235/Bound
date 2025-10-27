using Bound.Controls;
using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bound.Models.Items
{
    public class Item
    { 
        protected TextureCollection _textures;
        protected Rectangle _collisionRectangle;
        protected DebugRectangle _debugRectangle;
        protected List<Sprite> _spriteBlacklist = new List<Sprite>();
        protected Sprite _owner;

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public virtual Sprite Owner { get; set; }

        public Dictionary<string, Attribute> Attributes = new Dictionary<string, Attribute>();
        public TextureManager.ItemType Type { get; }
        public TextureCollection Textures { get { return _textures; } }

        public int Quantity = 1;

        public Rectangle CollisionRectangle
        {
            get {  return _collisionRectangle; }
        }

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

        public virtual Item Clone()
        {
            var output = new Item(_textures, Id, Name, Description, Type);
            output.Attributes = Attributes;
            output.Quantity = Quantity;

            return output;
        }

        protected bool CheckCollision(List<Sprite> sprites)
        {
            //will have to alter code for this and StartKnockback to take into account MATK and defence values for each type of attack
            var damage = Attributes.ContainsKey("PATK") ? Attributes["PATK"].Value : 1f;
            
            var length = _spriteBlacklist.Count;

            foreach (var sprite in sprites)
            {
                if (sprite.IsImmune || _spriteBlacklist.Contains(sprite))
                    continue;

                if (sprite.IsTouchingLeft(_collisionRectangle))
                    sprite.StartKnocback("left", damage);
                else if (sprite.IsTouchingRight(_collisionRectangle))
                    sprite.StartKnocback("right", damage);
                else if (sprite.IsTouchingTop(_collisionRectangle))
                    sprite.StartKnocback("up", damage);
                else if (sprite.IsTouchingBottom(_collisionRectangle))
                    sprite.StartKnocback("down", damage);

                //sprite will now be immune if it was hit
                if (sprite.IsImmune)
                    _spriteBlacklist.Add(sprite);
            }

            return (_spriteBlacklist.Count > length) ? true : false;

        }

        public virtual void Use()
        {
            if (Owner != null)
                Owner.UnlockEffects();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public virtual void Update(GameTime gameTime, List<Sprite> sprites)
        {
        }
    }
}
