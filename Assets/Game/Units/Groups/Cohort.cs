using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        private const float defaultSpeed = 1.5f;
        public float currentSpeed = defaultSpeed;

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

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override float WalkSpeed()
        {
            return currentSpeed;
        }

        public override void WalkSpeed(float speed)
        {
            currentSpeed = speed;
        }
    }
}