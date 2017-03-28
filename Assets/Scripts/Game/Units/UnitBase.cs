using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Map;
using Game.Units.Formation;
using UnityEngine;

namespace Game.Units
{
    public abstract class UnitBase
    {
        protected UnitBase()
        {
            WalkSpeed = DefaultSpeed;
        }

        public abstract float DefaultSpeed { get; }
        public virtual Vector3 Position { get; set; } = Vector3.zero;

        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        public virtual IFormation Formation { get; set; } = new VerticalLineFormation();
        public virtual float WalkSpeed { get; set; } = 1.0f;

        public virtual Int2 ChildrenDimensions { get; set; }

        public abstract int UnitCount { get; }

        public abstract void Draw();
    }
}