using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormations
{
    public class OrbFormation : ContuberniumFormationBase
    {
        private const float Angle = 40;
        private const float Radius = 2f;

        public override void Order(Contubernium unit, bool instant = false)
        {
            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (MeshDrawableUnit child in unit)
            {
                float x = spacing.x / 2 * Radius * Mathf.Cos(Angle * i);
                float z = spacing.y * Radius * Mathf.Sin(Angle * i);

                localPositions.Add(new Vector3(x, 0, z));

                child.Rotation = Quaternion.LookRotation(unit.Position - child.Position, Vector3.up);

                ++i;
            }

            ProcessLocalOffsets<Contubernium, MeshDrawableUnit>(localPositions, unit, instant);

            unit.ChildrenDimensions = new Int2(3, 3);
            unit.WalkSpeed = 0.3f;
        }
    }
}