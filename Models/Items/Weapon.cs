using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models.Items
{
    public class Weapon : Item
    {
        public static bool InUse = false;

        private AnimationManager _animationManager;
        private Dictionary<string, Animation> _animations;
        private Rectangle _collisionRectangle;
        private Sprite _user;

        public enum WeaponTypes
        {
            Sword, Bow, Unrecognised
        }

        public Rectangle CollisionRectangle
        {
            get { return _collisionRectangle; }
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
            }
        }

        public float Scale { get; set; }

        private readonly WeaponTypes _weaponType;

        public Weapon(TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string attributes = "") : base(textures, id, name, description, type, attributes)
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

            SetCollisionRectangle();
        }

        private void SetCollisionRectangle()
        {
            if (!_animations.ContainsKey("Use"))
                return;

            var anim = _animations["Use"];
            switch (_weaponType)
            {
                case WeaponTypes.Sword:
                    _collisionRectangle = new Rectangle(0, 0, (int)(anim.FrameWidth * Scale), (int)(anim.FrameHeight * Scale));
                    break;
                default:
                    _collisionRectangle = new Rectangle(0, 0, 1, 1); break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_user != null)
            {
                _animationManager.Effects = _user.Effects;
                Vector2 offset = new Vector2(_user.Rectangle.Width * 0.8f, -_user.Rectangle.Height * 0.3f); //used to allign the animation with the leading hand
                if (_user.Effects == SpriteEffects.FlipHorizontally)
                    offset = new Vector2(-offset.X - (_animationManager.CurrentAnimation.FrameWidth * (Scale / _user.FullScale)) / 2, offset.Y);
                _animationManager.Position = _user.ScaledPosition + offset * _user.FullScale;
            }

            _animationManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager.IsPlaying)
                _animationManager.Draw(spriteBatch);
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
