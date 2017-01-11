using System;
using System.Collections.Generic;
using Map.Generation;
using UnityEngine.VR.WSA.Persistence;

namespace Map
{
    internal enum TileType
    {
        Grass,
        Water,
        Desert
    }

    public class HexBoard
    {
        private const float BorderPercentage = 0.2f;

        private readonly int size;
        private IEnumerable<CubicalCoordinate> Directions { get; } = new[] {
            new CubicalCoordinate(+1, 0), new CubicalCoordinate(+1, -1), new CubicalCoordinate(0, -1),
            new CubicalCoordinate(-1, 0), new CubicalCoordinate(-1, +1), new CubicalCoordinate(0, +1)
        };

        public IMapGenerator Generator { get; set; }
        public byte[,] Storage { get; private set; }


        public HexBoard(int size)
        {
            this.size = size;
        }

        public void GenerateMap()
        {
            Storage = Generator.Generate(size, BorderPercentage);
        }

        public byte this[CubicalCoordinate cc]
        {
            get
            {
                OddRCoordinate oc = cc.ToOddR();
                return Storage[oc.Q,oc.R];
            }
            set
            {
                OddRCoordinate oc = cc.ToOddR();
                Storage[oc.Q, oc.R] = value;
            }
        }

        public bool CheckCoordinate(OddRCoordinate oc)
        {
            return oc.Q >= 0 && oc.Q <= size && oc.R >= 0 && oc.R <= size;
        }

        public bool CheckCoordinate(CubicalCoordinate cc)
        {
            return CheckCoordinate(cc.ToOddR());
        }

        public List<Tuple<CubicalCoordinate,byte>> GetNeighbours(CubicalCoordinate cc)
        {
            List<Tuple<CubicalCoordinate, byte>> neighbours = new List<Tuple<CubicalCoordinate, byte>>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (CubicalCoordinate direction in Directions)
            {
                CubicalCoordinate neighbour = cc + direction;
                if (!CheckCoordinate(neighbour)) continue;

                Tuple<CubicalCoordinate, byte> tuple = new Tuple<CubicalCoordinate, byte>(direction, this[neighbour]);
                neighbours.Add(tuple);
            }
            return neighbours;
        }


    }
}