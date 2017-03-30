using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Map;
using Game.Units;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public abstract class UnitBase
    {
        private Commander commander;

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
        public abstract Vector2 DrawSize { get; }

        public abstract int UnitCount { get; }
        public abstract IEnumerable<MeshDrawableUnit> AllUnits { get; }

        public Commander Commander
        {
            get { return commander; }
            set
            {
                if (this is MeshDrawableUnit)
                    throw new ArgumentException("Single unit cannot have a commander");
                commander = value;
            }
        }

        public abstract void Draw();
    }
}