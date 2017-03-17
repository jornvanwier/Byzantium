using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>
{
    public void AddUnit(Cohort unit)
    {
        throw new NotImplementedException();
    }

    public void AddUnit(Cavalry unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Cohort> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Cohort unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Cavalry unit)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<Cavalry> IEnumerable<Cavalry>.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
