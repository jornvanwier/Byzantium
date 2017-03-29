﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Map;
using Game.Units;
using Game.Units.Formation;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SetRowFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            throw new NotImplementedException();
        }

        public override void Order(Contubernium unit)
        {
            OrderAny<Contubernium, MeshDrawableUnit>(2, 0.3f, unit);
        }

        public override void Order(Cavalry unit)
        {
            OrderAny<Cavalry, MeshDrawableUnit>(3, 0.4f, unit);
        }

        public override void Order(Cohort unit)
        {
            OrderAny<Cohort, Century>(3, 5, unit);
        }

        public override void Order(Century unit)
        {
            OrderAny<Century, Contubernium>(1, 2, unit);
        }

        private void OrderAny<T, TChild>(int width, float spacing, T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            int columnHeight = unit.UnitCount / width;

            var localPositions = new List<Vector3>();
            var originalpositions = new List<Vector3>();

            int i = 0;

            foreach (UnitBase child in unit)
            {
                float x = unit.Position.x + spacing * (i % width) - spacing * width / 4;
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing * (i / width) - spacing * columnHeight / 4;

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);
                originalpositions.Add(child.Position);

                ++i;
            }

            var processed = new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));

            int j = 0;
            foreach (UnitBase u in unit)
                u.Position = processed[j++];

            unit.ChildrenDimensions = new Int2(width, columnHeight);
        }
    }
}