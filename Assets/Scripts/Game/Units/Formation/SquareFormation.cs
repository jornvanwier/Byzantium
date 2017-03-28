using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
<<<<<<< HEAD
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Map;
=======
>>>>>>> b9b8552aeb9d5b715faef2872ff41b1430ed776f
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
            OrderAny<Contubernium, MeshDrawableUnit>(0.3f, unit);
        }

        public override void Order(Cavalry unit)
        {
            OrderAny<Cavalry, MeshDrawableUnit>(2, unit);
        }

        public override void Order(Cohort unit)
        {
            OrderAny<Cohort, Century>(3, unit);
        }

        public override void Order(Century unit)
        {
            OrderAny<Century, Contubernium>(1, unit);
        }

        private void OrderAny<T, TChild>(float spacing, T unit) where T : UnitBase, IMultipleUnits<TChild>
            where TChild : UnitBase
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount / rowWidth;

            var localPositions = new List<Vector3>();
            var originalpositions = new List<Vector3>();

            int i = 0;

            foreach (UnitBase child in unit)
            {
                float x = unit.Position.x + spacing * (i % rowWidth) - spacing * rowWidth / 4;
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing * (i / rowWidth) - spacing * columnHeight / 4;

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);
                originalpositions.Add(child.Position); 

                ++i;
            }

            var processed = new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));

            int j = 0;
            foreach (UnitBase u in unit) { 
                u.Position = processed[j++];
            }

            unit.ChildrenDimensions = new Int2(rowWidth, columnHeight);
        }
    }
}