using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public abstract class UnitBase
    {
        private Commander commander;

        private Rect hitbox;

        protected UnitBase()
        {
            WalkSpeed = DefaultSpeed;
        }

        public abstract float DefaultSpeed { get; }
        public virtual Vector3 Position { get; set; } = Vector3.zero;

        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        public virtual IFormation Formation { get; set; }
        public virtual float WalkSpeed { get; set; } = 1.0f;

        public virtual Int2 ChildrenDimensions { get; set; }
        public abstract Vector2 DrawSize { get; }

        public Rect Hitbox
        {
            get
            {
                hitbox.x = Position.x - DrawSize.x / 2;
                hitbox.y = Position.y - DrawSize.y / 2;
                hitbox.width = DrawSize.x;
                hitbox.height = DrawSize.y;
                return hitbox;
            }
        }

        public abstract int UnitCount { get; }
        public abstract IEnumerable<MeshDrawableUnit> AllUnits { get; }

        public abstract int Health { get; set; }

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

        public abstract string UnitName { get; }

        public string Info => "This is " + UnitName + "\nHealth: " + Health / 2f + "%";
    }
}