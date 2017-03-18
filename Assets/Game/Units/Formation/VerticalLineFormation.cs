using UnityEngine;
using System.Collections;
using System;
using Assets.Game.Units.Groups;
using Assets.Game.Units;

public class VerticalLineFormation : IFormation
{
    public void Order(Legion unit)
    {
        throw new NotImplementedException();
    }

    public void Order(Contubernium unit)
    {
        const float unitSize = 0.15f;
        int unitCount = unit.GetGroupSize();
        Vector3 position = unit.Position;

        int i = 0;
        foreach (UnitBase u in unit)
        {
            u.Position = new Vector3(position.x, position.y, position.z + ( i * unitSize) - (unitCount / 2 * unitSize));
            u.Position = unit.Rotation * u.Position;
            ++i;
        }      
    }

    public void Order(Cavalry unit)
    {
        throw new NotImplementedException();
    }

    public void Order(Cohort unit)
    {
        throw new NotImplementedException();
    }
}
