using System.Collections.Generic;

public interface IMultipleUnits<T> : IEnumerable<T>
{
    void AddUnit    (T unit);
    void RemoveUnit (T unit);
}