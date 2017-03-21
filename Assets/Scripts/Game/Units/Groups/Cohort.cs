using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        protected new const float DefaultSpeed = 1.5f;

        private readonly List<Century> centuries = new List<Century>();

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

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}