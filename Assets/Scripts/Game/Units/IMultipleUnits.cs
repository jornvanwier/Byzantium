using System.Collections;

namespace Assets.Scripts.Game.Units
{
    public interface IMultipleUnits<T> : IEnumerable
    {
        void AddUnit(T unit);
        void RemoveUnit(T unit);
    }
}