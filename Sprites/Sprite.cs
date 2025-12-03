using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bound.Models;
using System.Collections.Generic;
using System.Linq;
using Bound.Managers;
using Bound.States;
using Bound.Models.Items;

namespace Bound.Sprites
{
    public class Sprite : Component
    {
        #region Fields

        protected Models.TextureCollection _textures;
        protected Dictionary<string, Animation> _animations;
        protected Dictionary<string, Attribute> _attributes;
        protected List<Buff> _buffs = new List<Buff>();
        protected Dictionary<string, float> _buffAttributes = new Dictionary<string, float>();
        protected AnimationManager _animationManager;
        protected Inventory _inventory;
        protected bool _useHitboxOverride;
        protected Rectangle _overridenHitbox;

        protected float _layer { get; set; }

        protected Vector2 _origin { get; set; }

        protected Vector2 _position { get; set; }

        protected float _rotation { get; set; }
        protected virtual float _health { get; set; }
        protected virtual float _stamina { get; set; }
        protected virtual float _mana { get; set; }

        protected Color _colour;
        protected SpriteEffects _spriteEffects;

        private float _scale { get; set; }

        private bool _toUnlockEffects = false;
        protected Texture2D _texture;
        protected float g = 0.09f;
        protected float _dTime;
        protected float _terminalVelocity = 2;
        protected float Gravity = 0;
        protected string _name;
        protected DebugRectangle _debugRectangle;
        protected string _knockbackDirection;
        protected bool _inKnockback = false;
        protected float _knockbackAcceleration = -20f;
        protected float _knockbackInitialVelocity = 5f;
        protected Vector2 _knockbackVelocity;
        protected float _immunityTimer;
        protected Color _hitColour = Game1.BlendColors(Color.Red, Color.White, 0.5f);
        protected SpriteType _spriteType = SpriteType.Sprite;
        protected bool _lockEffects = false;
        protected float _knockbackDamageDealtOut = 1f;
        protected bool _blockThrowables = false;
        protected List<string> _consumableBlacklist = new List<string>();

        public enum SpriteType
        {
            Player,
            Mob,
            Sprite,
            Projectile
        }

        public List<Buff> Buffs
        {
            get { return _buffs; }
        }

        public Dictionary<string, float> ActiveBuffValues
        {
            get { return _buffAttributes; }
        }

        #endregion

        #region Properties
        public Vector2 Velocity;

        public Color Colour
        {
            get { return _colour; }
            set
            {
                _colour = value;
                if (_animationManager != null)
                    _animationManager.Colour = value;
            }
        }

        public SpriteEffects Effects
        {
            get
            {
                return _spriteEffects;
            }
            set
            {
                if (_lockEffects)
                    return;

                _spriteEffects = value;
                if (_animationManager != null)
                    _animationManager.Effects = value;

            }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsRemoved { get; set; }

        protected Game1 _game;
        public float Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;

                if (_animationManager != null)
                    _animationManager.Layer = _layer;
            }
        }

