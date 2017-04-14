using System.Collections.Generic;
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

        public static void OrderAnySetRow<T, TChild>(int width, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int columnHeight = unit.UnitCount / width;

            OrderAny<T, TChild>(width, columnHeight, unit, instant);
        }

        public static void OrderAnySetColumn<T, TChild>(int height, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int rowWidth = unit.UnitCount / height;

            OrderAny<T, TChild>(rowWidth, height, unit, instant);
        }

        public static void OrderAny<T, TChild>(T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth;

            OrderAny<T, TChild>(rowWidth, columnHeight, unit, instant);
        }

        private static void OrderAny<T, TChild>(int rowWidth, int columnHeight, T unit, bool instant = false)
            where T : UnitGroup<TChild> where TChild : UnitBase
        {
            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (TChild child in unit)
            {
                float x = spacing.x * (i % rowWidth) - spacing.x;
                // ReSharper disable once PossibleLossOfFraction
                float z = spacing.y * (i / rowWidth) - spacing.y;

                localPositions.Add(new Vector3(x, 0, z));

                ++i;
            }

            ProcessLocalOffsets<T, TChild>(localPositions, unit, instant);

            unit.ChildrenDimensions = new Int2(columnHeight, rowWidth);
        }
    }
}