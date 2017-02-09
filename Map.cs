using System;
using System.Collections.Generic;

namespace HexBoard
{
    public class Map
    {
        private readonly byte[,] _map;
        private readonly int _size;

        public Map(int size)
        {
            _map = new byte[size,size];
            _size = size;
        }

        public void generateMap(int seed = 238947, float waterChance = 0.45f, float borderPercentage = 0.10f)
        {
            var randomizer = new System.Random(seed);

            for (int x = 0; x < _size; ++x)
            {
                for (int y = 0; y < _size; ++y)
                {
                    if ((x <= borderPercentage * _size || x >= _size - borderPercentage * _size) &&
                        (y <= borderPercentage * _size || y >= _size - borderPercentage * _size))
                    {
                        _map[x,y] = (byte)TileType.Water;
                        return;
                    }

                    int chance = (byte)randomizer.Next(0, 101);
                    if (chance < waterChance)
                        _map[x, y] = (byte)TileType.Water;
                    else
                        _map[x, y] = (byte)TileType.Grass;
                }
            }

        }

        public byte this[int x, int y, int z]
        {
            get
            {
                int col = x + (z - (z & 1)) / 2;
                int row = z;
                return _map[col,row];
            }
            set
            {
                {
                    int col = x + (z - (z & 1)) / 2;
                    int row = z;
                    _map[col, row] = value;
                }
            }
        }

        public bool checkCoordinate(int column, int row)
        {
            return column >= 0 && column <= _size && row >= 0 && row <= _size;
        }

        public bool checkCoordinate(int x, int y, int z)
        {
            int col = x + (z - (z & 1)) / 2;
            int row = z;
            return checkCoordinate(col, row);
        }

        public List<Tuple<int[],byte>> getNeighbours(int x, int y, int z)
        {
            List<Tuple<int[], byte>> neighbours = new List<Tuple<int[], byte>>();

            int[][] directions = {
                new[]{+1, -1,  0}, new[]{+1,  0, -1}, new[]{0, +1, -1},
                new[]{-1, +1,  0}, new[]{-1,  0, +1}, new[]{0, -1, +1}
            };

            foreach (var arr in directions)
            {
                if (!checkCoordinate(x + arr[0], y + arr[1], z + arr[2])) continue;

                var tuple = new Tuple<int[], byte>(arr, this[arr[0],arr[1], arr[2]]);
                neighbours.Add(tuple);
            }
            return neighbours;
        }

        public byte[,] getMapArray()
        {
            return _map;
        }


    }
}