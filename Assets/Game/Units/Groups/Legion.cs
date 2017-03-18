using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>, IEnumerable<Cohort>, IEnumerable<Cavalry>
    {
        private List<Cavalry> cavalry = new List<Cavalry>();
        private List<Cohort> cohorts = new List<Cohort>();

        public IEnumerable<Cavalry> Cavalries { get { return cavalry;  } }
        public IEnumerable<Cohort> Cohorts { get { return cohorts;  } }

        public IEnumerator GetEnumerator()
        {
            var position = 0;
            while (position < cavalry.Count + cohorts.Count)
            {
                yield return position < cavalry.Count ? (UnitBase) cavalry[position] : cohorts[position - cavalry.Count];
                position++;
            }
        }

        public void AddUnit(Cavalry unit)
        {
            cavalry.Add(unit);
        }

        public void RemoveUnit(Cavalry unit)
        {
            int index = cavalry.IndexOf(unit);
            cavalry.RemoveAt(index);
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

        IEnumerator<Cohort> IEnumerable<Cohort>.GetEnumerator()
        {
            return cohorts.GetEnumerator();
        }

        IEnumerator<Cavalry> IEnumerable<Cavalry>.GetEnumerator()
        {
            return cavalry.GetEnumerator();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}