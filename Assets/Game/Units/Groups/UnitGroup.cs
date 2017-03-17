using System;
using System.Collections.Generic;
using Assets.Game.Units.Groups.Formations;
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

        public override void Order()
        {
            foreach (Unit child in Children)
            {
                child.Order();
            }
        }

        protected virtual void AddUnitInternal(Unit unit)
        {
            Children.Add(unit);
            unit.Parent = this;
        }
    }
}