using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormations
{
    public abstract class ContuberniumFormationBase : FormationBase
    {
        public override void Order(Legion unit)
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