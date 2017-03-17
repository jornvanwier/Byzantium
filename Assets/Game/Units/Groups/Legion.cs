using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>
{
    public List<Cavalry> Cavalry { get; } = new List<Cavalry>();

    public List<Cohort> Cohorts { get; } = new List<Cohort>();


    public void AddUnit(Cohort unit)
    {
        Cohorts.Add(unit);
    }

    public void AddUnit(Cavalry unit)
    {
        Cavalry.Add(unit);
    }

    public void RemoveUnit(Cohort unit)
    {
        int index = Cohorts.IndexOf(unit);
        Cohorts.RemoveAt(index);
    }

    public void RemoveUnit(Cavalry unit)
    {
        int index = Cavalry.IndexOf(unit);
        Cavalry.RemoveAt(index);
    }

    IEnumerator<UnitBase> IEnumerable<UnitBase>.GetEnumerator()
    {
        int position = 0;
        while (position < Cavalry.Count + Cohorts.Count)
        {
            yield return position < Cavalry.Count ? (UnitBase) Cavalry[position] : Cohorts[position - Cavalry.Count];
            position++;
        }
    }
}