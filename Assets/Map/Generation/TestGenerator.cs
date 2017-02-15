using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Map;
using Map.Generation;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using Random = System.Random;

namespace Assets.Map.Generation
{
    public class TestGenerator : IMapGenerator
    {
        private static readonly TerrainType[] Regions =
        {
            new TerrainType(TileType.WaterDeep, 0.3f),
            new TerrainType(TileType.WaterShallow, 0.4f),
            new TerrainType(TileType.Beach, 0.45f),
            new TerrainType(TileType.Grass, 0.55f),
            new TerrainType(TileType.Forest, 0.6f),
            new TerrainType(TileType.Grass, 0.7f),
            new TerrainType(TileType.MountainLow, 0.75f),
            new TerrainType(TileType.MountainHigh, 0.87f),
            new TerrainType(TileType.MountainTop, 1f),
        };

        public byte[,] Generate(int size, float borderPercentage)
        {
            float borderSize = borderPercentage * size;
            int seed = new Random().Next(0, 1000);
            Debug.Log(seed);

            float startTime = Time.realtimeSinceStartup;
            float[,] heightMap = GenerateFloatMap(size, 0.7f, borderSize, false, 6, 0.55f, 2, new Vector2(), seed);
            float perlinTime = Time.realtimeSinceStartup;

            Debug.Log("Perlin Time: " + (perlinTime - startTime));

            byte[,] tileMap = FloatToByteMap(heightMap);
//            AddRivers(ref tileMap, heightMap, 3);
            float endTime = Time.realtimeSinceStartup;

            Debug.Log("Byte Time: " + (endTime - perlinTime));
            Debug.Log("Total Time: " + (endTime - startTime));

            return tileMap;
        }

        private void AddRivers(ref byte[,] tileMap, float[,] heightMap, int numRivers, int initialRiverWidth = 5)
        {
            int size = tileMap.GetLength(0);
            for (int i = 0; i < numRivers; ++i)
            {
                Int2 startPos = GetRiverStartPosition(size);
                List<Int2> river = GetRiver(size, startPos, heightMap);
                foreach (Int2 riverTile in river)
                {
                    tileMap[riverTile.x, riverTile.y] = (byte) TileType.Desert;
                }
            }
        }

        private List<Int2> GetRiver(int mapSize, Int2 startPos, float[,] heightMap)
        {
            float stopHeight = Regions[0].Height;
            //rivier stroomt door tot hij diep water vindt

            List<Int2> river = new List<Int2> {startPos};
            Int2[] neighbours = GetNeighbours(mapSize, mapSize, startPos.x, startPos.y);
            float lowestNeighbour = heightMap[neighbours[0].x, neighbours[0].y];
            float prevHeight = -1;

            while (lowestNeighbour >= stopHeight)
            {
                lowestNeighbour = heightMap[neighbours[0].x, neighbours[0].y];
                Int2 lowestNeighbourIndex = neighbours[0];
                foreach (Int2 neighbour in neighbours)
                {
                    float neighbourHeight = heightMap[neighbour.x, neighbour.y];
                    if (neighbourHeight < lowestNeighbour)
                    {
                        lowestNeighbour = neighbourHeight;
                        lowestNeighbourIndex = neighbour;
                    }
                }

                if (lowestNeighbour > prevHeight ||
                    lowestNeighbourIndex.x == startPos.x && lowestNeighbourIndex.y == startPos.y)
                {
                    lowestNeighbourIndex = neighbours[new Random().Next(neighbours.Length)];
                }

                neighbours = GetNeighbours(mapSize, mapSize, lowestNeighbourIndex.x, lowestNeighbourIndex.y);

                if (river.Count > 100)
                    Debug.Log("Large river");

                if(!river.Contains(lowestNeighbourIndex))
                    river.Add(lowestNeighbourIndex);

                prevHeight = lowestNeighbour;
                startPos = lowestNeighbourIndex;
            }

            return river;
        }

