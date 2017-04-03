using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation
{
    public interface IFormation
    {
        void Order(Legion unit);
        void Order(Contubernium unit);
        void Order(Cavalry unit);
        void Order(Cohort unit);
        void Order(Century unit);
    }
}