using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
<<<<<<< HEAD
using Game.Units;
using Game.Units.Groups;
=======
>>>>>>> 7614ab8d53f8e87ba5e5818bb40feaf144ba48e5
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

            foreach (UnitBase child in unit)
            {
                float x = unit.Position.x + spacing.x * (i % rowWidth) - spacing.x * rowWidth / 4;
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing.y * (i / rowWidth) - spacing.y * columnHeight / 4;

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);

                ++i;
            }

<<<<<<< HEAD
            ProcessLocalOffsets<T, TChild>(localPositions, unit);
=======
            var processed = new List<Vector3>(ProcessLocalOffsets<T, TChild>(localPositions, unit));

            int j = 0;
            foreach (UnitBase u in unit)
                u.Position = processed[j++];
>>>>>>> 7614ab8d53f8e87ba5e5818bb40feaf144ba48e5

            unit.ChildrenDimensions = new Int2(columnHeight, rowWidth);
        }
    }
}