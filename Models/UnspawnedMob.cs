using Bound.Sprites;
using Microsoft.Xna.Framework;

using static Bound.States.Level;

namespace Bound.Models
{
    public class UnspawnedMob
    {
        public Sprite Sprite;
        TriggerType Trigger;
        Rectangle Bounds;
        private double _timeCondition; //in seconds

        public UnspawnedMob(Sprite sprite, TriggerType trigger, (Vector2 TopLeft, Vector2 BottomRight)? bounds, float timeCondition)
        {
            if (bounds == null)
            {
                if (trigger == TriggerType.Position)
                    Trigger = TriggerType.Time;
                else Trigger = trigger;

                Bounds = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                Trigger = trigger;
                var realbounds = ((Vector2 TopLeft, Vector2 BottomRight))bounds;
                Bounds = new Rectangle((int)realbounds.TopLeft.X, (int)realbounds.TopLeft.Y, (int)(realbounds.BottomRight.X - realbounds.TopLeft.X), (int)(realbounds.BottomRight.Y - realbounds.TopLeft.Y));
            }

            Sprite = sprite;
            _timeCondition = timeCondition;
        }

        public bool CheckSpawn(double timeElapsed, Player player)
        {
            switch (Trigger)
            {
                case TriggerType.Position:
                    if (Bounds.Contains((int)player.Position.X, (int)player.Position.Y))
                        Trigger = TriggerType.Time;
                    return false;
                case TriggerType.Time:
                    _timeCondition -= timeElapsed;
                    return _timeCondition <= 0;
                default:
                    return true;
            }
        }
    }
}
