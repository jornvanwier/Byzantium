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

        public override void Order(Legion unit, bool instant = false)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit, bool instant = false)
        {
            SquareFormation.OrderAnySetColumn<Contubernium, MeshDrawableUnit>(length, unit, instant);
        }

        public override void Order(Cohort unit, bool instant = false)
        {
            SquareFormation.OrderAnySetColumn<Cohort, Century>(length, unit, instant);
        }

        public override void Order(Century unit, bool instant = false)
        {
            SquareFormation.OrderAnySetColumn<Century, Contubernium>(length, unit, instant);
        }
    }
}