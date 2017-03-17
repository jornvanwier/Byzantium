using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class Century : UnitBase, IMultipleUnits<Contubernium>
{
    public void AddUnit(Contubernium unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Contubernium unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Contubernium> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
