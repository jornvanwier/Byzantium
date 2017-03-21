using Assets.Game.Units.Groups;

namespace Assets.Game.Units.Formation
{
    public abstract class FormationBase : IFormation
    {
        public abstract void Order(Legion unit);
        public abstract void Order(Contubernium unit);
        public abstract void Order(Cavalry unit);
        public abstract void Order(Cohort unit);
    }
}