using System;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Map
{
    public class FoliageRenderer : MonoBehaviour
    {
        private readonly Dictionary<GameObject, List<Matrix4x4>> instanceMatrices =
            new Dictionary<GameObject, List<Matrix4x4>>();

        public List<GameObject> Fern;
        public List<GameObject> OakTree;

        public float OneTreePer = 100;
        public List<GameObject> PalmTree;
        public List<GameObject> PineTree;
        public List<GameObject> Rock;

        public void Start()
        {
        }

        public void Update()
        {
            foreach (KeyValuePair<GameObject, List<Matrix4x4>> instanceMatrix in instanceMatrices)
            {
                if (instanceMatrix.Value.Count <= 0) continue;

                GameObject obj = instanceMatrix.Key;
                List<Matrix4x4> matrices = instanceMatrix.Value;

                DrawGameObjectRecursively(obj, matrices.ToArray());
            }
        }

        private static void DrawGameObjectRecursively(GameObject gameObject, Matrix4x4[] sourceMatrices,
            int maxDepth = 2)
        {
            if (maxDepth <= 0)
                throw new ArgumentException("The given gameobject does not contain any drawable children");

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            var meshRenderer = gameObject.GetComponent<Renderer>();

            if (meshFilter != null && meshRenderer != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Material[] materials = meshRenderer.sharedMaterials;
                foreach (Material material in materials)
                    DrawMeshInstanced(mesh, material, sourceMatrices);
            }
            else
            {
                foreach (Transform childTransform in gameObject.transform)
                    DrawGameObjectRecursively(childTransform.gameObject, sourceMatrices, maxDepth--);
            }
        }

        private static void DrawMeshInstanced(Mesh mesh, Material material, Matrix4x4[] sourceMatrices)
        {
            if (material.enableInstancing)
            {
                const int maxCount = 1023;
                for (int i = 0; i < sourceMatrices.Length; i += maxCount)
                {
                    int remainingItems = sourceMatrices.Length - i;
                    int destinationSize = remainingItems % maxCount;

                    var destinationMatrices = new Matrix4x4[destinationSize];
                    Array.Copy(sourceMatrices, i, destinationMatrices, 0, destinationSize);

                    Graphics.DrawMeshInstanced(mesh, 0, material, destinationMatrices, destinationSize, null,
                        ShadowCastingMode.Off, false);
                }
            }
            else
            {
                throw new ArgumentException("Material does not enable instancing");
            }
        }

        public void AttachBoard(HexBoard board, MapRenderer mapRenderer)
        {
            for (int x = 0; x < board.Storage.GetLength(0); x++)
            for (int y = 0; y < board.Storage.GetLength(1); y++)
            {
                var tileType = (TileType) board.Storage[x, y];
                if (!(Random.Range(0, OneTreePer) < 1)) continue;

                Vector3 worldPos = mapRenderer.CubicalCoordinateToWorld(new OddRCoordinate(y, x).ToCubical());
                GameObject foliage = null;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (tileType)
                {
                    case TileType.Taiga:
                        foliage = PineTree.PickRandom();
                        break;
                    case TileType.TemperateDeciduousForest:
                        foliage = OakTree.PickRandom();
                        break;
                    case TileType.TemperateRainForest:
                        foliage = OakTree.PickRandom();
                        break;
                    case TileType.Bare:
                        foliage = Rock.PickRandom();
                        break;
                    case TileType.Scorched:
                        foliage = Rock.PickRandom();
                        break;
                    case TileType.GrassLand:
                        foliage = Fern.PickRandom();
                        break;
                    case TileType.TropicalRainForest:
                        foliage = PalmTree.PickRandom();
                        break;
                    case TileType.TropicalSeasonalForest:
                        foliage = PalmTree.PickRandom();
                        break;
                    case TileType.SubTropicalDesert:
                        foliage = PalmTree.PickRandom();
                        break;
                }

                if (foliage == null) continue;

                Matrix4x4 matrix = Matrix4x4.TRS(worldPos, foliage.transform.rotation, foliage.transform.localScale);

                if (!instanceMatrices.ContainsKey(foliage) || instanceMatrices[foliage] == null)
                    instanceMatrices[foliage] = new List<Matrix4x4>();

                instanceMatrices[foliage].Add(matrix);
            }
        }
    }
}