using UnityEngine;

namespace Assets
{
    public static class PrimitiveMesh
    {
        public static Mesh Hexagon()
        {
            #region verts

            const float floorLevel = 0;
            Vector3[] vertices = {
                new Vector3(-1f , floorLevel, -.5f),
                new Vector3(-1f, floorLevel, .5f),
                new Vector3(0f, floorLevel, 1f),
                new Vector3(1f, floorLevel, .5f),
                new Vector3(1f, floorLevel, -.5f),
                new Vector3(0f, floorLevel, -1f)
            };

            #endregion

            #region triangles

            int[] triangles = {
                1,5,0,
                1,4,5,
                1,2,4,
                2,3,4
            };

            #endregion

            #region UV

            Vector2[] uv = {
                new Vector2(0,0.25f),
                new Vector2(0,0.75f),
                new Vector2(0.5f,1),
                new Vector2(1,0.75f),
                new Vector2(1,0.25f),
                new Vector2(0.5f,0),
            };

            #endregion

            #region finalize

            //create a mesh object to pass our data into
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uv
            };

            //make it play nicely with lighting
            mesh.RecalculateNormals();

            #endregion

            return mesh;
        }
    }
}