using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>, IEnumerable<Cohort>,
        IEnumerable<Cavalry>
    {
        public new const float DefaultSpeed = 1.5f;

        private readonly List<Cohort> cohorts = new List<Cohort>();
        private readonly List<Cavalry> cavalries = new List<Cavalry>();
        public IEnumerable<Cavalry> Cavalries => cavalries;
        public IEnumerable<Cohort> Cohorts => cohorts;

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
            var position = 0;
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
            throw new NotImplementedException();
        }
    }
}