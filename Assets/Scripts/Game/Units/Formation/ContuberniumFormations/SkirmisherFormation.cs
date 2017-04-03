using System.Collections.Generic;
using Assets.Scripts.Map;
using Game.Units;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormations
{
    public class SkirmisherFormation : ContuberniumFormationBase
    {
        private const int RowWidth = 4;
        private const int ColumnHeight = 2;

        public override void Order(Contubernium unit)
        {
            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (MeshDrawableUnit child in unit)
            {
                float x = spacing.x * 1.3f * (i % RowWidth) - spacing.x * RowWidth / 4;
                // ReSharper disable once PossibleLossOfFraction
                float z = spacing.y * 2 * (i / RowWidth) - spacing.y * ColumnHeight / 4;

                localPositions.Add(new Vector3(x, 0, z));

                ++i;
            }

            ProcessLocalOffsets<Contubernium, MeshDrawableUnit>(localPositions, unit);

            unit.ChildrenDimensions = new Int2(RowWidth * 2, ColumnHeight * 2);
            unit.WalkSpeed = 2.2f;
        }
    }
}