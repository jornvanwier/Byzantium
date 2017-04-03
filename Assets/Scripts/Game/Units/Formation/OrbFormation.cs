using System.Collections.Generic;
using Game.Units;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public class OrbFormation : ContuberniumFormationBase
    {
        private const float Angle = 360 / 8f;
        private const float Radius = 0.02f;

        public override void Order(Contubernium unit)
        {
            var localPositions = new List<Vector3>();

            int i = 0;

            Vector2 spacing = unit.DrawSize;

            foreach (MeshDrawableUnit child in unit)
            {
                float x = Radius * Mathf.Cos(Angle * i);
                float z = Radius * Mathf.Sin(Angle * i);

                localPositions.Add(new Vector3(x, unit.Position.y, z) - unit.Position);

                ++i;
            }
        }
    }
}