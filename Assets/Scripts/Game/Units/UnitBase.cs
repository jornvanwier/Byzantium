using Assets.Game.Units.Formation;
using UnityEngine;

namespace Assets.Game.Units
{
    public abstract class UnitBase
    {
        protected const float DefaultSpeed = 1;
        public virtual Vector3 Position { get; set; } = Vector3.zero;

        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        public virtual IFormation Formation { get; set; } = new VerticalLineFormation();
        public virtual float WalkSpeed { get; set; } = DefaultSpeed;

        public abstract void Draw();
    }
}