using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Game.Units.Groups;
using UnityEngine;

namespace Game.Units.Formation
{
    public class SquareFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            // Can't use generic method for legion because it contains both cohorts and cavalry
            throw new NotImplementedException();
        }

        public override void Order(Contubernium unit)
        {
            OrderAny<Contubernium, MeshDrawableUnit>(unit);
//            Debug.Log(unit.DrawSize);
        }

        public override void Order(Cavalry unit)
        {
            OrderAny<Cavalry, MeshDrawableUnit>(unit);
        }

        public override void Order(Cohort unit)
        {
            OrderAny<Cohort, Century>(unit);
        }

        public override void Order(Century unit)
        {
            OrderAny<Century, Contubernium>(unit);
            Debug.Log(unit.DrawSize);
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
            int rowWidth = (int)Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth;

            OrderAny<T, TChild>(rowWidth, columnHeight, unit);
        }

        private static void OrderAny<T, TChild>(int rowWidth, int columnHeight, T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            var localPositions = new List<Vector3>();
            var originalpositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (UnitBase child in unit)
            {
                float x = unit.Position.x + spacing.x * (i % rowWidth) - spacing.x * rowWidth / 4;
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing.y * (i / rowWidth) - spacing.y * columnHeight / 4;

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);
                originalpositions.Add(child.Position);

                ++i;
            }

            var processed = new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));

            int j = 0;
            foreach (UnitBase u in unit)
                u.Position = processed[j++];

            unit.ChildrenDimensions = new Int2(rowWidth, columnHeight);
        }
    }
}