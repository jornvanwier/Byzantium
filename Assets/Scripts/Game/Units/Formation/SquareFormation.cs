using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SquareFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Contubernium unit)
        {
            OrderAny<Contubernium, MeshDrawableUnit>(unit);
        }

        public override void Order(Cohort unit)
        {
            OrderAny<Cohort, Century>(unit);
        }

        public override void Order(Century unit)
        {
            OrderAny<Century, Contubernium>(unit);
        }

        public static void OrderAnySetRow<T, TChild>(int width, T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            int columnHeight = unit.UnitCount / width;

            OrderAny<T, TChild>(width, columnHeight, unit);
        }

        public static void OrderAnySetColumn<T, TChild>(int height, T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            int rowWidth = unit.UnitCount / height;

            OrderAny<T, TChild>(rowWidth, height, unit);
        }

        public static void OrderAny<T, TChild>(T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth;

            OrderAny<T, TChild>(rowWidth, columnHeight, unit);
        }

        private static void OrderAny<T, TChild>(int rowWidth, int columnHeight, T unit)
            where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (TChild child in unit)
            {
                float x = unit.Position.x + spacing.x * (i % rowWidth) - spacing.x / 2;
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing.y * (i / rowWidth) - spacing.y / 2;

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);

                ++i;
            }

            ProcessLocalOffsets<T, TChild>(localPositions, unit);

            unit.ChildrenDimensions = new Int2(columnHeight, rowWidth);
        }
    }
}