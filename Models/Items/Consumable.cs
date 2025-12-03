using Bound.Managers;
using Bound.Sprites;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models.Items
{
    public class Consumable : UsableItem
    {

        private static string[] _edibles = new string[] { 
            "Healing Potion", "Stamina Potion", "Stone Potion", "Magic Infused Potion" 
        };
        private static string[] _attackAttributes = new string[] { "PATK", "MATK" };

        public enum ConsumableTypes { NoCLDWNRecovery, Recovery, Buff, Projectile};

        private Texture2D _texture;
        private bool _isThrowable = false;
        private bool _isEdible;
        private float _cooldown;
        private float _cooldownTimer;
        private float _rotation;
        private bool _hasTexture;
        private float _buffDuration;
        private Vector2 _position = Vector2.Zero;
        private Vector2 _origin = Vector2.Zero;
        private ConsumableTypes _type;

        public ConsumableTypes ConsumableType
        {
            get { return  _type; }
        }

        public Consumable(Game1 game, TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, ConsumableTypes consumableType, string attributes = "") : base(game, textures, id, name, description, type, attributes)
        {
            _type = consumableType;
            SetValues();
        }

        private void SetValues()
        {
            _isEdible = _edibles.Contains(Name);
            _isThrowable = _type == ConsumableTypes.Projectile;

            if (Attributes.ContainsKey("CLDWN"))
                _cooldown = Attributes["CLDWN"].Value;
            else _cooldown = 1;

            if (Attributes.ContainsKey("DUR"))
                _buffDuration = Attributes["DUR"].Value;
            else _buffDuration = 20f;

            if (Textures.Statics.ContainsKey("Icon"))
            {
                _hasTexture = true;
                if (!Textures.Statics.TryGetValue("Projectile", out _texture))
                    _texture = Textures.Statics["Icon"];
            }

            _scale = 0.5f;
        }

        public Consumable(Game1 game, TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string consumableType, string attributes = "") : base(game, textures, id, name, description, type, attributes)
        {
            switch (consumableType.ReplaceLineEndings().Replace("\r\n", ""))
            {
                case "Projectile":
                    _type = ConsumableTypes.Projectile; break;
                case "No CLDWN Recovery":
                    _type = ConsumableTypes.NoCLDWNRecovery; break;
                case "Recovery":
                    _type = ConsumableTypes.Recovery; break;
                default:
                    _type = ConsumableTypes.Buff; break;
            }

            SetValues();
            
        }

        public override void Use()
        {
            if (!_hasTexture || _owner == null || (_isThrowable && _owner.BlockThrowables) || InUse || _owner.ConsumableBlacklistContains(Name)) return;
            InUse = true;
            _rotation = 0;
            _offset = Vector2.Zero;
            _origin = Vector2.Zero;
            _cooldownTimer = _cooldown;

            if (_isThrowable)
            {
                _owner.BlockThrowables = true;
                //Current state will always be a level if an item is being used
                (_game.CurrentState as Level).AddProjectile(new Projectile(_texture, Name, Owner, _game, Damage), Owner.Name != "player");

                //Quantity--;
            }
            else 
            {
                if (_isEdible)
                {
                    if (_owner.Effects == SpriteEffects.None)
                        _offset = new Vector2(_owner.ScaledRectangle.Width / 2, _owner.ScaledRectangle.Height / 3);
                    else
                    {
                        _offset = new Vector2(-_owner.ScaledRectangle.Width / 8, _owner.ScaledRectangle.Height / 3);
                        _origin = new Vector2(_texture.Width, 0);
                    }
                    SetRotationForEdible();
                }
                else
                {
                    if (_owner.Effects == SpriteEffects.None)
                        _offset = new Vector2(_owner.ScaledRectangle.Width / 2, _owner.ScaledRectangle.Height / 4);
                    else
                        _offset = new Vector2(-_owner.ScaledRectangle.Width / 5, _owner.ScaledRectangle.Height / 4);
                }

                if (_type == ConsumableTypes.Recovery)
                    _owner.AddToConsumableBlacklist(Name);

                _position = _owner.ScaledPosition;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (InUse && !_isThrowable)
                spriteBatch.Draw(_texture, _position + _origin + _offset, null, Color.White, _rotation, _origin, _scale * Game1.ResScale, _owner.Effects, _layer);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (InUse)
            {
                if (_isThrowable)
                {
                    if (_owner.BlockThrowables && _cooldownTimer <= 0)
                    {
                        _owner.BlockThrowables = false;
                        _owner.UnlockEffects();
                        InUse = false;
                    }
                }
                else
                {
                    if (_isEdible)
                        SetRotationForEdible();

                    _position = _owner.ScaledPosition;
                    if (_cooldownTimer <= 0)
                    {
                        InUse = false;
                        _owner.UnlockEffects();
                        if (_type != ConsumableTypes.NoCLDWNRecovery)
                        {
                            _owner.GiveBuff(new Buff(_texture, this, Attributes.Values.ToList(), _buffDuration));
                        }
                        //Quantity--;
                    }
                }
            }
        }

        private void SetRotationForEdible()
        {
            if (_owner.Effects == SpriteEffects.FlipHorizontally)
                _rotation = (1 - _cooldownTimer / _cooldown) * ((float)Math.PI / 2);
            else
                _rotation = 2 * (float)Math.PI - (1 - _cooldownTimer / _cooldown) * ((float)Math.PI / 2); //2π - progress * quater turn
        }

        public override Item Clone()
        {
            var newitem =  new Consumable(_game, Textures, Id, Name, Description, Type, _type, String.Join(", ", Attributes.Select(x => $"{x.Key} {x.Value.Value}")));
            newitem.Quantity = Quantity;

            return newitem;
        }
    }
}
