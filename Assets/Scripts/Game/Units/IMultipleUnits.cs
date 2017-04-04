using System.Collections.Generic;

namespace Assets.Scripts.Game.Units
{
    public interface IMultipleUnits<T> : IEnumerable<T>
    {
        void AddUnit(T unit);
        void RemoveUnit(T unit);
    }
}