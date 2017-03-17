using System.Runtime.Remoting.Messaging;

namespace Assets.Game.Units.Groups
{
    public class Cohort : UnitGroup
    {
        public override bool ShouldPathfind => true;

        public void AddUnit(Century unit)
        {
            AddUnitInternal(unit);
        }
    }
}