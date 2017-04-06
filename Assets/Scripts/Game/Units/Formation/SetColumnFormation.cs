using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SetColumnFormation : FormationBase
    {
        private readonly int length;

        public SetColumnFormation(int length)
        {
            this.length = length;
        }

        public override void Order(Legion unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit)
        {
            SquareFormation.OrderAnySetColumn<Contubernium, MeshDrawableUnit>(length, unit);
        }

        public override void Order(Cohort unit)
        {
            SquareFormation.OrderAnySetColumn<Cohort, Century>(length, unit);
        }

        public override void Order(Century unit)
        {
            SquareFormation.OrderAnySetColumn<Century, Contubernium>(length, unit);
        }
    }
}