using Assets.Scripts.Game.Units.Formation;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public abstract class UnitBase
    {
        protected UnitBase()
        {
            WalkSpeed = DefaultSpeed;
        }

        public virtual float DefaultSpeed { get; }
        public virtual Vector3 Position { get; set; } = Vector3.zero;

        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        public virtual IFormation Formation { get; set; } = new VerticalLineFormation();
        public virtual float WalkSpeed { get; set; } = 1.0f;

        public abstract int UnitCount { get; }

        public abstract void Draw();
    }
}