using UnityEngine;
using Worldgen;
using System.Collections.Generic;
using JetBrains.Annotations;


public class Generator2 : MonoBehaviour
{

    List<GameObject> HexagonGameObjects;

    ComputeBuffer gpuBuffer;

    Material mat;
    byte[,] Hexes;
    Vector3[] positions;
    public Mesh HexMesh;
    MaterialPropertyBlock b;

    GameObject thisObject;

    public int size = 1;

    [Range(0, 100)]
    public int WaterChance = 45;
    [Range(0, 25)]
    public int iterations = 1;


    // Use this for initialization
    [UsedImplicitly]
    private void Start()
    {
        Hexes = new byte[size, size];
        positions = new Vector3[size * size];
        System.Random ra = new System.Random(238947);

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                int chance = (byte)ra.Next(0, 101);
                if (chance < WaterChance)
                    Hexes[x, y] = (byte)TileType.Water;
                else
                    Hexes[x, y] = (byte)TileType.Grass;
            }
        }

        for (int i = 0; i < iterations; ++i)
        {
            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    int nghc = GetNeighbourWaterTileCount(x, y);
                    if (nghc > 4)
                    {
                        //Water
                        Hexes[x, y] = (byte)TileType.Water;
                    }
                    else
                    {
                        //Land
                        if (nghc < 4)
                        {
                            Hexes[x, y] = (byte)TileType.Grass;
                        }
                        else
                        {
                            Hexes[x, y] = (byte)ra.Next(0, 2);
                        }
                    }
                }
            }
        }

        gpuBuffer = new ComputeBuffer(size * size * sizeof(int), sizeof(int), ComputeBufferType.GPUMemory);

        int[,] data = new int[size, size];

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                data[x, y] = Hexes[x, y];
            }
        }

        gpuBuffer.SetData(data);
        b = new MaterialPropertyBlock();

        HexagonGameObjects = new List<GameObject>();
        mat = Resources.Load("HexMat", typeof(Material)) as Material;

        b.SetFloat(Shader.PropertyToID("_ArraySize"), size);

        b.SetBuffer(Shader.PropertyToID("hexProps"), gpuBuffer);

        thisObject = GameObject.Find("Core");
        // HexMesh = WorldGenerator.GenerateHexagonMesh(0.5f);


        MeshFilter f = thisObject.AddComponent<MeshFilter>();
        MeshRenderer r = thisObject.AddComponent<MeshRenderer>();
        f.sharedMesh = HexMesh;
        r.material = mat;

        r.SetPropertyBlock(b);
    }

    private int GetNeighbourWaterTileCount(int x, int y)
    {
        int nCount = 0;
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0)
                    continue;
                //Bounds Checking
                if (x + i < 0 || x + i >= size || y + j < 0 || y + j >= size) continue;
                if (Hexes[x + i, y + j] == (byte)TileType.Water)
                {
                    nCount++;
                }
            }
        }
        return nCount;
    }

    // Update is called once per frame
    [UsedImplicitly]
    private void Update()
    {
        //gpuBuffer.SetData(Hexes);
    }
}
