using Bound.Models.Items;
using Bound.States;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace Bound.Sprites
{
    public class Container : Sprite
    {
        private List<Item> _containedItems;
        public Container(Game1 game, string textureName, List<(string ItemName, int Count)> inventory, Vector2 position) : base(game.Textures.GetBlock("containers", textureName), game)
        {
            _spriteType = SpriteType.Container;
            _inventory = new Managers.Inventory(game, this);
            inventory.ForEach(x => _inventory.Add(x.ItemName, x.Count));
            Scale = 0.6f;

            _position = position;
            _health = textureName.ToLower() switch
            {
                "crate" => 10f,
                _ => 1f
            };

            _health = 1;
        }

        public override void Kill(Level level)
        {
            foreach (var item in _inventory.AllItems)
                _game.Player.Level.DropItem(item, _position);

            base.Kill(level);
        }

        protected override void CheckSpriteCollision(List<Sprite> sprites, List<Sprite> dealsKnockback)
        {
            ResetScaling();
            return;
        }

        protected override void Damage(float damage, bool isPhysical)
        {
            _health -= damage;
        }

        //removes line calling StartKnockback() and removes damage mitigation calculation so any held items that have a P/MDEF stat don't mitigate damage
        public override void Damage(string direction, float PATK, float MATK)
        {
            _health -= PATK + MATK;
        }
    }
}
