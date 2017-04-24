using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public abstract class LegionFormationBase : FormationBase
    {
        public override void Order(Contubernium unit, bool instant = false)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Cohort unit, bool instant = false)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Century unit, bool instant = false)
        {
            throw new FormationIncompatibleException(unit);
        }
    }
}