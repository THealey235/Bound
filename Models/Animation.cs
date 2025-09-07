using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bound.Models
{
    public class Animation : ICloneable
    {
        public int CurrentFrame { get; set; }

        public int FrameCount { get; private set; }

        public int FrameHeight { get { return Texture.Height; } }

        public float FrameSpeed { get; set; }

        public int FrameWidth { get { return Texture.Width / FrameCount; } }

        public bool IsLooping { get; set; }

        public Texture2D Texture { get; private set; }

        public Animation(Texture2D texture, int frameCount, float frameSpeed = 0.2f)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = frameSpeed;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
