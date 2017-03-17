using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<DrawableUnit>
    {
        public List<DrawableUnit> DrawableUnits { get; } = new List<DrawableUnit>();

        public void AddUnit(DrawableUnit unit)
        {
            DrawableUnits.Add(unit);
        }

        public void RemoveUnit(DrawableUnit unit)
        {
            int index = DrawableUnits.IndexOf(unit);
            DrawableUnits.RemoveAt(index);
        }
    }
}