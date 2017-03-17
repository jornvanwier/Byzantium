using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>, IEnumerable<UnitBase>
    {
        public List<Cavalry> Cavalry { get; } = new List<Cavalry>();
        public List<Cohort> Cohorts { get; } = new List<Cohort>();

        public void AddUnit(Cohort unit)
        {
            throw new NotImplementedException();
        }

        public void AddUnit(Cavalry unit)
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(Cohort unit)
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(Cavalry unit)
        {
            throw new NotImplementedException();
        }

        IEnumerator<UnitBase> IEnumerable<UnitBase>.GetEnumerator()
        {
            int position = 0;
            while (position < Cavalry.Count + Cohorts.Count)
            {
                yield return position < Cavalry.Count ? (UnitBase) Cavalry[position] : Cohorts[position - Cavalry.Count]
                    ;
                position++;
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}