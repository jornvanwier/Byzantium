using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Map.Generation;

namespace Map
{
    public class MapRenderer : MonoBehaviour
    {
        private ComputeBuffer computeBuffer;
        private HexBoard hexBoard;

        public int MapSize = 1;
        public Mesh Mesh;

        public Material HexMaterial;

        [UsedImplicitly]
        private void Start()
        {
            hexBoard = new HexBoard(MapSize) {Generator = new TestGenerator()};
            hexBoard.GenerateMap();

            CubicalCoordinate start = hexBoard.RandomValidTile();

            CubicalCoordinate goal = hexBoard.RandomValidTile();

            List<CubicalCoordinate> path = hexBoard.FindPath(start, goal);

            if (path != null)
            {
                foreach (CubicalCoordinate hex in path)
                {
                    hexBoard[hex] = (byte) TileType.Path;
                }
            }


            hexBoard[start] = (byte) TileType.Path;
            hexBoard[goal] = (byte) TileType.Path;

            SetupShader();
        }

        private void SetupShader()
        {
            computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.GPUMemory);

            int[,] data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            {
                for (int y = 0; y < MapSize; ++y)
                {
                    data[x, y] = hexBoard.Storage[x, y];
                }
            }

            computeBuffer.SetData(data);

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
            block.SetBuffer(Shader.PropertyToID("hexProps"), computeBuffer);

            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer mrenderer = gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = Mesh;
            mrenderer.material = HexMaterial;
            mrenderer.SetPropertyBlock(block);
        }

        [UsedImplicitly]
        private void Update()
        {
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            computeBuffer?.Dispose();
        }
    }
}