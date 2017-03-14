using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Legion : UnitGroup
    {
        public Legion() : base()
        {

        }

        public void AddUnit(Cohort unit)
        {
            AddUnitInternal(unit);
        }

        public void AddUnit(Cavalry unit)
        {
            AddUnitInternal(unit);
        }
    }
}