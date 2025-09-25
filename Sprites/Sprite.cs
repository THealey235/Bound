using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Bound.Models;
using Bound.Managers;

namespace Bound.Sprites
{
    public class Sprite : Component, ICloneable
    {
        #region Fields
        protected Models.TextureCollection _textures;

        protected Dictionary<string, Animation> _animations;

        protected AnimationManager _animationManager;

        protected float _layer { get; set; }

        protected Vector2 _origin { get; set; }

        protected Vector2 _position { get; set; }

        protected float _rotation { get; set; }

        private Color _colour;

        private float _scale { get; set; }

        private SpriteEffects _spriteEffects;

        protected Texture2D _texture;
        protected float g = 0.09f;
        protected double _dTime;
        protected float _terminalVelocity = 2;
        protected float Gravity = 0;
        protected string _name;
        protected DebugRectangle _debugRectangle;
        protected string _knockbackDirection;
        protected bool _inKnockback = false;
        protected float _knockbackDuration;
        protected float _knockbackAcceleration = -0.2f;
        protected float _knockbackTimer;
        protected float _knockbackInitialVelocity = 10f;
        protected Vector2 _knockbackVelocity;

        #endregion

        #region Properties
        public Vector2 Velocity;

        public List<Sprite> Children { get; set; }

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

        protected SpriteEffects SpriteEffects
        {
            get
            {
                return _spriteEffects;
            }
            set
            {
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
                if (_texture != null)
                {
                    return new Rectangle((int)(Position.X), (int)(Position.Y), (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));
                }

                else if (_animationManager != null && _animationManager.CurrentAnimation != null)
                {
                    var animation = _animationManager.CurrentAnimation;

                    return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, (int)(animation.FrameWidth * _scale), (int)(animation.FrameHeight * _scale));
                }

                throw new Exception("Unknown sprite");
            }
        }

