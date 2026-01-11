using Bound.Controls;
using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models.Items
{
    public class Item
    {
        protected TextureCollection _textures;
        protected Rectangle _collisionRectangle;
        protected DebugRectangle _debugRectangle;
        protected List<Sprite> _spriteBlacklist = new List<Sprite>();
        protected Sprite _owner;
        protected Dictionary<string, float> _attackAttrs = new Dictionary<string, float> { { "PATK", 0f }, { "MATK", 0f } };
        protected int _quantity = 1;
        protected Dictionary<string, Attribute> _attributes = new Dictionary<string, Attribute>();
        protected float _scale;

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public virtual Sprite Owner { get; set; }

        public virtual float Scale { get; set; } //used for usable-items

        public Dictionary<string, Attribute> Attributes
        {
            get { return _attributes; }
        }

        public TextureManager.ItemType Type { get; }
        public TextureCollection Textures { get { return _textures; } }

        public float PATK
        {
            get
            {
                float physicalMult = 1f;
                if (_owner != null)
                {
                    physicalMult = _owner.ActiveBuffValues.TryGetValue("STR", out physicalMult) ? physicalMult : 1f;
                }

                return _attackAttrs["PATK"] * physicalMult;
            }
        }

        public float MATK
        {
            get
            {
                float mult = 1f;
                if (_owner != null)
                {
                    mult = _owner.ActiveBuffValues.TryGetValue("+MATK", out mult) ? mult : 1f;
                }

                return _attackAttrs["MATK"] * mult;
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set 
            {
                if (value >= 0)
                    _quantity = value;
                if (value == 0)
                    _owner.Inventory.RemoveItem(Name);
            }
        }

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

            Attribute a;
            if (Attributes.TryGetValue("PATK", out a))
                _attackAttrs["PATK"] = a.Value;
            if (Attributes.TryGetValue("MATK", out a))
                _attackAttrs["MATK"] = a.Value;
        }

        //used for clonning items
        public Item(TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, Dictionary<string, Attribute> attributes) 
        {
            _textures = textures;
            Id = id;
            Name = name;
            Description = description;
            Type = type;

            _attributes = attributes;

            Attribute a;
            if (Attributes.TryGetValue("PATK", out a))
                _attackAttrs["PATK"] = a.Value;
            if (Attributes.TryGetValue("MATK", out a))
                _attackAttrs["MATK"] = a.Value;
        }

        private void SetAttributes(string attributes)
        {
            if (attributes == "")
                return;
            foreach (var attribute in attributes.Split(", ").ToList())
            {
                var attr = attribute.Split(" ");
                Attributes.Add(attr[0], new Attribute(attr[0], float.Parse(attr[1])));
            }
        }

        public virtual Item Clone()
        {
            var output = new Item(_textures, Id, Name, Description, Type, Attributes);
            output.Quantity = Quantity;

            return output;
        }

        protected bool CheckCollision(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite.IsImmune || _spriteBlacklist.Contains(sprite) || sprite.Type == Sprite.SpriteType.DroppedItem)
                    continue;

                if (sprite.IsTouchingLeft(_collisionRectangle))
                    sprite.Damage("left", PATK, MATK);
                else if (sprite.IsTouchingRight(_collisionRectangle))
                    sprite.Damage("right", PATK, MATK);
                else if (sprite.IsTouchingTop(_collisionRectangle))
                    sprite.Damage("up", PATK, MATK);
                else if (sprite.IsTouchingBottom(_collisionRectangle))
                    sprite.Damage("down", PATK, MATK);

                //sprite will now be immune if it was hit
                if (sprite.IsImmune)
                    _spriteBlacklist.Add(sprite);
            }

            return (_spriteBlacklist.Count > _spriteBlacklist.Count) ? true : false;

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