        protected float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (_animationManager != null)
                    _animationManager.Scale = FullScale;
            }
        }

        public float FullScale
        {
            get
            {
                return Scale * Game1.ResScale;
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = ScaledPosition;
            }
        }

        public Vector2 ScaledPosition
        {
            get
            {
                return new Vector2(Position.X * Game1.ResScale, Position.Y * Game1.ResScale);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                if (_useHitboxOverride)
                    return new Rectangle((int)Position.X, (int)Position.Y, (int)(_overridenHitbox.Width * _scale), (int)(_overridenHitbox.Height * _scale));

                else if (_texture != null)
                    return new Rectangle((int)(Position.X), (int)(Position.Y), (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));

                else if (_animationManager != null && _animationManager.CurrentAnimation != null)
                {
                    var animation = _animationManager.CurrentAnimation;

                    return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, (int)(animation.FrameWidth * _scale), (int)(animation.FrameHeight * _scale));
                }

                throw new System.Exception("Unknown sprite");
            }
        }

        public Rectangle ScaledRectangle
        {
            get
            {
                if (_useHitboxOverride)
                    return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, (int)(_overridenHitbox.Width * FullScale), (int)(_overridenHitbox.Height * FullScale));

                else if (_texture != null)
                    return new Rectangle((int)(ScaledPosition.X), (int)(ScaledPosition.Y), (int)(_texture.Width * FullScale), (int)(_texture.Height * FullScale));

                else if (_animationManager != null && _animationManager.CurrentAnimation != null)
                {
                    var animation = _animationManager.CurrentAnimation;

                    return new Rectangle((int)ScaledPosition.X - (int)Origin.X, (int)ScaledPosition.Y - (int)Origin.Y, (int)(animation.FrameWidth * FullScale), (int)(animation.FrameHeight * FullScale));
                }

                throw new System.Exception("Unknown sprite");
            }

        }

        public Vector2 TextureCenter
        {
            get 
            {
                if (_texture == null)
                    return Vector2.Zero;
                return new Vector2(_texture.Width / 2, _texture.Height / 2);
            }

        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;

                if (_animationManager != null)
                    _animationManager.Rotation = value;
            }
        }

        public Sprite Parent;
        protected float _speed = 100f;

        public bool IsImmune
        {
            get { return _immunityTimer > 0; }
        }

        public float Health
        {
            get { return _health; }
        }

        public float Mana
        {
            get { return _mana; }
        }

        public float Stamina
        {
            get { return _stamina; }
        }

        public float KnockbackDamageDealtOut
        {
            get { return _knockbackDamageDealtOut; }
        }

        public virtual Inventory Inventory
        {
            get { return _inventory; }
        }

        public SpriteType Type
        {
            get { return _spriteType; }
        }

        public Rectangle CustomHitbox
        {
            get { return _overridenHitbox; }
            set { _overridenHitbox = value; _useHitboxOverride = true; }
        }

        public bool UseCustomHitbox
        {
            get { return _useHitboxOverride; }
            set { _useHitboxOverride = value; }
        }

        public bool BlockThrowables
        {
            get { return _blockThrowables; }
            set { _blockThrowables = value; }
        }

        #endregion

        #region Methods

        protected Sprite()
        {

        }

        public Sprite(Texture2D texture, Game1 game)
        {
            SetValues(texture, game);
        }

        protected void SetValues(Texture2D texture, Game1 game)
        {
            _texture = texture;

            Origin = Vector2.Zero;

            Colour = Color.White;

            Scale = 1f;

            _game = game;

            _inventory = new Inventory(game, this);

            Reset();
        }

        public Sprite(Models.TextureCollection textureCollection, Game1 game)
        {
            _textures = textureCollection;
            if (_textures.Statics.ContainsKey("Standing"))
                SetValues(textureCollection.Statics["Standing"], game);
                
        }

        public override void Update(GameTime gameTime)
        {
            if (IsImmune)
                Colour = _hitColour;
            else
                Colour = Color.White;

            if (_animationManager != null)
                _animationManager.Update(gameTime);

            _dTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _buffAttributes.Clear();
            Buff buff;
            for (int i = 0; i < _buffs.Count; i++)
            {
                buff = _buffs[i];
                buff.DecrementTimer(_dTime);
                ApplyBuff(buff.Attributes, buff.Type);
                if (buff.SecondsRemaining <= 0)
                {
                    _buffs.RemoveAt(i);
                    _consumableBlacklist.Remove(buff.Source);
                    i--;
                }
            }
        }

        public virtual void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites = null, List<Sprite> dealsKnockback = null)
        {
            Update(gameTime);
            DoPhysics(surfaces, collideableSprites, dealsKnockback);
            _debugRectangle.Position = ScaledPosition;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null && _animationManager.IsPlaying == true)
                _animationManager.Draw(spriteBatch);
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, Effects, Layer);

            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        #region Sprite Collision
        public bool IsTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right + this.Velocity.X > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        public bool IsTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left + this.Velocity.X < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        public bool IsTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        public bool IsTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top + this.Velocity.Y < sprite.Rectangle.Bottom &&
                this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }
        #endregion

        #region Rectangle Collision
        public bool IsTouchingLeft(Rectangle rect)
        {
            return this.Rectangle.Right + this.Velocity.X > rect.Left &&
                this.Rectangle.Left < rect.Left &&
                this.Rectangle.Bottom > rect.Top &&
                this.Rectangle.Top < rect.Bottom;
        }

        public bool IsTouchingRight(Rectangle rect)
        {
            return this.Rectangle.Left + this.Velocity.X < rect.Right &&
                this.Rectangle.Right > rect.Right &&
                this.Rectangle.Bottom > rect.Top &&
                this.Rectangle.Top < rect.Bottom;
        }

        public bool IsTouchingTop(Rectangle rect)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > rect.Top &&
                this.Rectangle.Top < rect.Top &&
                this.Rectangle.Right > rect.Left &&
                this.Rectangle.Left < rect.Right;
        }

        public bool IsTouchingBottom(Rectangle rect)
        {
            return this.Rectangle.Top + this.Velocity.Y < rect.Bottom &&
                this.Rectangle.Bottom > rect.Bottom &&
                this.Rectangle.Right > rect.Left &&
                this.Rectangle.Left < rect.Right;
        }

        #endregion

        public object Clone()
        {
            var sprite = this.MemberwiseClone() as Sprite;

            if (_animations != null)
            {
                sprite._animations = this._animations.ToDictionary(c => c.Key, v => v.Value.Clone() as Animation);
                sprite._animationManager = sprite._animationManager.Clone() as AnimationManager;
            }

            return sprite;
        }

        //gravity is velocity and g is the increase in velocity each frame. Really, I should try to incorporate _dTime.
        protected float SimulateGravity(float gravity, float acceleration)
        {
            if (_terminalVelocity <= gravity)
                return gravity;
            else
                gravity += g + acceleration;

            return gravity;

        }

        protected virtual void DoPhysics(List<Rectangle> surfaces, List<Sprite> sprites = null, List<Sprite> dealsKnockback = null)
        {
            Velocity = new Vector2(0, 0);
            var inFreefall = true;

            if (!IsImmune)
                HandleMovements(ref inFreefall);
            Velocity *= _dTime;

            if (IsImmune)
                _immunityTimer -= (float)_dTime;

            if (inFreefall)
            {
                Gravity += SimulateGravity(g, 0);
                Velocity += new Vector2(0, Gravity);
            }


            Knockback();
            foreach (var surface in surfaces)
            {
                if ((Velocity.X > 0 && IsTouchingLeft(surface)))
                    SurfaceTouched("left", surface);
                else if (Velocity.X < 0 && IsTouchingRight(surface))
                    SurfaceTouched("right", surface);
                if ((Velocity.Y > 0 && IsTouchingTop(surface)) ||
                     (Velocity.Y < 0 && IsTouchingBottom(surface)))
                {
                    if (Velocity.Y > 0)
                        SurfaceTouched("top", surface);
                    else
                        SurfaceTouched("bottom", surface);
                    inFreefall = false;
                    Gravity = 0;
                }
            }

            CheckSpriteCollision(sprites, ref dealsKnockback);

            CheckJump(inFreefall);

            Position += Velocity;
        }

        protected virtual void SurfaceTouched(string surfaceFace, Rectangle surface)
        {
            switch (surfaceFace)
            {
                case "left":
                    Velocity.X = surface.Left - Rectangle.Right; break;
                case "right":
                    Velocity.X = surface.Right - Rectangle.Left; break;
                case "top":
                    Velocity.Y = surface.Top - Rectangle.Bottom; break;
                case "bottom":
                    Velocity.Y = surface.Bottom - Rectangle.Top; break;
            }
        }

        private void CheckSpriteCollision(List<Sprite> sprites, ref List<Sprite> dealsKnockback)
        {
            dealsKnockback = dealsKnockback ?? new List<Sprite>();
            if (sprites != null)
            {
                foreach (var sprite in sprites)
                {
                    if (sprite == this)
                        continue;

                    if (Velocity.X > 0 && IsTouchingLeft(sprite))
                    {
                        if (dealsKnockback.Contains(sprite))
                        {
                            StartKnocback("left", sprite.KnockbackDamageDealtOut);
                            sprite.StartKnocback("right", _knockbackDamageDealtOut);
                        }
                        else if (!(sprite.Type == SpriteType.Projectile))
                            Velocity.X = 0;
                    }
                    else if (Velocity.X < 0 && IsTouchingRight(sprite))
                    {
                        if (dealsKnockback.Contains(sprite))
                        {
                            StartKnocback("right", sprite.KnockbackDamageDealtOut);
                            sprite.StartKnocback("left", _knockbackDamageDealtOut);
                        }
                        else if (!(sprite.Type == SpriteType.Projectile))
                            Velocity.X = 0;
                    }
                    /*if (Velocity.Y > 0 && IsTouchingTop(sprite))
                    {
                        if (dealsKnockback.Contains(sprite))
                        {
                            StartKnocback("up", sprite.KnockbackDamageDealtOut);
                            sprite.StartKnocback("down", _knockbackDamageDealtOut);
                        }
                        else if (!(sprite.Type == SpriteType.Projectile))
                            Velocity.Y = 0;
                    }*/
                    if (Velocity.Y < 0 && IsTouchingBottom(sprite))
                    {
                        if (dealsKnockback.Contains(sprite))
                        {
                            StartKnocback("down", sprite.KnockbackDamageDealtOut);
                            sprite.StartKnocback("up", _knockbackDamageDealtOut);
                        }
                        else if (!(sprite.Type == SpriteType.Projectile))
                            Velocity.Y = 0;
                    }
                }
            }
        }

        public virtual void StartKnocback(string direction, float damage = 1f)
        {
            if (IsImmune)
            {
                switch (direction)
                {
                    case "down":
                    case "up":
                        Velocity.Y = 0; break;
                    case "left":
                    case "right":
                        Velocity.X = 0; break;
                }
            }

            _knockbackDirection = direction;
            _inKnockback = true;
            _immunityTimer = 0.25f; //seconds
            Damage(damage);

            switch (direction)
            {
                case "up":
                    _knockbackVelocity = new Vector2(0, -_knockbackInitialVelocity * 1.5f); break;
                case "down":
                    _knockbackVelocity = new Vector2(0, _knockbackInitialVelocity); break;
                case "left":
                    _knockbackVelocity = new Vector2(-_knockbackInitialVelocity, 1); break;
                case "right":
                    _knockbackVelocity = new Vector2(_knockbackInitialVelocity, 1); break;
                default:
                    return;
            }

            Velocity += _knockbackVelocity;
        }

        protected virtual void Knockback()
        {
            if (_inKnockback)
            {
                switch (_knockbackDirection)
                {
                    case "up":
                        _knockbackVelocity = new Vector2(0, _knockbackVelocity.Y - _knockbackAcceleration * (float)_dTime);
                        if (_knockbackVelocity.Y >= 0)
                        {
                            _inKnockback = false;
                            Velocity = Vector2.Zero;
                            Gravity = _knockbackVelocity.Y;
                            return;
                        }
                        else
                        {
                            Velocity -= new Vector2(0, Gravity);
                        }
                        break;
                    case "down":
                        _knockbackVelocity = new Vector2(0, _knockbackVelocity.Y + _knockbackAcceleration * (float)_dTime);
                        if (_knockbackVelocity.Y <= 0)
                            _inKnockback = false;
                        break;
                    case "left":
                        _knockbackVelocity = new Vector2(_knockbackVelocity.X - _knockbackAcceleration * (float)_dTime, 0);
                        if (_knockbackVelocity.X >= 0)
                            _inKnockback = false;
                        break;
                    case "right":
                        _knockbackVelocity = new Vector2(_knockbackVelocity.X + _knockbackAcceleration * (float)_dTime, 0);
                        if (_knockbackVelocity.X <= 0)
                            _inKnockback = false;
                        break;
                }

                Velocity += _knockbackVelocity;
            }
        }


        protected virtual void HandleMovements(ref bool inFreefall)
        {

        }

        protected virtual void CheckJump(bool inFreefall)
        {

        }

        //Runs when each state is reset due to a change in resolution
        public void Reset()
        {
            int width;
            int height;

            if (_useHitboxOverride)
            {
                width = _overridenHitbox.Width; height = _overridenHitbox.Height;
            }
            else
            {
                width = _texture.Width; height = _texture.Height;
            }

            _debugRectangle = new DebugRectangle
            (
                new Rectangle((int)ScaledPosition.X, (int)ScaledPosition.Y, width, height)
                , _game.GraphicsDevice
                , Layer + 0.01f,
                FullScale
            );
            if (_animationManager != null)
                _animationManager.Scale = FullScale;
        }

        public void UnlockEffects() => _lockEffects = false;

        public virtual void Kill(Level level) => level.RemoveMob(this);

        public virtual void Damage(float damage) => _health -= (damage >= 0) ? damage : 0f;

        public bool UseStamina(float staminaToUse)
        {
            if (staminaToUse <= _stamina)
            {
                _stamina -= staminaToUse;
                return true;
            }
            return false;
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse <= _mana)
            {
                _mana -= manaToUse;
                return true;
            }
            return false;
        }

        public void UpdateAttribute(Attribute attr)
        {
            if (!_attributes.ContainsKey(attr.Name))
                _attributes.Add(attr.Name, attr);
            else
                _attributes[attr.Name].Value += attr.Value;
        }

        public virtual bool GiveBuff(Buff buff)
        {
            foreach (var i in _buffs)
            {
                if (buff.Equals(i))
                {
                    i.ResetTimer(buff.SecondsRemaining);
                    return false;
                }
            }

            _buffs.Add(buff);

            if (buff.Type == Consumable.ConsumableTypes.Recovery)
            {
                foreach (var attr in buff.Attributes)
                {
                    //More cases need to be added when more recovery items with new effects are added such as a mana/stamian recovery potion
                    switch (attr.Name)
                    {
                        case "GLD":
                            Inventory.Money += (int)attr.Value; break;
                        case "HEAL":
                            _health += attr.Value; break;
                    }
                }
            }

            return true;
        }

        private void ApplyBuff(List<Attribute> attributes, Consumable.ConsumableTypes type)
        {
            if (type == Consumable.ConsumableTypes.Recovery)
                return;

            foreach (var attr in attributes)
            {
                if (_buffAttributes.ContainsKey(attr.Name))
                    _buffAttributes[attr.Name] += attr.Value;
                else
                    _buffAttributes.Add(attr.Name, attr.Value);
            }
        }

        private float GetAttributeValue(string name)
        {
            return _attributes[name].Value + (_buffAttributes.TryGetValue(name, out float value) ? value : 0);
        }



        public void AddToConsumableBlacklist(string name) => _consumableBlacklist.Add(name);
        public bool ConsumableBlacklistContains(string name) => _consumableBlacklist.Contains(name);

        #endregion
    }
}