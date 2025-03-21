﻿using Microsoft.Xna.Framework.Graphics;
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

        protected float _scale { get; set; }

        protected Texture2D _texture;

        protected float g = 0.09f;
        protected float _terminalVelocity = 2;

        protected float gravity = 0;

        #endregion

        #region Properties
        public Vector2 Velocity;

        public List<Sprite> Children { get; set; }

        public Color Colour { get; set; }

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

        public float Scale
        {
            get
            {
                return _scale * Game1.ResScale;
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;

                if (_animationManager != null)
                    _animationManager.Origin = _origin;
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
                    _animationManager.Position = _position;
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

                if (_animationManager != null)
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

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                  Matrix.CreateRotationZ(_rotation) *
                  Matrix.CreateTranslation(new Vector3(Position, 0));
            }
        }

        public Sprite Parent;

        /// The area of the sprite that could "potentially" be collided with
        public Rectangle CollisionArea
        {
            get
            {
                return new Rectangle(Rectangle.X, Rectangle.Y, MathHelper.Max(Rectangle.Width, Rectangle.Height), MathHelper.Max(Rectangle.Width, Rectangle.Height));
            }
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

            _scale = 1f;

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

            _animationManager = new AnimationManager(animation);

            Origin = new Vector2(animation.FrameWidth / 2, animation.FrameHeight / 2);

            _scale = 1f;

            _game = game;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, null, Colour, _rotation, Origin, Game1.ResScale * Scale, SpriteEffects.None, Layer);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
        }

        #region Collision
        //per pixel collision detection is not implemented for animated sprites, not my code
        //this will more than likey be never be used
        public bool Intersects(Sprite sprite)
        {
            if (this.TextureData == null)
                return false;

            if (sprite.TextureData == null)
                return false;

            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            var transformAToB = this.Transform * Matrix.Invert(sprite.Transform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < this.Rectangle.Height; yA++)
            {
                // Start at the beginning of the row
                var posInB = yPosInB;

                for (int xA = 0; xA < this.Rectangle.Width; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < sprite.Rectangle.Width &&
                        0 <= yB && yB < sprite.Rectangle.Height)
                    {
                        // Get the colors of the overlapping pixels
                        var colourA = this.TextureData[xA + yA * this.Rectangle.Width];
                        var colourB = sprite.TextureData[xB + yB * sprite.Rectangle.Width];

                        // If both pixel are not completely transparent
                        if (colourA.A != 0 && colourB.A != 0)
                        {
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        //public virtual void OnCollide(Sprite sprite)
        //{

        //}

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

