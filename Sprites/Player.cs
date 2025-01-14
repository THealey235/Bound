using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Bound.Sprites
{
    public class Player : Sprite
    {
        private float _speed = 0.09f;
        private Input _keys;
        private double dTime;

        public float Speed
        {
            get { return (float)(_speed * dTime); }
        }

        public Vector2 ScaledPosition
        {
            get
            {
                return new Vector2(Position.X * Game1.ResScale, Position.Y * Game1.ResScale);
            }
        }


        public Player(Texture2D texture, Input Keys) : base(texture)
        {
            _keys = Keys;
            _scale = 0.8f;
        }

        public Player(Dictionary<string, Animation> animations, Input Keys) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            dTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            base.Update(gameTime);
            _keys.Update();
            var inFreefall = true;
            var Velocity = new Vector2(0, 0);

            if (_keys.IsPressed("Up", true))
                inFreefall = false;
            //if (_keys.IsPressed("Down", true))
            //    Velocity += new Vector2(0, Speed);
            if (_keys.IsPressed("Left", true))
                Velocity += new Vector2(-Speed, 0);
            if (_keys.IsPressed("Right", true))
                Velocity += new Vector2(Speed, 0);
            if (_keys.IsPressed("Reset", true)) 
            { 
                Position = new Vector2(100, 0);
                gravity = 0;
            }

            if (inFreefall)
            {
                gravity += SimulateGravity(g, 0, 60);
                Velocity += new Vector2(0, gravity);
            }

            Position += Velocity;
            if (_animationManager != null)
                _animationManager.Position = ScaledPosition;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, Game1.ResScale * _scale, SpriteEffects.None, Layer);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
        }
        
    }
}
