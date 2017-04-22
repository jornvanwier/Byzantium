using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation.ContuberniumFormation
{
    public class OrbFormation : ContuberniumFormationBase
    {

        public override void Order(Contubernium unit, bool instant = false)
        {

            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            float angle = 360f / (unit.UnitCount + 1);
            float radius = 0.2f;

            foreach (MeshDrawableUnit child in unit)
            {
                float x = spacing.x * radius * Mathf.Cos(angle * i);
                float z = spacing.y * radius * Mathf.Sin(angle * i);

                localPositions.Add(new Vector3(x, 0, z));

                child.Rotation = Quaternion.LookRotation(unit.Position - child.Position, Vector3.up);

                ++i;
            }

            ProcessLocalOffsets<Contubernium, MeshDrawableUnit>(localPositions, unit, instant);

            int rows = Mathf.CeilToInt(Mathf.Sqrt(unit.UnitCount)) + 2;
            unit.ChildrenDimensions = new Int2(rows, rows);
            unit.WalkSpeed = 0.3f;
        }
    }
}