using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models.Items
{
    public class Weapon : Item
    {
        public static bool InUse = false;

        private Game1 _game;
        private AnimationManager _animationManager;
        private Dictionary<string, Animation> _animations;
        private Sprite _user;
        private Vector2 _offset = new Vector2(0, 0);

        public enum WeaponTypes
        {
            Sword, Bow, Unrecognised
        }

        public override Sprite User
        {
            get { return _user; }
            set 
            { 
                _user = value;
                _animationManager.Layer = 0.99f;
                _animationManager.Scale = Scale = _user.FullScale * 1.5f;
                _animationManager.Origin = _user.Origin;

                SetCollisionRectangle();
            }
        }

        public float Scale { get; set; }

        private readonly WeaponTypes _weaponType;

        public Weapon(Game1 game, TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string attributes = "") : base(textures, id, name, description, type, attributes)
        {
            name = name.ToLower();
            if (name.Contains("sword") || name.Contains("axe"))
                _weaponType = WeaponTypes.Sword;
            else if (name.Contains("bow"))
                _weaponType = WeaponTypes.Bow;
            else 
                _weaponType = WeaponTypes.Unrecognised;

            _animations = _textures.Sheets
                .Select(x => new KeyValuePair<string, Animation>(x.Key, new Animation(x.Value, _textures.FrameCount(x.Key), _textures.FrameSpeed(x.Key))))
                .ToDictionary(x => x.Key, x => x.Value);
            _animationManager = new AnimationManager()
            {
                Loop = false,
            };

            _game = game;
        }

        private void SetCollisionRectangle()
        {
            if (!_animations.ContainsKey("Use"))
                return;

            var anim = _animations["Use"];
            switch (_weaponType)
            {
                case WeaponTypes.Sword:
                    _rectangle = new Rectangle(0, 0, (int)(anim.FrameWidth * Scale), (int)(anim.FrameHeight * Scale));
                    break;
                default:
                    _rectangle = new Rectangle(0, 0, 1, 1); break;
            }

            _collisionRectangle = new DebugRectangle
            (
                _rectangle,
                _game.GraphicsDevice,
                _animationManager.Layer + 0.001f,
                1f
            )
            {
                BorderColour = Color.Red,
                Position = new Vector2(-100, -100)
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (_user != null && _animationManager.CurrentAnimation != null)
            {
                _animationManager.Effects = _user.Effects;
                _offset = new Vector2(_user.Rectangle.Width * 0.8f, -_user.Rectangle.Height * 0.3f); //used to allign the animation with the leading hand
                if (_user.Effects == SpriteEffects.FlipHorizontally)
                    _offset = new Vector2(-_offset.X - (_animationManager.CurrentAnimation.FrameWidth * (Scale / _user.FullScale)) / 2, _offset.Y);
                _animationManager.Position = _user.ScaledPosition + _offset * _user.FullScale;

                _rectangle.X = (int)_user.Position.X;
                _rectangle.Y = (int)_user.Position.Y;

                _collisionRectangle.Position = _user.ScaledPosition + _offset * _user.FullScale;
            }
            if (!_animationManager.IsPlaying)
            {
                _collisionRectangle.Position = new Vector2(-100, -100);
                _rectangle.X = -100;
                _rectangle.Y = -100;
            }

            _animationManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager.IsPlaying)
                _animationManager.Draw(spriteBatch);

            if (Game1.InDebug && _collisionRectangle != null)
                _collisionRectangle.Draw(gameTime, spriteBatch);
        }

        public override void Use()
        {
            switch (_weaponType)
            {
                case WeaponTypes.Sword:
                    UseSword(); break;
                case WeaponTypes.Bow:
                    UseBow(); break;
                default:
                    break;
            }
        }

        private void UseSword()
        {
            if (_animations.ContainsKey("Use") && !_animationManager.IsPlaying)
                _animationManager.Play(_animations["Use"]);
        }

        private void UseBow()
        {

        }
    }
}
