using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public abstract class FormationBase : IFormation
    {
        protected const float UnitSize = 0.15f;

        public abstract void Order(Legion unit);
        public abstract void Order(Contubernium unit);
        public abstract void Order(Cohort unit);
        public abstract void Order(Century unit);

        protected static void ProcessLocalOffsets<T, TChild>(IList<Vector3> offsetPositions, T unit)
            where T : UnitBase, IMultipleUnits<TChild> where TChild : UnitBase
        {
            float maxDist = 0.0f;
            int i = 0;
            Vector3 position = unit.Position;

            foreach (TChild child in unit)
            {
                Vector3 oldPosition = child.Position;
                Vector3 newPosition = unit.Rotation * offsetPositions[i];
                Vector3 targetPosition = newPosition + position;
                Vector3 definitivePosition = Vector3.MoveTowards(oldPosition, targetPosition, Time.deltaTime);

                child.Position = definitivePosition;

                float dist = Vector3.Distance(oldPosition, definitivePosition);
                maxDist = dist > maxDist ? dist : maxDist;
                ++i;
            }

            float speed = maxDist / Time.deltaTime;

            if (speed < unit.WalkSpeed)
                unit.WalkSpeed = speed / 10;
            else
                unit.WalkSpeed = unit.DefaultSpeed;
        }
    }
}