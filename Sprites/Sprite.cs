using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Bound.Models;
using Bound.Managers;

namespace Bound.Sprites
{
    //Currently unused as i do not have any entities such as enemies or players in my game
    public class Sprite : Component, ICloneable
    {
        #region Fields
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
        protected float _terminalVelocity = 2;

        protected float Gravity = 0;

        protected string _name;

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

                else if (_animationManager != null)
                {
                    var animation = _animations.FirstOrDefault().Value;

                    return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, animation.FrameWidth, animation.FrameHeight);
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

        public readonly Color[] TextureData;

        public Sprite Parent;

        public SpriteEffects Effects
        {
            get { return _spriteEffects; }
        }

        #endregion

        #region Methods
        public Sprite(Texture2D texture, Game1 game)
        {
            _texture = texture;

            Children = new List<Sprite>();

            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            Colour = Color.White;

            TextureData = new Color[_texture.Width * _texture.Height];
            _texture.GetData(TextureData);

            Scale = 1f;

            _game = game;
        }

        public Sprite(Dictionary<string, Animation> animations, Game1 game)
        {
            _texture = null;

            Children = new List<Sprite>();

            Colour = Color.White;

            TextureData = null;

            _animations = animations;

            var animation = _animations.FirstOrDefault().Value;

            _animationManager = new AnimationManager();

            Origin = new Vector2(animation.FrameWidth / 2, animation.FrameHeight / 2);

            Scale = 1f;

            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationManager != null)
                _animationManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null && _animationManager.IsPlaying == true)
                _animationManager.Draw(spriteBatch);
            else if (_texture != null)
                spriteBatch.Draw(_texture, Position, null, Colour, _rotation, Origin, FullScale, SpriteEffects, Layer);
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

        #endregion
    }
}

