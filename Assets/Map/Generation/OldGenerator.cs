using Assets.Map;
using UnityEngine;

namespace Map.Generation
{
    public class OldGenerator : IMapGenerator
    {
        public byte[,] Generate(int size, float borderPercentage)
        {
            byte[,] map = new byte[size, size];
            const float zoom = 5,
                landChance = 0.5f;
            float beachSize = 0.05f,
                seedX = Random.value * size * 2,
                seedY = Random.value * size * 2,
                center = (float)size / 2;
            seedX -= seedX / 2;
            seedY -= seedY / 2;

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    float distanceToCenter = Mathf.Sqrt(Mathf.Pow(x - center, 2) + Mathf.Pow(y - center, 2)) / ((float)size / 2),
                        height = Mathf.PerlinNoise(seedX + (float)x / size * zoom, seedY + y / (float)size * zoom);
                    if (distanceToCenter > 1 - borderPercentage)
                    {
                        //if point is too far from center: water
                        map[x, y] = (byte)TileType.WaterShallow;
                    }
                    else
                    {
                        if (height < landChance)
                        {
                            if (distanceToCenter > 1 - borderPercentage - beachSize)
                            {
                                //if on border of circle, and it would normally be grass: beach
                                map[x, y] = (byte)TileType.Beach;
                            }
                            else
                            {
                                map[x, y] = (byte)TileType.GrassLand;
                            }
                        }
                        else if (height < landChance + beachSize)
                        {
                            map[x, y] = (byte)TileType.Beach;
                        }
                        else
                        {
                            map[x, y] = (byte)TileType.WaterShallow;
                        }
                    }
                }
            }

            return map;
        }
    }
}