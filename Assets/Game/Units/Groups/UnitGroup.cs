using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Units.Groups
{
    public abstract class UnitGroup : Unit
    {
        public List<Unit> Children { get; set; }

        protected UnitGroup()
        {

        }

        protected override void SetWorldPos(Vector3 worldPos)
        {
            base.SetWorldPos(worldPos);
        }

        protected override void SetWorldRotation(Quaternion rotation)
        {
            base.SetWorldRotation(rotation);
        }

        protected virtual void AddUnitInternal(Unit unit)
        {
            Children.Add(unit);
        }
    }
}