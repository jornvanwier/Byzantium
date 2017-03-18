using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
    {
        private List<DrawableUnit> drawableUnits { get; } = new List<DrawableUnit>();

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
    }
}