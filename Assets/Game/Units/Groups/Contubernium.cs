using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

        public int GetGroupSize()
        {
            return drawableUnits.Count;
        }

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

        public override Vector3 Position { set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public override void Draw()
        {
            foreach (MeshDrawableUnit u in drawableUnits)
            {
                u.Draw();
            }
            
        }


    }
}