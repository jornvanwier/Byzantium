namespace Assets.Game.Units.Groups
{
    public class Cohort : UnitGroup
    {

        public void AddUnit(Century unit)
        {
            AddUnitInternal(unit);
        }
    }
}