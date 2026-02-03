using Bound.Models;
using Bound.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static Bound.Managers.TextureManager;

namespace Bound.Sprites
{
    public class Mob : Sprite
    {
        protected int _exp;

        public Mob(Models.TextureCollection textures, Game1 game, MobInfo info) : base(textures, game)
        {
            _spriteType = SpriteType.Mob;
            _textures = textures;
            SetAnimations(textures);
            SetMobInfo(info);
        }

        public Mob(Game1 game, string name)
        {
            _spriteType = SpriteType.Mob;
            MobInfo info;

            if (game.Textures.Sprites.TryGetValue(name, out _textures) && game.Textures.GetMobInfo(name, out info))
            {
                SetValues(_textures.Statics["Idle"], game);
                SetAnimations(_textures);
                SetMobInfo(info);
            }
        }

        private void SetAnimations(Models.TextureCollection textures)
        {
            _animationManager = new Managers.AnimationManager();
            _animations = _textures.Sheets.Select(x => (x.Key, new Animation(x.Value, _textures.FrameCount(x.Key), _textures.FrameSpeed(x.Key)))).ToDictionary(x => x.Key, x => x.Item2);
            if (_animations.ContainsKey("Moving"))
                _animationManager.Play(_animations["Moving"]);
        }

        private void SetMobInfo(MobInfo info)
        {
            _speed = info.Speed;
            _health = info.Health;
            _stamina = info.Stamina;
            _mana = info.Mana;
            _knockbackDamageDealtOut = info.KNBKDmg;
            _knockbackInitialVelocity = info.KNBKVelocity;
            _exp = info.EXP;
            Scale = info.Scale;
        }


        protected override void HandleMovements(ref bool inFreefall)
        {
            var distance = (_game.Player.Position - Position).Length();

            if ((int)_game.Player.Position.X < (int)Position.X)
            {
                Velocity -= new Vector2(_speed, 0);
                Effects = SpriteEffects.FlipHorizontally;
            }
            else if ((int)_game.Player.Position.X > (int)Position.X)
            {
                Velocity += new Vector2(_speed, 0);
                Effects = SpriteEffects.None;
            }
        }

        public override void Kill(Level level)
        {
            base.Kill(level);
            _game.ActiveSave.EXP += _exp;
        }
    }
}
