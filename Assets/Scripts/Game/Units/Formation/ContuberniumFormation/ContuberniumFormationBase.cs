using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormation
{
    public abstract class ContuberniumFormationBase : FormationBase
    {
        public override void Order(Legion unit, bool instant = false)
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