using Bound.Models.Items;
using Bound.States;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace Bound.Sprites
{
    public class Container : Sprite
    {
        public Container(Game1 game, string textureName, List<(string ItemName, int Count)> inventory, Vector2 position) : base(game.Textures.GetBlock("containers", textureName), game)
        {
            _spriteType = SpriteType.Container;
            _inventory = new Managers.Inventory(game, this);
            inventory.ForEach(x => _inventory.Add(x.ItemName, x.Count));

            _position = position;
            _health = textureName.ToLower() switch
            {
                "crate" => 10,
                _ => 1
            };
        }

        public override void Kill(Level level)
        {
            foreach (var item in _inventory.AllItems)
                _game.Player.Level.DropItem(item, _position);

            base.Kill(level);
        }

        protected override void CheckSpriteCollision(List<Sprite> sprites, List<Sprite> dealsKnockback)
        {
            return;
        }
    }
}
