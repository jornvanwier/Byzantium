namespace Assets.Game.Units
{
    public interface IMultipleUnits<in T>
    {
        void AddUnit    (T unit);
        void RemoveUnit (T unit);
    }
}