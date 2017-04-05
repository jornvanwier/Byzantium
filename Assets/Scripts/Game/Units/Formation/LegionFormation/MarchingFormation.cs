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

            float cavalrySize = unit.CavalryCount * unit.Cavalries.First().DrawSize.x;
            float totalSizeY = cavalrySize + unit.CohortCount * unit.Cohorts.First().DrawSize.y;

            int i = 0;

            foreach (Cavalry c in (IEnumerable<Cavalry>) unit)
                localPositions.Add(new Vector3(0, 0, i++ * unit.Cavalries.First().DrawSize.y - totalSizeY / 2));

            ProcessLocalOffsets<Legion, Cavalry>(localPositions, unit);

            unit.ChildrenDimensionsCavalry = new Int2(1, unit.CavalryCount);

            localPositions.Clear();

            i = 0;

            foreach (Cohort c in (IEnumerable<Cohort>) unit)
                localPositions.Add(new Vector3(0, 0,
                    cavalrySize + i++ * unit.Cohorts.First().DrawSize.y - totalSizeY / 2));

            ProcessLocalOffsets<Legion, Cohort>(localPositions, unit);

            unit.ChildrenDimensionsCohort = new Int2(1, unit.CohortCount);
            unit.ChildrenDimensions = new Int2(1, unit.UnitCount);
        }
    }
}