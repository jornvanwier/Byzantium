﻿using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Game.Units.Groups;
using UnityEngine;

namespace Game.Units.Formation
{
    public abstract class FormationBase : IFormation
    {
        protected const float UnitSize = 0.15f;

        public abstract void Order(Legion unit);
        public abstract void Order(Contubernium unit);
        public abstract void Order(Cavalry unit);
        public abstract void Order(Cohort unit);
        public abstract void Order(Century unit);

        public IEnumerable<Vector3> ProcessLocalOffsets(IEnumerable<Vector3> originalPositions,
            IEnumerable<Vector3> offsetPositions, UnitBase unit)
        {
            float maxDist = 0.0f;
            int i = 0;
            Vector3 position = unit.Position;

            var orPos = new List<Vector3>(originalPositions);
            var newWorldPositions = new List<Vector3>();

            foreach (Vector3 u in offsetPositions)
            {
                Vector3 newPosition = unit.Rotation * u;
                Vector3 targetPosition = newPosition + position;
                Vector3 oldPosition = orPos[i];
                Vector3 definitivePosition = Vector3.MoveTowards(oldPosition, targetPosition, Time.deltaTime);
                newWorldPositions.Add(definitivePosition);

                float dist = Vector3.Distance(oldPosition, definitivePosition);
                maxDist = dist > maxDist ? dist : maxDist;
                ++i;
            }

            float speed = maxDist / Time.deltaTime;

            if (speed < unit.WalkSpeed)
                unit.WalkSpeed = speed / 10;
            else
                unit.WalkSpeed = unit.DefaultSpeed;

            return newWorldPositions;
        }
    }
}