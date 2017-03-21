using Assets.Game.Units.Groups;

namespace Assets.Game.Units.Formation
{
    public interface IFormation
    {
        void Order(Legion unit);
        void Order(Contubernium unit);
        void Order(Cavalry unit);
        void Order(Cohort unit);
    }
}