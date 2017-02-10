using System;
using System.Collections.Generic;
using System.Threading;
using Map;
using Map.Generation;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace Assets.Map.Generation
{
    public class TestGenerator : IMapGenerator
    {
        public byte[,] Generate(int size, float borderPercentage)
        {
            float borderSize = borderPercentage * size;
            int seed = new Random().Next(0, 1000);
            Debug.Log(seed);

            float startTime = Time.realtimeSinceStartup;
            float[] map = GenerateFloatMap(size, 1, borderSize, false, 6, 0.55f, 2, new Vector2(), seed);
            float perlinTime = Time.realtimeSinceStartup;
            Debug.Log("Perlin Time: " + (perlinTime - startTime));
            byte[,] byteMap = FloatToByteMap(map);
            float endTime = Time.realtimeSinceStartup;
            Debug.Log("Byte Time: " + (endTime - perlinTime));
            Debug.Log("Total Time: " + (endTime - startTime));

            return byteMap;
        }

        private byte[,] FloatToByteMap(float[] floatMap)
        {
            int size = (int) Mathf.Sqrt(floatMap.Length);
            byte[,] map = new byte[size, size];

            TerrainType[] regions =
            {
                new TerrainType(TileType.WaterDeep, 0.3f),
                new TerrainType(TileType.WaterShallow, 0.4f),
                new TerrainType(TileType.Beach, 0.45f),
                new TerrainType(TileType.Grass, 0.55f),
                new TerrainType(TileType.Forest, 0.6f),
                new TerrainType(TileType.MountainLow, 0.7f),
                new TerrainType(TileType.MountainHigh, 0.9f),
                new TerrainType(TileType.MountainTop, 1f),
            };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float currentHeight = floatMap[y * size + x];
                    foreach (var region in regions)
                    {
                        if (!(currentHeight <= region.Height)) continue;
                        map[x, y] = (byte) region.Type;
                        break;
                    }
                }
            }

            return map;
        }

        private float[] GenerateFloatMap(int size, float scale, float borderSize, bool squareBorder, int octaves,
            float persistance, float lacunarity, Vector2 position, int seed)
        {
            scale *= (float) size / 2; //adjust scale so size is irrelavant

            //clean input
            scale = Mathf.Clamp(scale, 0.000001f, float.PositiveInfinity);
            octaves = Mathf.Clamp(octaves, 0, 10);
            persistance = Mathf.Clamp(persistance, 0, 1);
            lacunarity = Mathf.Clamp(lacunarity, 0, 2);

            //fill map
            float[] map = new float[size * size];
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

            List<float[]> mapParts = new List<float[]>();
            int numThreads = Environment.ProcessorCount;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                Thread t = new Thread(() =>
                {
                    mapParts.Add(GenerateMapPart(0, size / numThreads * i, size, size / numThreads * i + 1, squareBorder,
                        size, borderSize,
                        center,
                        halfSize,
                        persistance,
                        lacunarity, octaves, scale, octaveOffsets, position));
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            int index = 0;
            foreach (float[] mapPart in mapParts)
            {
                mapPart?.CopyTo(map, index += mapPart.Length);
            }

            float lowest = float.PositiveInfinity,
                highest = float.NegativeInfinity,
                highestAllowedValue = 1;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (map[y * size + x] < lowest)
                    {
                        lowest = map[y * size + x];
                    }
                    if (map[y * size + x] > highest)
                    {
                        highest = map[y * size + x];
                    }
                }
            }

            float multiplier = highestAllowedValue / (highest - lowest);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    map[y * size + x] -= lowest;
                    map[y * size + x] *= multiplier;
                }
            }

            return map;
        }

        public float[] GenerateMapPart(int x1, int y1, int x2, int y2, bool squareBorder, float size,
            float borderSize,
            Vector2 center, float halfSize, float persistance, float lacunarity, int octaves, float scale,
            Vector2[] octaveOffsets, Vector2 position)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            float[] mapPart = new float[width * height];
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


                    mapPart[(y - y1) * width + x] = noiseHeight;
                }
            }

            return mapPart;
        }
    }
}