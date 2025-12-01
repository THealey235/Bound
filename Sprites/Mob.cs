using Bound.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static Bound.Managers.TextureManager;

namespace Bound.Sprites
{
    public class Mob : Sprite
    {
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

        private static void SetAnimations(TextureCollection textures)
        {
            if (textures.Sheets.Count > 0)
            {
                //set animationManager and animations
            }
        }

        private void SetMobInfo(MobInfo info)
        {
            _speed = info.Speed;
            _health = info.Health;
            _stamina = info.Stamina;
            _mana = info.Mana;
            _knockbackDamageDealtOut = info.KNBKDmg;
        }


        protected override void HandleMovements(ref bool inFreefall)
        {
            var distance = (_game.Player.Position - Position).Length();

            if ((int)_game.Player.Position.X < (int)Position.X)
                Velocity -= new Vector2(_speed, 0);
            else if ((int)_game.Player.Position.X > (int)Position.X)
                Velocity += new Vector2(_speed, 0);
        }
    }
}
