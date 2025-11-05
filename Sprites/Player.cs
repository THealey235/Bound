using Bound.Managers;
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
        private Level _level;

        public Save Save;
        public int HotbarSlot = 1;

        protected override float _health
        {
            get { return _game.ActiveSave.Health; }
            set { _game.ActiveSave.Health = value;}
        }

        protected override float _mana
        {
            get { return _game.ActiveSave.Mana; }
            set { _game.ActiveSave.Mana = value; }
        }
        protected override float _stamina
        {
            get { return _game.ActiveSave.Stamina; }
            set { _game.ActiveSave.Stamina = value; }
        }

        public Dictionary<string, Models.Attribute> Attributes
        {
            get { return _attributes; }
        }

        public float Speed
        {
            get { return _speed; }
        }

        public new Vector2 ScaledPosition
        {
            get
            {
                return new Vector2(Position.X * Game1.ResScale, Position.Y * Game1.ResScale);
            }
        }

        public override Inventory Inventory
        {
            get { return Save.Inventory; }
        }

        public Level Level
        {
            get { return _level; }
            set
            {
                _level = value;
                _health = _game.ActiveSave.MaxHealth;

            }
        }

        public Player(Texture2D texture, Input Keys, Game1 game) : base(texture, game)
        {
            _name = "player";
            Save.SetPlayer(this);
            _spriteType = SpriteType.Player;
            _knockbackDamageDealtOut = 2f;
            _speed = 100f;

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
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager != null)
            {
                if (_animationManager.IsPlaying == true)
                    _animationManager.Draw(spriteBatch);
                else if (Gravity != 0)
                    spriteBatch.Draw(_animations["Walking"].Texture, ScaledPosition, new Rectangle(0, 0, _texture.Width, _texture.Height), Colour, _rotation, Origin, FullScale, Effects, Layer);
                else
                    spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, Effects, Layer);
            }
            else if (_texture != null)
                spriteBatch.Draw(_texture, ScaledPosition, null, Colour, _rotation, Origin, FullScale, Effects, Layer);

            if (Game1.InDebug)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime, List<Rectangle> surfaces, List<Sprite> sprites = null, List<Sprite> dealsKnocback = null)
        {
            base.Update(gameTime);

            DoPhysics(surfaces, sprites, dealsKnocback);

            _debugRectangle.Position = ScaledPosition;

            for (int i = 1; i < 4; i++)
            {
                if (_keys.IsPressed($"Hotbar {i}", false))
                    HotbarSlot = i;
            }

            if (Save.Health == 0)
                Kill();
            
            if (_animationManager != null)
                SetAnimation();

            Save.Update();
        }

        private void SetAnimation()
        {
            if (Velocity.X != 0 && Gravity == 0)
                _animationManager.Play(_animations["Walking"]);
            else
                _animationManager.Stop();
        }

        protected override void CheckJump(bool inFreefall)
        {
            if (!inFreefall && _keys.IsPressed("Jump", true) && !_inKnockback)
                Gravity = -4;
        }

        protected override void HandleMovements(ref bool inFreefall)
        {
            var previousEffect = Effects;
            if (_keys.IsPressed("Up", true))
                inFreefall = false;
            if (_keys.IsPressed("Down", true))
                Velocity += new Vector2(0, Speed);
            if (_keys.IsPressed("Left", true))
            {
                Velocity += new Vector2(-Speed, 0);
                Effects = SpriteEffects.FlipHorizontally;
            }
            if (_keys.IsPressed("Right", true))
            {
                Velocity += new Vector2(Speed, 0);
                Effects = SpriteEffects.None;
            }
            if (_keys.IsPressed("Reset", true))
            {
                Position = new Vector2(100, 0);
                Gravity = 0;
                Save.Health -= 5;
            }
            if (_keys.IsPressed("Use", true))
            {
                _lockEffects = Level.HUD.UseItem();
            }
        }

        public void Kill()
        {
            Save.ResetAttrs();
            Position = new Vector2(0, 0);
        }

        public void UpdateWhileStatic(GameTime gameTime)
        {
            _dTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void UpdateAttributes(int saveIndex)
        {
            _attributes = _game.SavesManager.Saves[saveIndex].Attributes;
        }

        public void RemoveItemFromHotbar(string name) => Level.HUD.RemoveFromHotbar(name);
    }
}
