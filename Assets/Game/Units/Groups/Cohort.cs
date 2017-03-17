using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        public List<Century> Centuries { get; } = new List<Century>();

        public void AddUnit(Century unit)
        {
            Centuries.Add(unit);
        }

        public void RemoveUnit(Century unit)
        {
            int index = Centuries.IndexOf(unit);
            Centuries.RemoveAt(index);
        }
    }
}