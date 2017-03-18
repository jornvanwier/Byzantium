using System.Collections;

namespace Assets.Game.Units
{
    public interface IMultipleUnits<T> : IEnumerable
    {
        void AddUnit(T unit);
        void RemoveUnit(T unit);
    }
}