using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public abstract class FormationBase : IFormation
    {
        protected const float UnitSize = 0.15f;

        public abstract void Order(Legion unit, bool instant = false);
        public abstract void Order(Contubernium unit, bool instant = false);
        public abstract void Order(Cohort unit, bool instant = false);
        public abstract void Order(Century unit, bool instant = false);

        protected static void ProcessLocalOffsets<T, TChild>(IList<Vector3> offsetPositions, T unit, bool instant)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            ProcessLocalOffsets(offsetPositions, unit, unit, instant);
        }


        protected static void ProcessLocalOffsets<T, TChild>(IList<Vector3> offsetPositions, T unit, IEnumerable<TChild> children, bool instant)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            float maxDist = 0.0f;
            int i = 0;
            Vector3 position = unit.Position;

            foreach (TChild child in children)
            {
                Vector3 newPosition = unit.Rotation * offsetPositions[i];
                Vector3 targetPosition = newPosition + position;
                if (instant)
                {
                    child.SetPositionInstant(targetPosition);
                }
                else
                {
                    Vector3 oldPosition = child.Position;

                    Vector3 definitivePosition = Vector3.MoveTowards(oldPosition, targetPosition, Time.deltaTime);

                    child.Position = definitivePosition;
                    float dist = Vector3.Distance(oldPosition, definitivePosition);
                    maxDist = dist > maxDist ? dist : maxDist;
                }

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