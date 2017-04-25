using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public class StandardFormation : LegionFormationBase
    {
        public override void Order(Legion unit, bool instant = false)
        {
            instant = true;

            PlaceCavalry(unit, PlaceCohorts(unit, instant) / 2, instant);

            int height = (int) Mathf.Sqrt(unit.UnitCount);
            int width = unit.UnitCount / height;
            unit.ChildrenDimensions = new Int2(width + 2, height + 2);
        }


        // Returns width
        private static float PlaceCohorts(Legion unit, bool instant)
        {
            IEnumerable<Cohort> enumerable = unit.ChildrenAreCavalry(false);
            Cohort[] cohorts = enumerable as Cohort[] ?? enumerable.ToArray();
            int cohortCount = cohorts.Length;
            int width = Mathf.CeilToInt(Mathf.Sqrt(cohortCount));
            int height = Mathf.CeilToInt(cohortCount / (float)width);

            Vector2 spacing = cohorts.First().DrawSize;

            Vector2 offsets;

            offsets.x = width / 2.0f * spacing.x - spacing.x / 2.0f;
            offsets.y = height / 2.0f * spacing.y - spacing.y / 2.0f;

            offsets *= -1;

            var localPositions = new List<Vector3>();

            int inRowCounter = 0;
            int inColumnCounter = 0;
            for (int i = 0; i < cohortCount; i++)
            {
                float x = offsets.x + inColumnCounter * spacing.x;
                float y = offsets.y + inRowCounter * spacing.y;

                localPositions.Add(new Vector3(x, 0, y));

                ++inColumnCounter;

                if (inColumnCounter >= height)
                {
                    inColumnCounter = 0;
                    inRowCounter++;
                }
            }

            ProcessLocalOffsets(localPositions, unit, cohorts, instant);

            return spacing.x * width;
        }

        private static void PlaceCavalry(Legion unit, float cohortWidth, bool instant)
        {
            IEnumerable<Cohort> enumerable = unit.ChildrenAreCavalry(true);
            Cohort[] cavalry = enumerable as Cohort[] ?? enumerable.ToArray();
            int cavalryCount = cavalry.Length;

            Vector2 spacing = cavalry.First().DrawSize;


            float offsetY = -Mathf.CeilToInt(cavalryCount / 2f) * spacing.y / 2f - spacing.y / 2f;


            var localPositions = new List<Vector3>();
            for (int i = 0; i < cavalryCount; i++)
            {
                int xOffsetMultiply = i % 2 == 0 ? 1 : -3;

                float x = cohortWidth + spacing.x * xOffsetMultiply;
                float y = offsetY + spacing.y * (i - i % 2);

                localPositions.Add(new Vector3(x, 0, y));
            }

            ProcessLocalOffsets(localPositions, unit, cavalry, instant);
        }
    }
}