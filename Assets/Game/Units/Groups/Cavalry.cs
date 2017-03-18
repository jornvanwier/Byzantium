using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<DrawableUnit>
    {
        private List<DrawableUnit> DrawableUnits = new List<DrawableUnit>();

        public void AddUnit(DrawableUnit unit)
        {
            DrawableUnits.Add(unit);
        }

        public void RemoveUnit(DrawableUnit unit)
        {
            int index = DrawableUnits.IndexOf(unit);
            DrawableUnits.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return DrawableUnits.GetEnumerator();
        }

    }
}