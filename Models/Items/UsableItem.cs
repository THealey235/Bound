using Bound.Managers;
using Bound.Sprites;
using Microsoft.Xna.Framework;

namespace Bound.Models.Items
{
    public abstract class UsableItem : Item
    {
        public bool InUse = false;

        protected Vector2 _offset = new Vector2(0, 0);
        protected Game1 _game;
        protected bool _previousPlaying;
        protected float _scale;
        protected float _layer = 0.76f;

        public override Sprite Owner
        {
            get { return _owner; }
            set
            {
                if (value == null)
                    return;
                _spriteBlacklist.Remove(_owner);
                _owner = value;
                _spriteBlacklist.Add(_owner);
            }
        }

        public virtual float Scale
        {
            get { return _scale; }
            set { }
        }

        protected UsableItem(Game1 game, TextureCollection textures, int id, string name, string description, TextureManager.ItemType type, string attributes = "") : base(textures, id, name, description, type, attributes)
        {
            _game = game;
        }

        public override abstract Item Clone();
    }
}
