using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
    {
        private List<DrawableUnit> drawableUnits = new List<DrawableUnit>();

        public int GetGroupSize()
        {
            return drawableUnits.Count;
        }

        public void AddUnit(DrawableUnit unit)
        {
            drawableUnits.Add(unit);
        }

        public void RemoveUnit(DrawableUnit unit)
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

        public virtual void Draw()
        {
            foreach (DrawableUnit u in drawableUnits)
            {
                u.Draw();
            }
            
        }


    }
}