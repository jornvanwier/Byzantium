using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private List<MeshDrawableUnit> DrawableUnits = new List<MeshDrawableUnit>();

        public void AddUnit(MeshDrawableUnit unit)
        {
            DrawableUnits.Add(unit);
        }

        public void RemoveUnit(MeshDrawableUnit unit)
        {
            int index = DrawableUnits.IndexOf(unit);
            DrawableUnits.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return DrawableUnits.GetEnumerator();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}