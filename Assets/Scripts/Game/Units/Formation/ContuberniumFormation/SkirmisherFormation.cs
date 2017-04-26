using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormation
{
    public class SkirmisherFormation : ContuberniumFormationBase
    {
        public override FormationStats Stats { get; } = new FormationStats
        {
            WalkSpeed = FormationStats.DefaultWalkSpeed,
            AttackDamageMultiplier = 1.5f,
            DefenseMultiplier = 0.6f
        };

        public override void Order(Contubernium unit, bool instant = false)
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth * 2;

            var localPositions = new List<Vector3>();

            int unitCount = unit.UnitCount;


            Vector2 spacing = unit.First().DrawSize;
            Vector2 offsets;

            offsets.x = columnHeight / 2.0f * spacing.x - spacing.x / 2.0f;
            offsets.y = rowWidth / 2.0f * spacing.y - spacing.y / 2.0f;

            offsets *= -1;

            int inRowCounter = 0;
            int inColumnCounter = 0;
            for (int i = 0; i < unitCount * 2; i++)
            {
                if (i % 2 == 0)
                {
                    ++inColumnCounter;
                    continue;
                }

                float x = offsets.x + inColumnCounter * spacing.x;
                float y = offsets.y + inRowCounter * spacing.y;

                localPositions.Add(new Vector3(x, 0, y));

                ++inColumnCounter;

                if (inColumnCounter >= columnHeight)
                {
                    inColumnCounter = 0;
                    inRowCounter++;
                }
            }

            ProcessLocalOffsets<Contubernium, MeshDrawableUnit>(localPositions, unit, instant);

            unit.ChildrenDimensions = new Int2(columnHeight, rowWidth);
        }
    }
}