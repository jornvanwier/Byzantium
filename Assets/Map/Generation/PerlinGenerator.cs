using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Map.Generation;
using UnityEngine;
using Random = System.Random;

namespace Assets.Map.Generation
{
    public class PerlinGenerator : IMapGenerator
    {
        private const float WaterHeight = 0.4f;
        private const float RiverStartHeight = 0.8f;
        private const float Scale = 0.7f;
        private const int Octaves = 6;
        private const float Persistance = 0.55f;
        private const float Lacunarity = 2f;
        private const bool SquareBorder = false;

        private static readonly ElevationLevel[] ElevationLevels =
        {
            new ElevationLevel(0.3f,
                new Biome(TileType.WaterDeep, 1f)),
            new ElevationLevel(0.4f,
                new Biome(TileType.WaterShallow, 1f)),
            new ElevationLevel(0.45f,
                new Biome(TileType.Beach, 1f)),
            new ElevationLevel(0.625f,
                new Biome(TileType.SubTropicalDesert, 0.25f),
                new Biome(TileType.GrassLand, 0.4f),
                new Biome(TileType.TropicalSeasonalForest, 0.7f),
                new Biome(TileType.TropicalRainForest, 1f)),
            new ElevationLevel(0.8f,
                new Biome(TileType.TemperateDesert, 0.25f),
                new Biome(TileType.GrassLand, 0.6f),
                new Biome(TileType.TemperateDeciduousForest, 0.85f),
                new Biome(TileType.TemperateRainForest, 1f)),
            new ElevationLevel(0.9f,
                new Biome(TileType.TemperateDesert, 0.425f),
                new Biome(TileType.Shrubland, 0.7f),
                new Biome(TileType.Taiga, 1f)),
            new ElevationLevel(1f,
                new Biome(TileType.Scorched, 0.25f),
                new Biome(TileType.Bare, 0.425f),
                new Biome(TileType.Tundra, 0.6f),
                new Biome(TileType.Snow, 1f))
        };

        public byte[,] Generate(int size, float borderPercentage)
        {
            int seed = new Random().Next(0, 1000);
            Debug.Log(seed);
            float borderSize = borderPercentage * size;
            int moistureResolution = size / 1024; //moet factor van size zijn, hoger is preciezer en trager
            if (moistureResolution == 0) moistureResolution = 1;

            byte[,] tileMap = null;
            Utils.LogOperationTime("Total map generation", () =>
            {
                float[,] heightMap = Utils.LogOperationTime("Heightmap",
                    () =>
                        GenerateFloatMap(size, Scale, borderSize, SquareBorder, Octaves, Persistance, Lacunarity,
                            new Vector2(), seed));

                bool[,] waterMap = Utils.LogOperationTime("Watermap", () => GetWaterMap(heightMap));

//                Utils.LogOperationTime("Rivers", () => AddRivers(waterMap, heightMap, 3));

                float[,] moistureMap = Utils.LogOperationTime("Moisturemap",
                    () => GetMoistureMap(waterMap, moistureResolution));

                tileMap = Utils.LogOperationTime("Tilemap", () => GetTileMap(heightMap, moistureMap));
            });

            return tileMap;
        }

        private byte[,] GetTileMap(float[,] heightMap, float[,] moistureMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            byte[,] map = new byte[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float currentHeight = heightMap[x, y];
                    float currentMoisture = moistureMap[x, y];
                    foreach (ElevationLevel elevation in ElevationLevels)
                    {
                        if (currentHeight <= elevation.Height)
                        {
                            foreach (Biome biome in elevation.Biomes)
                            {
                                if (currentMoisture <= biome.Moisture)
                                {
                                    map[x, y] = (byte) biome.Type;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return map;
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

        private float[,] GetMoistureMap(bool[,] watermap, int moistureResolution = 1, int waterResolution = 32)
        {
            int tileMapSize = watermap.GetLength(0);
            List<Int2> beachWaterTiles = GetBeachWaterTiles(watermap, tileMapSize / waterResolution);
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
                        watermap, beachWaterTiles, moistureResolution
                    );
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            PerlinizeMap(ref moistureMap, 0.2f);
            NormalizeMap(ref moistureMap);
            InvertMap(ref moistureMap);
            moistureMap = ResizeMap(moistureMap, moistureResolution);

            return moistureMap;
        }

        private long GenerateMoistureMapPart(ref float[,] moistureMap, int x1, int y1, int x2, int y2, bool[,] waterMap,
            List<Int2> beachWaterTiles, int moistureResolution)
        {
            long count = 0;
            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    Int2 currentPositionOnTileMap = new Int2(x, y) * moistureResolution;
                    bool isWater = waterMap[currentPositionOnTileMap.X, currentPositionOnTileMap.Y];
                    float moisture = 0;
                    if (!isWater)
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

        private List<Int2> GetBeachWaterTiles(bool[,] waterMap, int resolution)
        {
            int size = waterMap.GetLength(0);
            List<Int2> beachWaterTiles = new List<Int2>();
            for (int y = 0; y < size; y += resolution)
            {
                for (int x = 0; x < size; x += resolution)
                {
                    if (waterMap[x, y])
                    {
                        Int2 currentTile = new Int2(x, y);
                        Int2[] neighbours = GetNeighbours(size, currentTile);
                        foreach (Int2 neighbour in neighbours)
                        {
                            bool neighbourIsWater = waterMap[neighbour.X, neighbour.Y];
                            if (neighbourIsWater) continue;
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
                    } while (tileMap[startPos.X, startPos.Y] != (byte) TileType.WaterShallow);

                    List<Int2> river = GetRiver(size, startPos, heightMap);
                    if (river.Count == 0)
                        failedRivers++;

                    foreach (Int2 riverTile in river)
                    {
                        tileMap[riverTile.X, riverTile.Y] = (byte) TileType.WaterDeep;
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
            float currentHeight = heightMap[currentTile.X, currentTile.Y];

            if (currentHeight > RiverStartHeight)
                return true;

            IOrderedEnumerable<Int2> neighbours =
                GetNeighbours(mapSize, currentTile)
                    .Where(n => heightMap[n.X, n.Y] >= currentHeight && !river.Contains(n))
                    //remove neighbours that are lower than current tile
                    .OrderBy(n => heightMap[n.X, n.Y]); //Sort by tile height

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
            int x = position.X;
            int y = position.Y;
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

        private bool[,] GetWaterMap(float[,] floatMap)
        {
            int size = (int) Mathf.Sqrt(floatMap.Length);
            bool[,] map = new bool[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isWater = floatMap[x, y] <= WaterHeight;
                    map[x, y] = isWater;
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

            //fill moistureMap
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

        private void PerlinizeMap(ref float[,] map, float perlinFactor = 0.1f)
        {
            int size = map.GetLength(0);
            float scale = Scale * size / 4;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float mapX = x / scale;
                    float mapY = y / scale;
                    float value = Mathf.PerlinNoise(mapX, mapY) * (1 + perlinFactor);
                    map[x, y] *= value;
                }
            }
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

        private void InvertMap(ref float[,] map, float max = 1)
        {
            int size = map.GetLength(0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    map[x, y] = max - map[x, y];
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