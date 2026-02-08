using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Bound.Models.Items;
using System;

namespace Bound.Sprites.Bosses
{
    public class Dwarfroot : Boss
    {
        public Dwarfroot(Game1 game) : base(game, "Dwarfroot")
        {
            _animationManager.Play(_animations["Walking"]);
            _atRest = new ActionInfo(0, Vector2.Zero, new Rectangle(0, 0, 0, 0), new Rectangle(24, 21, 51, 74), (0, 0));
            _weapon = new Item(new Models.TextureCollection(), -1, "Dwarfroot Hammer", "None", Managers.TextureManager.ItemType.Weapon, "PATK 20, MATK 5");
            _weapon.Owner = this;

            _actions = new Dictionary<string, ActionInfo>()
            {
                { "Clobber", new ActionInfo(90 * Scale, new Vector2(22, 0), new Rectangle(64, 40, 75, 50), new Rectangle(45, 21, 40, 74), (12, 16))},
            };

            _currentAction = _atRest;

            _debugRectangle = new Models.DebugRectangle(Rectangle, _game.GraphicsDevice, _game.Player.Layer + 0.01f, Game1.ResScale);

            ResetFullTextureDebugRectangle();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        protected override void HandleMovements(ref bool inFreefall)
        {
            var distance = Math.Abs(((_game.Player.TextureCenter.X * Scale) + _game.Player.Position.X) - (Position.X + (Scale * (_currentAction.BossHitBox.X + (_currentAction.BossHitBox.Width / 2)))));

            if (_canAttack)
            {
                foreach (var action in _actions)
                {
                    if (distance <= action.Value.Range)
                    {
                        ChangeAction(action);
                        return;
                    }
                }

                _currentAction = _atRest;
            }

            if (_currentAction != _atRest)
                return;

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

            _animationManager.Loop = true;
        }
    }
}
