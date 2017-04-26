using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SquareFormation : FormationBase
    {
        public override void Order(Legion unit, bool instant = false)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit, bool instant = false)
        {
            OrderAny<Contubernium, MeshDrawableUnit>(unit, instant);
        }

        public override void Order(Cohort unit, bool instant = false)
        {
            OrderAny<Cohort, Century>(unit, instant);
        }

        public override void Order(Century unit, bool instant = false)
        {
            OrderAny<Century, Contubernium>(unit, instant);
        }

        public void OrderAnySetRow<T, TChild>(int width, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int columnHeight = unit.UnitCount / width;

            OrderAny<T, TChild>(width, columnHeight, unit, instant);
        }

        public void OrderAnySetColumn<T, TChild>(int height, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int rowWidth = unit.UnitCount / height;

            OrderAny<T, TChild>(rowWidth, height, unit, instant);
        }

        public void OrderAny<T, TChild>(T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth;

            OrderAny<T, TChild>(rowWidth, columnHeight, unit, instant);
        }

        private void OrderAny<T, TChild>(int rowWidth, int columnHeight, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            var localPositions = new List<Vector3>();

            int unitCount = unit.UnitCount;


            Vector2 spacing = unit.First().DrawSize;
            Vector2 offsets;

            offsets.x = columnHeight / 2.0f * spacing.x - spacing.x / 2.0f;
            offsets.y = rowWidth / 2.0f * spacing.y - spacing.y / 2.0f;

            offsets *= -1;

            int inRowCounter = 0;
            int inColumnCounter = 0;
            for (int i = 0; i < unitCount; i++)
            {
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

            ProcessLocalOffsets<T, TChild>(localPositions, unit, instant);

            unit.ChildrenDimensions = new Int2(columnHeight, rowWidth);
        }

        public override FormationStats Stats { get; } = new FormationStats
        {
            WalkSpeed = FormationStats.DefaultWalkSpeed,
            AttackDamageMultiplier = 1f,
            DefenseMultiplier = 1f
        };
    }
}