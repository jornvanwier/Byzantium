using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>, IEnumerable<Cohort>,
        IEnumerable<Cavalry>
    {
        private readonly List<Cavalry> cavalries = new List<Cavalry>();

        private readonly List<Cohort> cohorts = new List<Cohort>();
        public override float DefaultSpeed => 1.5f;
        public IEnumerable<Cavalry> Cavalries => cavalries;
        public IEnumerable<Cohort> Cohorts => cohorts;

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (UnitBase child in this)
                    child.Rotation = value;
            }
        }

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public override int UnitCount => cavalries.Count + cohorts.Count;

        public override Vector2 DrawSize => ChildSpacing * Vector2.Scale(cohorts[0].DrawSize, ChildrenDimensions);
        protected override float ChildSpacing => 0.1f;

        IEnumerator<Cavalry> IEnumerable<Cavalry>.GetEnumerator()
        {
            return Cavalries.GetEnumerator();
        }

        IEnumerator<Cohort> IEnumerable<Cohort>.GetEnumerator()
        {
            return Cohorts.GetEnumerator();
        }

        public void AddUnit(Cavalry unit)
        {
            cavalries.Add(unit);
        }

        public void RemoveUnit(Cavalry unit)
        {
            int index = cavalries.IndexOf(unit);
            cavalries.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            int position = 0;
            while (position < cavalries.Count + cohorts.Count)
            {
                yield return
                    position < cavalries.Count ? (UnitBase) cavalries[position] : cohorts[position - cavalries.Count];
                position++;
            }
        }

        public void AddUnit(Cohort unit)
        {
            cohorts.Add(unit);
        }

        public void RemoveUnit(Cohort unit)
        {
            int index = cohorts.IndexOf(unit);
            cohorts.RemoveAt(index);
        }

        public override void Draw()
        {
            foreach (UnitBase unit in this)
                unit.Draw();
        }
    }
}