using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public class MarchingFormation : LegionFormationBase
    {
        public override void Order(Legion unit)
        {
            var localPositions = new List<Vector3>();

            Cohort[] sortedCohorts = SortCavalryFirst(unit);

            float totalSize = 0;
            int i = 0;

            foreach (Cohort cohort in unit)
            {
                totalSize += cohort.DrawSize.x;
                
                localPositions.Add(new Vector3(i++ * cohort.DrawSize.x, 0, 0));
            }

            IList<Vector3> offsetPositions = localPositions.Select(l => {
                l.x -= totalSize;
                return l;
            }).ToList();

            ProcessLocalOffsets<Legion, Cohort>(offsetPositions, unit);

            unit.ChildrenDimensions = new Int2(unit.UnitCount, 1);
        }
    }
}