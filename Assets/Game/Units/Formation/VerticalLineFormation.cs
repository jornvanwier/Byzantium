using UnityEngine;
using System.Collections;
using System;
using Assets.Game.Units.Groups;
using Assets.Game.Units;

public class VerticalLineFormation : FormationBase
{
    public override void Order(Legion unit)
    {
        throw new NotImplementedException();
    }

    public override void Order(Contubernium unit)
    {
        const float unitSize = 0.15f;
        int unitCount = unit.GetGroupSize();
        Vector3 position = unit.Position;

        float maxDist = 0.0f;

        int i = 0;
        foreach (UnitBase u in unit)
        {
            Vector3 newPosition = new Vector3(position.x, position.y, position.z + ( i * unitSize) - (unitCount / 2 * unitSize));
            Vector3 localPosition = newPosition - position;
            newPosition = unit.Rotation * localPosition;
            Vector3 targetPosition = newPosition + position;
            Vector3 oldPosition = u.Position;
            u.Position = Vector3.MoveTowards(oldPosition, targetPosition, Time.deltaTime);

            float dist = Vector3.Distance(oldPosition, u.Position);
            maxDist = dist > maxDist ? dist : maxDist;
            ++i;
        }

        float normalDistance = Contubernium.defaultSpeed * Time.deltaTime;
        float speed = maxDist / Time.deltaTime;

        if (speed < unit.WalkSpeed())
        {
            unit.WalkSpeed(speed / 10);
        }
        else
        {
            unit.WalkSpeed(Contubernium.defaultSpeed);
        }





    }

    public override void Order(Cavalry unit)
    {
        throw new NotImplementedException();
    }

    public override void Order(Cohort unit)
    {
        throw new NotImplementedException();
    }
}
