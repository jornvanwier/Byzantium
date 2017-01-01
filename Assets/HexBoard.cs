using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.WSA;
using Worldgen;

namespace Assets
{
    internal enum TileType
    {
        Plains,
        Water,
        Desert
    }

    public struct AxialCoordinate
    {
        public AxialCoordinate(int q, int r)
        {
            Q = q;
            R = r;
        }

        public int Q;
        public int R;

        public static AxialCoordinate operator +(AxialCoordinate a, AxialCoordinate b)
        {
            return new AxialCoordinate(a.Q + b.Q, a.R + b.R);
        }

        public override string ToString()
        {
            return $"Q: {Q} R: {R}";
        }

        public static AxialCoordinate FromIndices(int x, int z)
        {
//            int col = x + (z + (z & 1)) / 2;
            int col = x - (z - (z & 1)) / 2;

            return new AxialCoordinate(col, z);
        }
    }

    /// <summary>
    /// Hex Board implementation using axial coordinates
    /// </summary>
    public class HexBoard
    {
        public Mesh DrawMesh { get; }
        private const byte NotFound = 255;

        public int Size { get; }

        public readonly AxialCoordinate[] Directions =
        {
            new AxialCoordinate(+1, 0), new AxialCoordinate(+1, -1), new AxialCoordinate(0, -1),
            new AxialCoordinate(-1, 0), new AxialCoordinate(-1, +1), new AxialCoordinate(0, +1)
        };

        private readonly byte[,] storage;

        public HexBoard(Mesh drawMesh, int size)
        {
            DrawMesh = drawMesh;
            Size = size;
            storage = new byte[Size, Size];
            Generate();
            TestShader();
        }

        public byte this[AxialCoordinate ac]
        {
            get { return this[ac.Q, ac.R]; }
            set { this[ac.Q, ac.R] = value; }
        }

        public byte this[int q, int r]
        {
            get
            {
                int x = q + (r - (r & 1)) / 2,
                    z = r;

                if (x < 0 || z < 0 || x >= Size || z >= Size)
                {
                    return NotFound;
                }
                return storage[x, z];
            }
            set
            {
                int x = q + (r - (r & 1)) / 2,
                    z = r;
//                Debug.Log($"Q: {x}, R: {z}");
                if (x < 0 || z < 0 || x >= Size || z >= Size)
                {
                    Debug.Log($"Attempted to access out of bounds: x: {x} z: {z}");
                    return;
                }
                storage[x, z] = value;
            }
        }

        public AxialCoordinate[] GetNeighbors(AxialCoordinate ac)
        {
            return Directions.Select(direction => ac + direction).ToArray();
        }

        public byte[] GetNeighborValues(AxialCoordinate ac)
        {
            return GetNeighbors(ac).Select(neighborAc => this[neighborAc]).Where(nb => nb != NotFound).ToArray();
        }

        private void InitMesh()
        {
        }

        private void Generate()
        {
            TileType[] tiles = (TileType[]) Enum.GetValues(typeof(TileType));

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    storage[x, y] = (byte) tiles[UnityEngine.Random.Range(0, tiles.Length)];
                }
            }

            Smooth();
        }

        private void Smooth(int iterations = 1, byte threshold = 3)
        {
            for (int i = 0; i < iterations; i++)
            {
                for (int x = 0; x < Size; x++)
                {
                    for (int z = 0; z < Size; z++)
                    {
                        AxialCoordinate currentPos = AxialCoordinate.FromIndices(x, z);
                        if (this[currentPos] == NotFound)
                        {
                            // Panic if the current position somehow isn't on the map
                            Debug.LogError("NO GOOD " + currentPos);
                        }
                        byte[] nearbyTiles = GetNeighborValues(currentPos);
//                        Debug.Log(string.Join("-", nearbyTiles.Select(b => b.ToString()).ToArray()));
                        IGrouping<byte, byte> found = nearbyTiles
                            .GroupBy(v => v)
                            .OrderByDescending(g => g.Count())
                            .FirstOrDefault(t => t.Count() > threshold && t.Key != NotFound);
                        if (found == null) continue;
                        byte tile = found.Key;

                        Debug.Log("changing");

                        storage[x, z] = tile;
                    }
                }
            }
        }

        private void TestShader()
        {
            ComputeBuffer gpuBuffer = new ComputeBuffer(Size * Size * sizeof(int), sizeof(int),
                ComputeBufferType.GPUMemory);

            int[,] data = new int[Size, Size];
            Array.Copy(storage, data, storage.Length);
            gpuBuffer.SetData(data);
            MaterialPropertyBlock b = new MaterialPropertyBlock();

            Material mat = Resources.Load("HexMat", typeof(Material)) as Material;

            b.SetFloat(Shader.PropertyToID("_ArraySize"), Size);

            b.SetBuffer(Shader.PropertyToID("hexProps"), gpuBuffer);

            GameObject thisObject = GameObject.Find("Core");
            // HexMesh = WorldGenerator.GenerateHexagonMesh(0.5f);


            MeshFilter f = thisObject.AddComponent<MeshFilter>();
            MeshRenderer r = thisObject.AddComponent<MeshRenderer>();
            f.sharedMesh = DrawMesh;
            r.material = mat;

            r.SetPropertyBlock(b);
        }

        public void LogString()
        {
            for (int row = 0; row < Size; row++)
            {
                StringBuilder sb = new StringBuilder();
                if ((row & 1) == 1)
                {
                    sb.Append(" ");
                }
                for (int col = 0; col < Size; col++)
                {
                    byte val = storage[col, row];
                    sb.Append(val == 120 ? "X" : val.ToString());
                }
                sb.Append("\n");
                Debug.Log(sb.ToString());
            }
        }
    }
}