using System;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;
using System.Collections.Generic;

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
            
            int unitCount = unit.GetGroupSize();
            Vector3 position = unit.Position;
            var i = 0;
            List<Vector3> localPositions = new List<Vector3>();
            List<Vector3> originalpositions = new List<Vector3>();
            
            foreach (UnitBase u in unit)
            {
                Vector3 newPosition = new Vector3(position.x, position.y,
                    position.z + i * unitSize - unitCount / 2f * unitSize);
                Vector3 localPosition = newPosition - position;
                localPositions.Add(localPosition);
                originalpositions.Add(u.Position);
                ++i;
            }
            List<Vector3> list = new List<Vector3>(processLocalOffsets(originalpositions, localPositions, Contubernium.DefaultSpeed, unit));
            int j = 0;
            foreach (UnitBase u in unit)
            {
                u.Position = list[j++];
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