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
        protected readonly List<T> Storage = new List<T>();

        private int health = -1;
        private int maxHealth = -1;
        protected DrawingSet Set;

        protected UnitGroup(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public virtual Int2 ChildrenDimensions { get; set; }

        public override int MaxHealth
        {
            get
            {
                if (maxHealth == -1)
                    maxHealth = Storage.Select(u => u.MaxHealth).Aggregate((x, y) => x + y);
                return maxHealth;
            }
        }

        public override int Health
        {
            get
            {
                if (health == -1)
                    health = Storage.Select(u => u.Health).Aggregate((x, y) => x + y);
                return health;
            }
            set
            {
                int healthDifference = health - value;
                health = value;
                foreach (T child in this)
                    if (healthDifference > 0) //Take damage
                    {
                        if (child.Health > healthDifference)
                        {
                            child.Health -= healthDifference;
                            break;
                        }
                        healthDifference -= child.Health;
                        child.Health = 0;
                        //RemoveUnit(child);
                    }
                    else //Give health
                    {
                        if (child.MaxHealth > -healthDifference)
                        {
                            child.Health -= healthDifference;
                            break;
                        }
                        healthDifference += child.Health;
                        child.Health = child.MaxHealth;
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

        public abstract Vector2 GroupSpacing { get; }

        public override int UnitCount => Storage.Count;

        public override Vector2 DrawSize => Vector2.Scale(Storage[0].DrawSize, ChildrenDimensions) + GroupSpacing;

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (T child in Storage)
                foreach (MeshDrawableUnit drawableUnit in child.AllUnits)
                    yield return drawableUnit;
            }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public IEnumerator<T> GetEnumerator()
        {
            return Storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract void Order(bool instant = false);

        public void AddUnit(T unit)
        {
            Storage.Add(unit);
            Set = Prefetch(this);
        }

        public void RemoveUnit(T unit)
        {
            Storage.Remove(unit);
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