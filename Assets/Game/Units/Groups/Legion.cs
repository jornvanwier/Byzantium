using System.Collections.Generic;
using Assets.Game.Units.Groups.Formations;

namespace Assets.Game.Units.Groups
{
    public class Legion : UnitGroup
    {
        public ILegionFormation Formation { get; set; }

        public override bool ShouldPathfind => true;

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

        public override void Order()
        {
            Formation.Order(this);
        }
    }
}