using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        public new const float DefaultSpeed = 1.5f;

        private readonly List<Century> centuries = new List<Century>();

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

        public override int UnitCount => centuries.Count;

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
            foreach (Century unit in this)
            {
                unit.Draw();
            }
        }
    }
}