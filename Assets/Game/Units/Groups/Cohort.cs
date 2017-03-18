using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        private List<Century> centuries = new List<Century>();

        public void AddUnit(Century unit)
        {
            centuries.Add(unit);
        }

        public void RemoveUnit(Century unit)
        {
            int index = centuries.IndexOf(unit);
            centuries.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return centuries.GetEnumerator();
        }

    }
}