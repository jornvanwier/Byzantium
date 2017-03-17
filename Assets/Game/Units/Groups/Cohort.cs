using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohort : UnitBase, IMultipleUnits<Century>
{
    public void AddUnit(Century unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Century unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Century> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
