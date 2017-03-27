using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Groups;
using Game.Units.Groups;
using UnityEngine;

namespace Game.Units.Formation
{
    public class SquareFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            throw new System.NotImplementedException();
        }

        public override void Order(Contubernium unit)
        {
            OrderAny<Contubernium,MeshDrawableUnit>(Mathf.Sqrt(3)/ 3, unit);
        }

        public override void Order(Cavalry unit)
        {
            throw new System.NotImplementedException();
        }

        public override void Order(Cohort unit)
        {
            throw new System.NotImplementedException();
        }

        private void OrderAny<T, TChild>(float spacing, T unit) where T : UnitBase, IMultipleUnits<TChild> where TChild : UnitBase
        {
            int rowWidth = (int) Mathf.Sqrt(unit.UnitCount);
            int columnHeight = unit.UnitCount - rowWidth;

            var localPositions = new List<Vector3>();
            var originalpositions = new List<Vector3>();

            int i = 0;

            foreach (UnitBase child in unit)
            {
                float x = unit.Position.x + spacing * (i % rowWidth) - (spacing * rowWidth / 4);
                // ReSharper disable once PossibleLossOfFraction
                float z = unit.Position.z + spacing * (i / rowWidth) - (spacing * columnHeight / 4);

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);
                originalpositions.Add(child.Position);

                ++i;
            }

            var processed = new List<Vector3>(ProcessLocalOffsets(originalpositions, localPositions, unit));

            int j = 0;
            foreach (UnitBase u in unit)
            {
                u.Position = processed[j++];
            }
        }
    }
}