using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SetColumnFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit)
        {
            SquareFormation.OrderAnySetColumn<Contubernium, MeshDrawableUnit>(2, unit);
        }

        public override void Order(Cavalry unit)
        {
            SquareFormation.OrderAnySetColumn<Cavalry, MeshDrawableUnit>(1, unit);
        }

        public override void Order(Cohort unit)
        {
            SquareFormation.OrderAnySetColumn<Cohort, Century>(3, unit);
        }

        public override void Order(Century unit)
        {
            SquareFormation.OrderAnySetColumn<Century, Contubernium>(2, unit);
        }
    }
}