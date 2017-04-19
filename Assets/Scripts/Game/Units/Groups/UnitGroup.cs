using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;


namespace Assets.Scripts.Game.Units.Groups
{
    public abstract class UnitGroup<T> : UnitBase, IEnumerable<T> where T : UnitBase
    {
        protected readonly List<T> storage = new List<T>();
        protected DrawingSet Set;

        protected UnitGroup(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public virtual Int2 ChildrenDimensions { get; set; }

        private int health = -1;

        public override int Health
        {
            get
            {
                if (health == -1)
                {
                    health = storage.Select(u => u.Health).Aggregate((x, y) => x + y);
                }
                return health;
            }
            set
            {
                int healthDifference = health - value;
                health = value;
                foreach (T child in this)
                {
                    if (healthDifference > 0)
                    {
                        //give damage
                        if (child.Health > healthDifference)
                        {
                            child.Health -= healthDifference;
                            break;
                        }
                        healthDifference -= child.Health;
                        child.Health = 0;
                    }
                    else
                    {
                        //give health

                    }
                }
            }
        }

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (T child in this)
                    child.Rotation = value;
            }
        }

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Order();
            }
        }


        public override int UnitCount => storage.Count;

        public override Vector2 DrawSize => Vector2.Scale(storage[0].DrawSize, ChildrenDimensions);

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (T child in storage)
                foreach (MeshDrawableUnit drawableUnit in child.AllUnits)
                    yield return drawableUnit;
            }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public IEnumerator<T> GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract void Order(bool instant = false);

        public void AddUnit(T unit)
        {
            storage.Add(unit);
            Set = Prefetch(this);
        }

        public void RemoveUnit(T unit)
        {
            storage.Remove(unit);
            Set = Prefetch(this);
        }

        public override void SetPositionInstant(Vector3 pos)
        {
            base.Position = pos;
            Order(true);
        }

        public override void Draw()
        {
            if (Set != null)
                DrawAll(Set);
        }
    }
}