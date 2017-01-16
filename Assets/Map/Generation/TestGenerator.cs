using Map;
using Map.Generation;
using UnityEngine;
using Random = System.Random;

namespace Assets.Map.Generation
{
    public class TestGenerator : IMapGenerator
    {
        public byte[,] Generate(int size, float borderPercentage)
        {
            //variables
            float scale = 100;//Hoger is meer zoom in
            int octaves = 4;
            float persistance = 0.5f;
            float lacunarity = 2f;
            float landChance = 1f;
            Vector2 position = new Vector2(0, 0);
            int seed = 5020;

            //clean input
            scale = Mathf.Clamp(scale, 0.000001f, float.PositiveInfinity);
            lacunarity = Mathf.Clamp(lacunarity, 1, float.PositiveInfinity);
            persistance = Mathf.Clamp(persistance, 0, 1);

            //fill map
            byte[,] map = new byte[size, size];
            Random rando = new Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rando.Next(-100000, 100000);
                float offsetY = rando.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float halfSize = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float mapX = (x - halfSize) / scale * frequency + octaveOffsets[i].x + position.x;
                        float mapY = (y - halfSize) / scale * frequency + octaveOffsets[i].y + position.y;
                        float value = Mathf.PerlinNoise(mapX, mapY);
                        noiseHeight += value * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    map[x, y] = (byte)(noiseHeight > landChance ? TileType.Grass : TileType.Water);
                }
            }

            return map;
        }
    }
}