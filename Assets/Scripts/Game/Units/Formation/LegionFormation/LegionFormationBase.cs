using Assets.Scripts.Game.Units.Groups;
using Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public abstract class LegionFormationBase : FormationBase
    {
        public override void Order(Contubernium unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Cavalry unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Cohort unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Century unit)
        {
            throw new FormationIncompatibleException(unit);
        }
    }
}