        private static Int2[] GetNeighbours(int width, int height, int x, int y)
        {
            if (x >= width || y >= height || x < 0 || y < 0)
                throw new ArgumentException("Requested position is out of bounds");

            if (x + 1 == width) //right boundary
            {
                if (y + 1 == height) //bottom boundary
                {
                    return new[] {new Int2(x, y - 1), new Int2(x - 1, y)};
                }
                if (y == 0) //top boundary
                {
                    return new[] {new Int2(x, y + 1), new Int2(x - 1, y)};
                }
                return new[] {new Int2(x, y + 1), new Int2(x - 1, y), new Int2(x, y - 1)};
            }
            if (x == 0) //left boundary
            {
                if (y + 1 == height) //bottom boundary
                {
                    return new[] {new Int2(x, y - 1), new Int2(x + 1, y)};
                }
                if (y == 0) //top boundary
                {
                    return new[] {new Int2(x, y + 1), new Int2(x + 1, y)};
                }
                return new[] {new Int2(x, y + 1), new Int2(x + 1, y), new Int2(x, y - 1)};
            }
            if (y + 1 == height) //bottom boundary
            {
                return new[] {new Int2(x + 1, y), new Int2(x, y - 1), new Int2(x - 1, y)};
            }
            if (y == 0) //top boundary
            {
                return new[] {new Int2(x, y + 1), new Int2(x + 1, y), new Int2(x - 1, y)};
            }
            return new[] {new Int2(x, y + 1), new Int2(x + 1, y), new Int2(x, y - 1), new Int2(x - 1, y)};
        }

        private Int2 GetRiverStartPosition(int size)
        {
            Random random = new Random();

            return new Int2(random.Next(size / 4, size - size / 4), random.Next(size / 4, size - size / 4));
            ;
        }

        private byte[,] FloatToByteMap(float[,] floatMap)
        {
            int size = (int) Mathf.Sqrt(floatMap.Length);
            byte[,] map = new byte[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float currentHeight = floatMap[x, y];
                    foreach (var region in Regions)
                    {
                        if (!(currentHeight <= region.Height)) continue;
                        map[x, y] = (byte) region.Type;
                        break;
                    }
                }
            }

            return map;
        }

        private float[,] GenerateFloatMap(int size, float scale, float borderSize, bool squareBorder, int octaves,
            float persistance, float lacunarity, Vector2 position, int seed)
        {
            scale *= (float) size / 4; //adjust scale so size is irrelavant

            //clean input
            scale = Mathf.Clamp(scale, 0.000001f, float.PositiveInfinity);
            octaves = Mathf.Clamp(octaves, 0, 10);
            persistance = Mathf.Clamp(persistance, 0, 1);
            lacunarity = Mathf.Clamp(lacunarity, 0, 2);

            //fill map
            float[,] map = new float[size, size];
            Random random = new Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-10000, 10000);
                float offsetY = random.Next(-10000, 10000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float halfSize = size / 2f;

            int numThreads = Environment.ProcessorCount;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int partNum = i;
                Thread t = new Thread(() =>
                {
                    GenerateMapPart(ref map, 0, size / numThreads * partNum, size, size / numThreads * (partNum + 1),
                        squareBorder,
                        size, borderSize,
                        center,
                        halfSize,
                        persistance,
                        lacunarity, octaves, scale, octaveOffsets, position);
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            float lowest = float.PositiveInfinity,
                highest = float.NegativeInfinity;
            const float highestAllowedValue = 1;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (map[x, y] < lowest)
                    {
                        lowest = map[x, y];
                    }
                    if (map[x, y] > highest)
                    {
                        highest = map[x, y];
                    }
                }
            }

            float multiplier = highestAllowedValue / (highest - lowest);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    map[x, y] -= lowest;
                    map[x, y] *= multiplier;
                }
            }

            return map;
        }

        public void GenerateMapPart(ref float[,] map, int x1, int y1, int x2, int y2, bool squareBorder, float size,
            float borderSize,
            Vector2 center, float halfSize, float persistance, float lacunarity, int octaves, float scale,
            Vector2[] octaveOffsets, Vector2 position)
        {
            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    float heightModifier, amplitudeModifier;
                    if (squareBorder)
                    {
                        amplitudeModifier = Mathf.Min(x, y, size - x, size - y, borderSize) / borderSize;
                        heightModifier = Mathf.Min(x, y, size - x, size - y, borderSize / 2) / (borderSize / 2);
                    }
                    else
                    {
                        float distanceToCenter = Vector2.Distance(center, new Vector2(x, y));
                        float distanceToBorder = Mathf.Max(0, halfSize - distanceToCenter);
                        heightModifier = Mathf.Min(distanceToBorder, borderSize) / borderSize;
                        amplitudeModifier = Mathf.Min(distanceToBorder, borderSize / 2) / (borderSize / 2);
                    }

                    for (int i = 0; i < octaves; i++)
                    {
                        float mapX = (x - halfSize) / scale * frequency + octaveOffsets[i].x + position.x;
                        float mapY = (y - halfSize) / scale * frequency + octaveOffsets[i].y + position.y;
                        float value = Mathf.PerlinNoise(mapX, mapY);
                        noiseHeight += value * amplitude * heightModifier;

                        amplitude *= persistance * amplitudeModifier;
                        frequency *= lacunarity;
                    }


                    map[x, y] = noiseHeight;
                }
            }
        }
    }
}