using Bound.Models;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Bound.Sprites
{
    public class Player : Sprite
    {
        private Input _keys;
        private Dictionary<string, Models.Attribute> _attributes;

        public Save Save;
        public Level Level;
        public int HotbarSlot = 1;
        public Dictionary<string, Models.Attribute> Attributes
        {
            get { return _attributes; }
        }

        public float Speed
        {
            get { return (float)(_speed * _dTime); }
        }

        public new Vector2 ScaledPosition
        {
            get
            {
                return new Vector2(Position.X * Game1.ResScale, Position.Y * Game1.ResScale);
            }
        }

        public Player(Texture2D texture, Input Keys, Game1 game) : base(texture, game)
        {
            _name = "player";

            _keys = Keys;
            Scale = 0.8f;
            _debugRectangle = new DebugRectangle
            (
                new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height)
                , game.GraphicsDevice
                , Layer + 0.01f,
                FullScale
            );

            Origin = Vector2.Zero;

            if (_game.SaveIndex != -1)
                Save = _game.SavesManager.Saves[_game.SaveIndex];
            else
                Save = null;

            if (Save != null)
                _attributes = Save.Attributes;

            if (_game.Textures.Sprites.ContainsKey("Player"))
            {
                _animations = new Dictionary<string, Animation>();
                _textures = _game.Textures.Sprites["Player"];
                foreach(var sheet in _textures.Sheets)
                {
                    _animations.Add(sheet.Key, new Animation(sheet.Value, _textures.FrameCount(sheet.Key), 0.15f));
                }

                _animationManager = new Managers.AnimationManager()
                {
                    Scale = FullScale,
                };
            }

            foreach (var item in _game.Items)
                item.Value.User = this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null)
            {
                if (_animationManager.IsPlaying == true)
                    _animationManager.Draw(spriteBatch);
                else if (Gravity != 0)
                    spriteBatch.Draw(_animations["Walking"].Texture, ScaledPosition, new Rectangle(0, 0, _texture.Width, _texture.Height), Colour, _rotation, Origin, FullScale, SpriteEffects, Layer);
                else
                    spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, SpriteEffects, Layer);
            }
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, SpriteEffects, Layer);

            if (Game1.InDebug)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> sprites = null)
        {
            if (_animationManager != null)
                _animationManager.Update(gameTime);

            _dTime = gameTime.ElapsedGameTime.TotalMilliseconds;

            DoPhysics(surfaces);

            for (int i = 1; i < 4; i++)
            {
                if (_keys.IsPressed($"Hotbar {i}", false))
                    HotbarSlot = i;
            }

            if (Save.Health == 0)
                Kill();
            
            if (_animationManager != null)
                SetAnimation();
        }

        private void SetAnimation()
        {
            if (Velocity.X != 0 && Gravity == 0)
                _animationManager.Play(_animations["Walking"]);
            else
                _animationManager.Stop();
        }

        protected override void DoPhysics(List<Rectangle> surfaces, List<Sprite> sprites = null)
        {
            Velocity = new Vector2(0, 0);
            var inFreefall = true;
            var toTruncate = false;

            HandleKeys(ref inFreefall);

            if (inFreefall)
            {
                Gravity += SimulateGravity(g, 0, 60);
                Velocity += new Vector2(0, Gravity);
            }

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

            if (!inFreefall && _keys.IsPressed("Jump", true) && !_inKnockback)
            {
                Gravity = -4;
            }

            Position += Velocity;
            if (toTruncate) //this snaps the player to the top of a block when it falls
                Position = new Vector2(Position.X, (int)Position.Y);

            _debugRectangle.Position = ScaledPosition;
        }

        private void HandleKeys(ref bool inFreefall)
        {
            var previousEffect = SpriteEffects;
            if (_keys.IsPressed("Up", true))
                inFreefall = false;
            if (_keys.IsPressed("Down", true))
                Velocity += new Vector2(0, Speed);
            if (_keys.IsPressed("Left", true))
            {
                Velocity += new Vector2(-Speed, 0);
                SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            if (_keys.IsPressed("Right", true))
            {
                Velocity += new Vector2(Speed, 0);
                SpriteEffects = SpriteEffects.None;
            }
            if (_keys.IsPressed("Reset", true))
            {
                Position = new Vector2(100, 0);
                Gravity = 0;
                Save.Health -= 5;
            }
            if (_keys.IsPressed("Use", true))
                Level.HUD.UseItem();
        }

        public void Kill()
        {
            Save.ResetAttrs();
            Position = new Vector2(0, 0);
        }

        public void UpdateWhileStatic(GameTime gameTime)
        {
            _dTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void UpdateAttributes(int saveIndex)
        {
            _attributes = _game.SavesManager.Saves[saveIndex].Attributes;
        }
    }
}
