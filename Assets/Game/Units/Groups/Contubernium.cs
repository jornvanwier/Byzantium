using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
{
    public void AddUnit(DrawableUnit unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(DrawableUnit unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<DrawableUnit> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}