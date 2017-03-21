using Assets.Game.Units.Groups;

public interface IFormation
{
    void Order(Legion unit);
    void Order(Contubernium unit);
    void Order(Cavalry unit);
    void Order(Cohort unit);
}