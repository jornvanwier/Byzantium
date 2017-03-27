using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        public new const float DefaultSpeed = 1.5f;
        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

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

        public override int UnitCount => drawableUnits.Count;

        public void AddUnit(MeshDrawableUnit unit)
        {
            drawableUnits.Add(unit);
        }

        public void RemoveUnit(MeshDrawableUnit unit)
        {
            int index = drawableUnits.IndexOf(unit);
            drawableUnits.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return drawableUnits.GetEnumerator();
        }

        public override void Draw()
        {
            foreach (MeshDrawableUnit unit in this)
            {
                unit.Draw();
            }

        }
    }
}