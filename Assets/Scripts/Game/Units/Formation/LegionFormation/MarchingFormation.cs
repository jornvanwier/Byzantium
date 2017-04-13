using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public class MarchingFormation : LegionFormationBase
    {
        public override void Order(Legion unit, bool instant = false)
        {
            var localPositions = new List<Vector3>();

            Cohort[] sortedCohorts = SortCavalryFirst(unit);

            const float sizeMultiplier = 2.8f;
            float totalSize = 0;
            int i = 0;

            foreach (Cohort cohort in sortedCohorts)
            {
                totalSize += cohort.DrawSize.y * sizeMultiplier;

                localPositions.Add(new Vector3(0, 0, i++ * cohort.DrawSize.y * sizeMultiplier));
            }

            IList<Vector3> offsetPositions = localPositions.Select(l =>
                {
                    l.z -= totalSize / 2;
                    return l;
                })
                .ToList();

            ProcessLocalOffsets<Legion, Cohort>(offsetPositions, unit, instant);

            unit.ChildrenDimensions = new Int2(unit.UnitCount, 1);
        }
    }
}