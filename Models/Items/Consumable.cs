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

        private static float _one_quater_rotation = (MathHelper.Pi / 2) ;
        private static bool _blockThrowables;
        private static List<string> _throwablesPointingUp = new List<string> { "Throwing Dagger" };

        private Texture2D _texture;
        private bool _isThrowable;
        private float _cooldown;
        private float _cooldownTimer;
        private float _rotation;

        private bool _throwablePointingUp;
        private float _horizontalVelocity;
        private float _elapsedTime;
        private float _g = 9.81f;

        private bool _hasTexture;
        private float _buffDuration;
        private Vector2 _position = Vector2.Zero;
        private Vector2 _origin = Vector2.Zero;

        public Consumable(Game1 game, TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string attributes = "") : base(game, textures, id, name, description, type, attributes)
        {
            if (Attributes.ContainsKey("PATK") || Attributes.ContainsKey("MATK"))
            {
                _isThrowable = true;
                if (_throwablesPointingUp.Contains(name))
                    _throwablePointingUp = true;
            }
            else _isThrowable = false;

            if (Attributes.ContainsKey("CLDWN"))
                _cooldown = Attributes["CLDWN"].Value;
            else _cooldown = 1;

            if (Attributes.ContainsKey("DUR"))
                _buffDuration = Attributes["DUR"].Value;
            else _buffDuration = 20f;

            if (Textures.Statics.ContainsKey("Icon"))
            {
                _hasTexture = true;
                _texture = Textures.Statics["Icon"];
            }

            _scale = 0.5f; 
        }

        public override void Use()
        {
            if (!_hasTexture || _owner == null || (_isThrowable && _blockThrowables)) return;

            InUse = true;
            _rotation = 0;
            _offset = Vector2.Zero;
            _origin = Vector2.Zero;
            _horizontalVelocity = 100f * Game1.ResScale;
            _elapsedTime = 0f;

            if (_isThrowable)
            {
                _cooldownTimer = _cooldown;
                _blockThrowables = true;

                if (_throwablePointingUp)
                    _rotation = _one_quater_rotation;

                //Current state will always be a level if an item is being used
                (_game.CurrentState as Level).AddProjectile(new Projectile(_texture, Name, Owner, _game, _rotation, new Vector2(0, 0)), Owner.Name != "player");

                Quantity--;
            }
            else
            {
                _cooldownTimer = _cooldown;

                if (_owner.Effects == SpriteEffects.None)
                    _offset = new Vector2(_owner.ScaledRectangle.Width / 2, _owner.ScaledRectangle.Height / 2);
                else
                {
                    _offset = new Vector2(0, _owner.ScaledRectangle.Height / 2);
                    _origin = new Vector2(_texture.Width, 0);
                }

                SetRotationForEdible();
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
                    if (_blockThrowables && _cooldownTimer <= 0)
                    {
                        _blockThrowables = false;
                        _owner.UnlockEffects();
                    }
                }
                else
                {
                    SetRotationForEdible();
                    _position = _owner.ScaledPosition;
                    if (_cooldownTimer <= 0)
                    {
                        InUse = false;
                        _owner.UnlockEffects();
                        foreach (var attribute in Attributes.Values)
                            _owner.GiveBuff(new Buff(_texture, Name, attribute.Name, attribute.Value, _buffDuration));
                        Quantity--;
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
            var newitem =  new Consumable(_game, Textures, Id, Name, Description, Type, String.Join(',', Attributes.Select(x => $"{x.Key} {x.Value.Value}")));
            newitem.Quantity = Quantity;

            return newitem;
        }
    }
}
