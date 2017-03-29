using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Game.Units.Groups;
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
            int unitCount = unit.GetGroupSize();
            Vector3 position = unit.Position;
            int i = 0;
            var localPositions = new List<Vector3>();
            var originalpositions = new List<Vector3>();

            foreach (UnitBase u in unit)
            {
                var newPosition = new Vector3(position.x, position.y,
                    position.z + i * UnitSize - unitCount / 2f * UnitSize);
                Vector3 localPosition = newPosition - position;
                localPositions.Add(localPosition);
                originalpositions.Add(u.Position);
                ++i;
            }
            var list =
                new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));
            int j = 0;
            foreach (UnitBase u in unit)
                u.Position = list[j++];
        }

        public void OrderLinq(Contubernium unit)
        {
            int unitCount = unit.GetGroupSize();
            Vector3 position = unit.Position;
            int i = 0;

            List<Vector3> localPositions = (from UnitBase u in unit
                    select
                    new Vector3(position.x, position.y, position.z + i++ * UnitSize - unitCount / 2f * UnitSize) -
                    position)
                .ToList();
            List<Vector3> originalpositions = (from UnitBase u in unit select u.Position).ToList();

            var list = new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));
            int j = 0;
            foreach (UnitBase u in unit)
                u.Position = list[j++];
        }

        public override void Order(Cavalry unit)
        {
            throw new NotImplementedException();
        }

        public override void Order(Cohort unit)
        {
            throw new NotImplementedException();
        }

        public override void Order(Century unit)
        {
            throw new NotImplementedException();
        }
    }
}