        public Rectangle ScaledRectangle
        {
            get
            {
                if (_texture != null)
                {
                    return new Rectangle((int)(ScaledPosition.X), (int)(ScaledPosition.Y), (int)(_texture.Width * FullScale), (int)(_texture.Height * FullScale));
                }

                else if (_animationManager != null && _animationManager.CurrentAnimation != null)
                {
                    var animation = _animationManager.CurrentAnimation;

                    return new Rectangle((int)ScaledPosition.X - (int)Origin.X, (int)ScaledPosition.Y - (int)Origin.Y, (int)(animation.FrameWidth * FullScale), (int)(animation.FrameHeight * FullScale));
                }

                throw new Exception("Unknown sprite");
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
        protected float _speed = 0.1f;

        public SpriteEffects Effects
        {
            get { return _spriteEffects; }
        }

        #endregion

        #region Methods
        public Sprite(Texture2D texture, Game1 game)
        {
            SetValues(texture, game);
        }

        private void SetValues(Texture2D texture, Game1 game)
        {
            _texture = texture;

            Children = new List<Sprite>();

            Origin = Vector2.Zero;

            Colour = Color.White;

            Scale = 1f;

            _game = game;

            Reset();
        }

        public Sprite(Models.TextureCollection textureCollection, Game1 game)
        {
            _textures = textureCollection;
            if (_textures.Statics.ContainsKey("Standing"))
                SetValues(textureCollection.Statics["Standing"], game);
                
        }

        public Sprite(Dictionary<string, Animation> animations, Game1 game)
        {
            _texture = null;

            Children = new List<Sprite>();

            Colour = Color.White;

            _animations = animations;

            var animation = _animations.FirstOrDefault().Value;

            _animationManager = new AnimationManager();

            Origin = Vector2.Zero;

            Scale = 1f;

            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationManager != null)
                _animationManager.Update(gameTime);

            _dTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> collideableSprites = null)
        {
            Update(gameTime);
            DoPhysics(surfaces, collideableSprites);
            _debugRectangle.Position = ScaledPosition;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null && _animationManager.IsPlaying == true)
                _animationManager.Draw(spriteBatch);
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, SpriteEffects, Layer);

            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        #region Sprite Collision
        protected bool IsTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right + this.Velocity.X > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        protected bool IsTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left + this.Velocity.X < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        protected bool IsTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        protected bool IsTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top + this.Velocity.Y < sprite.Rectangle.Bottom &&
                this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }
        #endregion

        #region Rectangle Collision
        protected bool IsTouchingLeft(Rectangle rect)
        {
            return this.Rectangle.Right + this.Velocity.X > rect.Left &&
                this.Rectangle.Left < rect.Left &&
                this.Rectangle.Bottom > rect.Top &&
                this.Rectangle.Top < rect.Bottom;
        }

        protected bool IsTouchingRight(Rectangle rect)
        {
            return this.Rectangle.Left + this.Velocity.X < rect.Right &&
                this.Rectangle.Right > rect.Right &&
                this.Rectangle.Bottom > rect.Top &&
                this.Rectangle.Top < rect.Bottom;
        }

        protected bool IsTouchingTop(Rectangle rect)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > rect.Top &&
                this.Rectangle.Top < rect.Top &&
                this.Rectangle.Right > rect.Left &&
                this.Rectangle.Left < rect.Right;
        }

        protected bool IsTouchingBottom(Rectangle rect)
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

        protected float SimulateGravity(float gravity, int resistance, int mass)
        {
            if (_terminalVelocity <= gravity)
                return gravity;
            else if (resistance == 0)
                gravity += g;
            else
                //f = ma
                gravity += ((mass * (g * 10)) - resistance) / (float)mass;
            return gravity;

        }

        protected virtual void DoPhysics(List<Rectangle> surfaces, List<Sprite> sprites = null)
        {
            Velocity = new Vector2(0, 0);
            var inFreefall = true;
            var toTruncate = false;

            if (!_inKnockback)
                HandleMovements(ref inFreefall);

            Gravity += SimulateGravity(g, 0, 60);
            Velocity += new Vector2(0, Gravity);

            foreach (var surface in surfaces)
            {
                if ((Velocity.X > 0 && IsTouchingLeft(surface)) ||
                     (Velocity.X < 0 && IsTouchingRight(surface)))
                    Velocity.X = 0;
                if ((Velocity.Y > 0 && IsTouchingTop(surface)) ||
                     (Velocity.Y < 0 && IsTouchingBottom(surface)))
                {
                    if (Gravity > 0)
                    {//snaps them to the ground so that you velocity doesn't suddenly decrease when close to ground
                        Velocity.Y = surface.Top - Rectangle.Bottom;
                        toTruncate = true;
                    }
                    else
                    {
                        Velocity.Y = 0;
                    }
                    inFreefall = false;
                    Gravity = 0;
                    break;
                }
            }

            if (sprites != null)
            {
                foreach (var sprite in sprites)
                {
                    if (Velocity.X > 0 && IsTouchingLeft(sprite))
                    {
                        StartKnocback("left");
                    }
                    else if (Velocity.X < 0 && IsTouchingRight(sprite))
                    {
                        StartKnocback("right");
                    }
                    if (Velocity.Y > 0 && IsTouchingTop(sprite))
                    {
                        StartKnocback("up");
                    }
                    else if (Velocity.Y < 0 && IsTouchingBottom(sprite))
                    {
                        StartKnocback("down");
                    }
                }
            }

            Knockback(ref Velocity);

            Position += Velocity;
            if (toTruncate) //this snaps the player to the top of a block when it falls
                Position = new Vector2(Position.X, (int)Position.Y);
        }

        protected void StartKnocback(string direction)
        {
            switch (direction)
            {
                case "up":
                    _knockbackVelocity = new Vector2(0, -_knockbackInitialVelocity); break;
                case "down":
                    _knockbackVelocity = new Vector2(0, _knockbackInitialVelocity); break;
                case "left":
                    _knockbackVelocity = new Vector2(1, -_knockbackInitialVelocity); break;
                case "right":
                    _knockbackVelocity = new Vector2(1, _knockbackInitialVelocity); break;
                default:
                    return;
            }

            _knockbackDirection = direction;
            _inKnockback = true;
            _knockbackDuration = 1500f; //in milliseconds
            _knockbackTimer = 0f;

        }

        protected void Knockback(ref Vector2 velocity)
        {
            if (_inKnockback)
            {
                velocity = _knockbackVelocity;
                _knockbackTimer += (float)_dTime;
                if (_knockbackTimer >= _knockbackDuration)
                {
                    _inKnockback = false;
                    return;
                }

                //TODO: write this more concisely
                switch (_knockbackDirection)
                {
                    case "up":
                        _knockbackVelocity = new Vector2(velocity.X, _knockbackVelocity.Y - _knockbackAcceleration * (float)_dTime);
                        if (_knockbackVelocity.Y >= 0)
                            _inKnockback = false;
                        break;
                    case "down":
                        _knockbackVelocity = new Vector2(velocity.X, _knockbackVelocity.Y + _knockbackAcceleration * (float)_dTime);
                        if (_knockbackVelocity.Y <= 0)
                            _inKnockback = false;
                        break;
                    case "left":
                        _knockbackVelocity = new Vector2(_knockbackVelocity.X - _knockbackAcceleration * (float)_dTime, velocity.Y);
                        if (_knockbackVelocity.Y >= 0)
                            _inKnockback = false;
                        break;
                    case "right":
                        _knockbackVelocity = new Vector2(_knockbackVelocity.X + _knockbackAcceleration * (float)_dTime, velocity.Y);
                        if (_knockbackVelocity.Y <= 0)
                            _inKnockback = false;
                        break;
                }

                
            }
        }


        protected virtual void HandleMovements(ref bool inFreefall)
        {

        }

        //Runs when each state is reset due to a change in resolution
        public void Reset()
        {
            _debugRectangle = new DebugRectangle
            (
                new Rectangle((int)ScaledPosition.X, (int)ScaledPosition.Y, (int)(_texture.Width), (int)(_texture.Height))
                , _game.GraphicsDevice
                , Layer + 0.01f,
                FullScale
            );
            if (_animationManager != null)
                _animationManager.Scale = FullScale;
        }

        #endregion
    }
}

