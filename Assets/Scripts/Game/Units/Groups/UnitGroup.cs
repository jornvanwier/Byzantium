using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;


namespace Assets.Scripts.Game.Units.Groups
{
    public abstract class UnitGroup<T> : UnitBase, IEnumerable<T> where T : UnitBase
    {
        private readonly List<T> storage = new List<T>();
        protected DrawingSet Set;
        public virtual Int2 ChildrenDimensions { get; set; }

        protected UnitGroup(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override int Health
        {
            get { return storage[0].Health; }
            set
            {
                foreach (T child in this)
                    child.Health = value;
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
        public void AddUnit<TOther>(TOther unit) where TOther:UnitBase
        {
            T child = storage.PickRandom();
            
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