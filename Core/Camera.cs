using Bound.Sprites;
using Microsoft.Xna.Framework;
using Bound;

namespace CameraFollowingSprite.Core
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public Vector2 V2Transform { get; private set; }

        public void Follow(Sprite target)
        {
            var offset = Matrix.CreateTranslation(    //to center it around the center of the window not the top left
                            Game1.ScreenWidth / 2,
                            Game1.ScreenHeight / 2,
                            0);

            var position = Matrix.CreateTranslation(
                            -target.ScaledPosition.X - (target.Rectangle.Width / 2),
                            -target.ScaledPosition.Y - (target.Rectangle.Height / 2),
                            0);


            Transform = position * offset;
            V2Transform = new Vector2( -(Transform.M41), -(Transform.M42) );
            Game1.V2Transform = V2Transform;
        }
    }
}
