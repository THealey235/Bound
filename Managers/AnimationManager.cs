using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Bound.Models;

namespace Bound.Managers
{
    public class AnimationManager : ICloneable
    {
        private Animation _animation;

        private float _timer;

        public bool IsPlaying { get; private set; }

        public Animation CurrentAnimation
        {
            get
            {
                return _animation;
            }
        }

        public float Layer { get; set; }

        public Color Colour { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public SpriteEffects Effects { get; set; }

        public float Scale { get; set; }

        public bool Loop { get; set; }

        #region Methods
      
        public AnimationManager()
        {
            IsPlaying = false;
            Colour = Color.White;
            Scale = 1f;
            Loop = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _animation.Texture,
                Position,
                new Rectangle
                (
                    _animation.CurrentFrame * _animation.FrameWidth,
                    0,
                    _animation.FrameWidth,
                    _animation.FrameHeight
                ),
                Colour,
                Rotation,
                Origin,
                Scale,
                Effects,
                Layer
            );
        }

        public void Play(Animation animation)
        {
            if (_animation == animation && IsPlaying)
                return;

            _animation = animation;

            _animation.CurrentFrame = 0;

            _timer = 0;

            IsPlaying = true;
        }

        public void Stop()
        {
            _timer = 0f;
            
            if (_animation != null)
                _animation.CurrentFrame = 0;

            IsPlaying = false;
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying == false)
                return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                {
                    _animation.CurrentFrame = 0;
                    IsPlaying = Loop;
                }
            }
        }

        public object Clone()
        {
            var animationManager = this.MemberwiseClone() as AnimationManager;

            animationManager._animation = animationManager._animation.Clone() as Animation;

            return animationManager;
        }

        #endregion
    }
}
