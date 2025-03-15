using Bound.Models;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace Bound.Sprites
{
    public class Player : Sprite
    {
        private float _speed = 0.09f;
        private Input _keys;
        private double _dTime;
        private SpriteEffects _spriteEffect;
        private DebugRectangle _debugRectangle;
        private Dictionary<string, Attribute> _attributes;

        public Save SaveState;
        public int HotbarSlot = 1;
        public Dictionary<string, Attribute> Attributes
        {
            get { return _attributes; }
        }

        public float Speed
        {
            get { return (float)(_speed * _dTime); }
        }

        public Vector2 ScaledPosition
        {
            get
            {
                return new Vector2(Position.X * Game1.ResScale, Position.Y * Game1.ResScale);
            }
        }


        public Player(Texture2D texture, Input Keys, Game1 game) : base(texture, game)
        {
            _keys = Keys;
            _scale = 0.8f;
            _debugRectangle = new DebugRectangle
            (
                new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height)
                , game.GraphicsDevice
                , Layer + 0.01f,
                Scale
            );
            SaveState = _game.SavesManager.Saves[_game.SaveIndex];
            
            if (SaveState != null)
                _attributes = SaveState.Attributes;
        }

        public Player(Dictionary<string, Animation> animations, Input Keys, Game1 game) : base(animations, game)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Vector2.Zero, Game1.ResScale * _scale, _spriteEffect, Layer);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);

            if (Game1.InDebug)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime, List<Rectangle> surfaces)
        {
            base.Update(gameTime);
            _dTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            _keys.Update();

            DoPhysics(surfaces);

            for (int i = 1; i < 4; i++)
            {
                if (_keys.IsPressed($"Hotbar {i}", false))
                    HotbarSlot = i;
            }
        }

        private void DoPhysics(List<Rectangle> surfaces)
        {
            Velocity = new Vector2(0, 0);
            var inFreefall = true;

            HandleKeys(ref inFreefall);

            if (inFreefall)
            {
                gravity += SimulateGravity(g, 0, 60);
                Velocity += new Vector2(0, gravity);
            }

            foreach (var surface in surfaces)
            {
                if ((Velocity.X > 0 && IsTouchingLeft(surface)) ||
                     (Velocity.X < 0 && IsTouchingRight(surface)))
                    Velocity.X = 0;
                if ((Velocity.Y > 0 && IsTouchingTop(surface)) ||
                     (Velocity.Y < 0 && IsTouchingBottom(surface)))
                {
                    if (gravity > 0)
                    {//snaps them to the ground so that you velocity doesn't suddenly decrease when close to ground
                        Velocity.Y = surface.Top - Rectangle.Bottom;
                    }
                    else
                    {
                        Velocity.Y = 0;
                    }
                    inFreefall = false;
                    gravity = 0;
                    break;
                }
            }


            if (!inFreefall && _keys.IsPressed("Jump", true))
            {
                gravity = -4;
            }

            Position += Velocity;
            if (_animationManager != null)
                _animationManager.Position = ScaledPosition;

            _debugRectangle.Position = ScaledPosition;
        }

        private bool HandleKeys(ref bool inFreefall)
        {
            if (_keys.IsPressed("Up", true))
                inFreefall = false;
            //if (_keys.IsPressed("Down", true))
            //    Velocity += new Vector2(0, Speed);
            if (_keys.IsPressed("Left", true))
            {
                Velocity += new Vector2(-Speed, 0);
                _spriteEffect = SpriteEffects.None;
            }
            if (_keys.IsPressed("Right", true))
            {
                Velocity += new Vector2(Speed, 0);
                _spriteEffect = SpriteEffects.FlipHorizontally;
            }
            if (_keys.IsPressed("Reset", true))
            {
                Position = new Vector2(100, 0);
                gravity = 0;
            }

            return inFreefall;
        }

        
        public void Reset()
        {
            _debugRectangle = new DebugRectangle
            (
                new Rectangle((int)ScaledPosition.X, (int)ScaledPosition.Y, (int)(_texture.Width), (int)(_texture.Height))
                , _game.GraphicsDevice
                , Layer + 0.01f,
                Scale
            );
        }

        public void UpdateWhileStatic(GameTime gameTime)
        {
            _keys.Update();
            _dTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void UpdateAttributes(int saveIndex)
        {
            _attributes = _game.SavesManager.Saves[saveIndex].Attributes;
        }
    }
}
