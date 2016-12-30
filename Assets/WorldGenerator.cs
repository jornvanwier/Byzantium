using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldgen
{
    class WorldGenerator
    {
        public static Mesh GenerateCircleMesh(float radius, int segments)
        {
            Mesh m = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            float AngleIncrement = (Mathf.PI * 2.0f) / segments;

            Vector3 InnerVec = Vector3.zero;
            verts.Add(InnerVec);

            for (int i = 0; i < segments; ++i)
            {
                float angle = AngleIncrement * i;

                Vector3 pos = new Vector3();
                pos.x = Mathf.Cos(angle);
                pos.y = Mathf.Sin(angle);
                Vector3 result = pos * radius;
                result.z = result.y;
                result.y = 0;
                verts.Add(result);
            }

            for (int i = 0; i < segments - 1; ++i)
            {
                tris.Add(0);
                tris.Add(i + 2);
                tris.Add(i + 1);
            }

            tris.Add(0);
            tris.Add(1);
            tris.Add(segments);

            m.vertices = verts.ToArray();
            m.triangles = tris.ToArray();
            m.RecalculateNormals();
            ;
            return m;
        }

        public static Mesh GenerateHexagonMesh(float radius)
        {
            return GenerateCircleMesh(radius, 6);
        }


    }
}
