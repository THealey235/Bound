using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound.Sprites.Bosses
{
    public class Dwarfroot : Boss
    {
        public Dwarfroot(Game1 game) : base(game, "Dwarfroot")
        {
            _animationManager.Play(_animations["Walking"]);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationManager.Position = new Vector2(ScaledPosition.X, ScaledPosition.Y + (1 * Game1.ResScale));
            DrawHealthBar(gameTime, spriteBatch);
            if (_animationManager != null && _animationManager.IsPlaying == true)
                _animationManager.Draw(spriteBatch);
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, Effects, Layer);

            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }
    }
}
