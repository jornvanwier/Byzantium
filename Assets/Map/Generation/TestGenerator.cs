using System;
using System.Collections.Generic;
using System.Linq;
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
            new TerrainType(TileType.Grass, 0.5f),
            new TerrainType(TileType.Forest, 0.6f),
            new TerrainType(TileType.Grass, 0.7f),
            new TerrainType(TileType.MountainLow, 0.75f),
            new TerrainType(TileType.MountainHigh, 0.87f),
            new TerrainType(TileType.MountainTop, 1f),
        };

        public byte[,] Generate(int size, float borderPercentage)
        {
            float startTime = Time.realtimeSinceStartup;

            float borderSize = borderPercentage * size;
            int seed = new Random().Next(0, 1000);
            seed = 767;
            Debug.Log(seed);
            float[,] heightMap = GenerateFloatMap(size, 0.7f, borderSize, false, 6, 0.55f, 2, new Vector2(), seed);

            float perlinTime = Time.realtimeSinceStartup;
            Debug.Log("Perlin Time: " + (perlinTime - startTime));

            byte[,] tileMap = GetTileMap(heightMap);

            float byteTime = Time.realtimeSinceStartup;
            Debug.Log("Byte Time: " + (byteTime - perlinTime));

            //            AddRivers(tileMap, heightMap, 3);

            float riverTime = Time.realtimeSinceStartup;
            Debug.Log("River Time: " + (riverTime - byteTime));

            int moistureResolution = 4;//moet factor van size zijn
            float[,] moistureMap = GetMoistureMap(tileMap, moistureResolution);
//            tileMap = GetTileMap(moistureMap); //uncomment to visualize moisturemap

            float moistTime = Time.realtimeSinceStartup;
            Debug.Log("Moist Time: " + (moistTime - riverTime));
            Debug.Log("Total Time: " + (riverTime - startTime));

            return tileMap;
        }

        private static T[,] ResizeMap<T>(T[,] map, int factor)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            T[,] bigMap = new T[width * factor, height * factor];


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    T value = map[x, y];
                    for (int i = 0; i < factor; i++)
                    {
                        for (int j = 0; j < factor; j++)
                        {
                            bigMap[x * factor + i, y * factor + j] = value;
                        }
                    }
                }
            }

            return bigMap;
        }

        private float[,] GetMoistureMap(byte[,] tileMap, int moistureResolution = 3, int waterResolution = 32)
        {
            int tileMapSize = tileMap.GetLength(0);
            List<Int2> beachWaterTiles = GetBeachWaterTiles(tileMap, tileMapSize / waterResolution);
            int moistureMapSize = tileMapSize / moistureResolution;
            float[,] moistureMap = new float[moistureMapSize, moistureMapSize];

            int numThreads = Environment.ProcessorCount;
            List<Thread> threads = new List<Thread>();

            long count = 0;

            for (int i = 0; i < numThreads; i++)
            {
                int partNum = i;
                Thread t = new Thread(() =>
                {
                    count += GenerateMoistureMapPart(ref moistureMap, 0, moistureMapSize / numThreads * partNum,
                        moistureMapSize,
                        moistureMapSize / numThreads * (partNum + 1),
                        tileMap, beachWaterTiles, moistureResolution
                    );
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Debug.Log("Checked distance " + count + " times");

            NormalizeMap(ref moistureMap);
            moistureMap = ResizeMap(moistureMap, moistureResolution);

            return moistureMap;
        }

        private long GenerateMoistureMapPart(ref float[,] moistureMap, int x1, int y1, int x2, int y2, byte[,] tileMap,
            List<Int2> beachWaterTiles, int moistureResolution)
        {
            long count = 0;
            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    Int2 currentPositionOnTileMap = new Int2(x, y) * moistureResolution;
                    TileType currentTile = (TileType)tileMap[currentPositionOnTileMap.x, currentPositionOnTileMap.y];
                    float moisture = 0;
                    if (currentTile != TileType.WaterDeep && currentTile != TileType.WaterShallow)
                    {
                        float closestWaterTile = float.PositiveInfinity;
                        foreach (Int2 waterTile in beachWaterTiles)
                        {
                            float distance = waterTile.Distance(currentPositionOnTileMap);
                            count++;
                            if (distance < closestWaterTile)
                                closestWaterTile = distance;
                        }
                        moisture = closestWaterTile;
                    }
                    moistureMap[x, y] = moisture;
                }
            }
            return count;
        }

        private List<Int2> GetBeachWaterTiles(byte[,] map, int resolution)
        {
            int size = map.GetLength(0);
            List<Int2> beachWaterTiles = new List<Int2>();
            for (int y = 0; y < size; y += resolution)
            {
                for (int x = 0; x < size; x += resolution)
                {
                    if (map[x, y] == (byte) TileType.WaterShallow)
                    {
                        Int2 currentTile = new Int2(x, y);
                        Int2[] neighbours = GetNeighbours(size, currentTile);
                        foreach (Int2 neighbour in neighbours)
                        {
                            TileType neighbourTile = (TileType) map[neighbour.x, neighbour.y];
                            if (neighbourTile == TileType.WaterDeep && neighbourTile == TileType.WaterShallow) continue;
                            beachWaterTiles.Add(currentTile);
                            break;
                        }
                    }
                }
            }
            return beachWaterTiles;
        }

        private void AddRivers(byte[,] tileMap, float[,] heightMap, int numRivers, int initialRiverWidth = 5)
        {
            int size = tileMap.GetLength(0);
            int failedRivers = 0;
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numRivers; ++i)
            {
                Thread t = new Thread(() =>
                {
                    Int2 startPos;
                    do
                    {
                        startPos = GetRiverStartPosition(size);
                    } while (tileMap[startPos.x, startPos.y] != (byte) TileType.WaterShallow);

                    List<Int2> river = GetRiver(size, startPos, heightMap);
                    if (river.Count == 0)
                        failedRivers++;

                    foreach (Int2 riverTile in river)
                    {
                        tileMap[riverTile.x, riverTile.y] = (byte) TileType.WaterDeep;
                    }
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            if (failedRivers > 0)
                Debug.LogWarning("Failed creating " + failedRivers + " out of " + numRivers + " rivers");
        }

        private List<Int2> GetRiver(int mapSize, Int2 startPos, float[,] heightMap)
        {
            List<Int2> river = new List<Int2> {startPos};

            if (AddRiverTile(mapSize, river, heightMap)) return river;

            river.RemoveAt(0);
            return river;
        }

        private bool AddRiverTile(int mapSize, List<Int2> river, float[,] heightMap)
        {
            Int2 currentTile = river.Last();
            float currentHeight = heightMap[currentTile.x, currentTile.y];

            if (currentHeight > Regions[Regions.Length - 2].Height)
                return true;

            IOrderedEnumerable<Int2> neighbours =
                GetNeighbours(mapSize, currentTile)
                    .Where(n => heightMap[n.x, n.y] >= currentHeight && !river.Contains(n))
                    //remove neighbours that are lower than current tile
                    .OrderBy(n => heightMap[n.x, n.y]); //Sort by tile height 

            foreach (Int2 neighbour in neighbours)
            {
                river.Add(neighbour);
                if (AddRiverTile(mapSize, river, heightMap))
                {
                    return true;
                }
                river.RemoveAt(river.Count - 1);
            }

            return false;
        }

        private static Int2[] GetNeighbours(int size, Int2 position)
        {
            int x = position.x;
            int y = position.y;
            int width = size, height = size;
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
            int x = random.Next(0, size);
            int y = random.Next(0, size);
            return new Int2(x, y);
        }

        private byte[,] GetTileMap(float[,] floatMap)
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

            NormalizeMap(ref map);

            return map;
        }

        private void NormalizeMap(ref float[,] map, float highestAllowedValue = 1)
        {
            int size = map.GetLength(0);

            float lowest = float.PositiveInfinity,
                highest = float.NegativeInfinity;
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