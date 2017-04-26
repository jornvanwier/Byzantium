using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation
{
    public interface IFormation
    {
        void Order(Legion unit, bool instant = false);
        void Order(Contubernium unit, bool instant = false);
        void Order(Cohort unit, bool instant = false);
        void Order(Century unit, bool instant = false);
        FormationStats Stats { get; }
    }
}