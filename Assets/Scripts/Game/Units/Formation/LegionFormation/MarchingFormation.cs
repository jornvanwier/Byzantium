using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public class MarchingFormation : LegionFormationBase
    {
        public override void Order(Legion unit, bool instant = false)
        {
            const int width = 1;
            int height = unit.UnitCount;

            // Put cavalry first
            List<Cohort> sortedUnits = EnumerableHelper.Glue(unit.ChildrenAreCavalry(true), unit.ChildrenAreCavalry(false)).ToList();

            // Use the drawsize of cavalry since its the largest
            Vector2 spacing = unit.ChildrenAreCavalry(true).First().DrawSize;

            Vector2 offsets;

            offsets.x = (width / 2.0f) * spacing.x - (spacing.x / 2.0f);
            offsets.y = (height / 2.0f) * spacing.y - (spacing.y / 2.0f);

            offsets *= -1;

            var localPositions = new List<Vector3>();

            for (int i = 0; i < height; i++)
            {
                float x = offsets.x;
                float y = offsets.y + i * spacing.y;

                localPositions.Add(new Vector3(x, 0, y));
            }

            ProcessLocalOffsets(localPositions, unit, sortedUnits, true);

            unit.ChildrenDimensions = new Int2(width, height);
        }
    }
}