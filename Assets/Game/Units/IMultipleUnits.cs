using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IMultipleUnits<in T>
{
    void AddUnit    (T unit);
    void RemoveUnit (T unit);
}