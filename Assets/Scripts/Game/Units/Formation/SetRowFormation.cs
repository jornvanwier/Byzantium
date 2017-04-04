using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SetRowFormation : FormationBase
    {
        private readonly int length;

        public SetRowFormation(int length)
        {
            this.length = length;
        }

        public override void Order(Legion unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit)
        {
            SquareFormation.OrderAnySetRow<Contubernium, MeshDrawableUnit>(length, unit);
        }

        public override void Order(Cavalry unit)
        {
            SquareFormation.OrderAnySetRow<Cavalry, MeshDrawableUnit>(length, unit);
        }

        public override void Order(Cohort unit)
        {
            SquareFormation.OrderAnySetRow<Cohort, Century>(length, unit);
        }

        public override void Order(Century unit)
        {
            SquareFormation.OrderAnySetRow<Century, Contubernium>(length, unit);
        }
    }
}