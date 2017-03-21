using System;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
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

            var maxDist = 0.0f;

            var i = 0;
            foreach (UnitBase u in unit)
            {
                var newPosition = new Vector3(position.x, position.y,
                    position.z + i * unitSize - unitCount / 2f * unitSize);
                Vector3 localPosition = newPosition - position;
                newPosition = unit.Rotation * localPosition;
                Vector3 targetPosition = newPosition + position;
                Vector3 oldPosition = u.Position;
                u.Position = Vector3.MoveTowards(oldPosition, targetPosition, Time.deltaTime);

                float dist = Vector3.Distance(oldPosition, u.Position);
                maxDist = dist > maxDist ? dist : maxDist;
                ++i;
            }

            float speed = maxDist / Time.deltaTime;

            if (speed < unit.WalkSpeed)
            {
                unit.WalkSpeed = speed / 10;
            }
            else
            {
                unit.WalkSpeed = Contubernium.DefaultSpeed;
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
}