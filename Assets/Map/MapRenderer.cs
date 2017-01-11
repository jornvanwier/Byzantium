using System;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Map.Generation;
using UnityEngine;

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
            hexBoard = new HexBoard(MapSize) {Generator = new SquareGenerator()};
            hexBoard.GenerateMap();

            CubicalCoordinate start = new CubicalCoordinate(20, 30);
            hexBoard[start] = (byte) TileType.Desert;

            foreach (Tuple<CubicalCoordinate, byte> tuple in hexBoard.GetNeighbours(start))
            {
                hexBoard[start + tuple.Item1] = (byte) TileType.Water;
            }

//            hexBoard[start + new CubicalCoordinate(0, -1)] = (byte) TileType.Water;

            SetupShader(hexBoard.Storage);
        }

        private void SetupShader(byte[,] map)
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
    }
}