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
        private bool _previousPlaying;
        private AnimationManager _animationManager;
        private Dictionary<string, Animation> _animations;
        private Sprite _user;
        private Vector2 _offset = new Vector2(0, 0);
        private float _scale;

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
                _animationManager.Layer = 0.76f;
                _animationManager.Scale = Scale =  Game1.ResScale;
                _animationManager.Origin = _user.Origin;


            }
        }

        public float Scale 
        { 
            get 
            { 
                return _scale; 
            } 
            set 
            { 
                _scale = value;
                _animationManager.Scale = Scale;
            } 
        }

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

        private void SetCollisionRectangle(string key = "Use")
        {
            if (!_animations.ContainsKey(key))
                return;

            var anim = _animations[key];
            switch (_weaponType)
            {
                case WeaponTypes.Sword:
                    _collisionRectangle = new Rectangle(0, 0, anim.FrameWidth, anim.FrameHeight);
                    break;
                default:
                    _collisionRectangle = new Rectangle(0, 0, 1, 1); break;
            }

            _debugRectangle = new DebugRectangle
            (
                _collisionRectangle,
                _game.GraphicsDevice,
                _animationManager.Layer + 0.001f,
                Scale
            )
            {
                BorderColour = Color.Red
            };
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (_user != null && _animationManager.IsPlaying)
            {
                UpdateAnimation();
                CheckCollision(sprites);
            }

            _animationManager.Update(gameTime);

            //reset after the weapon has finished its use animation
            if (_previousPlaying && !_animationManager.IsPlaying)
            {
                _previousPlaying = false;
                _debugRectangle = null;
                _collisionRectangle = new Rectangle(-1, -1, 0, 0);
                _user.UnlockEffects();
                _spriteBlacklist.Clear();
            }
        }

        private void UpdateAnimation()
        {
            _animationManager.Effects = _user.Effects;

            //used to allign the animation with the leading hand
            _offset = new Vector2(0, (_user.Rectangle.Height - _animationManager.CurrentAnimation.FrameHeight) / 2);
            if (_user.Effects == SpriteEffects.FlipHorizontally)
                _offset = new Vector2(-_animationManager.CurrentAnimation.FrameWidth + 5, _offset.Y);
            else
                _offset = new Vector2(_user.Rectangle.Width - 5, _offset.Y);

            _animationManager.Position = _debugRectangle.Position = (_user.Position + _offset) * Game1.ResScale;

            _collisionRectangle.X = (int)(_user.Position.X + _offset.X);
            _collisionRectangle.Y = (int)(_user.Position.Y + _offset.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_animationManager.IsPlaying)
                _animationManager.Draw(spriteBatch);

            if (Game1.InDebug && _debugRectangle != null)
                _debugRectangle.Draw(gameTime, spriteBatch);
        }

        public override void Use()
        {
            if (_animationManager.IsPlaying)
                return;

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
            if (_animations.ContainsKey("Use"))
            {
                StartAnimation("Use");
                UpdateAnimation();
            }
        }

        private void StartAnimation(string key)
        {
            var animation = _animations[key];
            _animationManager.Play(animation);
            _previousPlaying = true;
            SetCollisionRectangle(key);
        }

        private void UseBow()
        {

        }
    }
}
