using System;
using System.Collections.Generic;
using Map.Generation;
using Priority_Queue;
using Random = UnityEngine.Random;

namespace Map
{
    public enum TileType
    {
        WaterDeep,
        WaterShallow,
        Grass,
        Forest,
        MountainLow,
        MountainHigh,
        MountainTop,
        Beach,
        Desert,
        Path
    }

    public class HexBoard
    {
        private const float BorderPercentage = 0.2f;

        private readonly int size;

        private IEnumerable<CubicalCoordinate> Directions { get; } = new[]
        {
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
                return Storage[oc.R, oc.Q];
            }
            set
            {
                OddRCoordinate oc = cc.ToOddR();
                Storage[oc.R, oc.Q] = value;
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

        public CubicalCoordinate RandomValidTile()
        {
            CubicalCoordinate cc;
            do
            {
                cc = new OddRCoordinate(Random.Range(0, size), Random.Range(0, size)).ToCubical();
            } while (this[cc] == (byte) TileType.WaterShallow);

            return cc;
        }

        public List<Tuple<CubicalCoordinate, byte>> GetNeighbours(CubicalCoordinate cc)
        {
            List<Tuple<CubicalCoordinate, byte>> neighbours = new List<Tuple<CubicalCoordinate, byte>>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (CubicalCoordinate direction in Directions)
            {
                CubicalCoordinate neighbour = cc + direction;
                if (!CheckCoordinate(neighbour)) continue;

                Tuple<CubicalCoordinate, byte> tuple = new Tuple<CubicalCoordinate, byte>(neighbour, this[neighbour]);
                neighbours.Add(tuple);
            }
            return neighbours;
        }

        // TODO Replace start with unit or legion
        public List<CubicalCoordinate> FindPath(CubicalCoordinate start, CubicalCoordinate goal)
        {
            List<CubicalCoordinate> closedSet = new List<CubicalCoordinate>();

            Dictionary<CubicalCoordinate, CubicalCoordinate> cameFrom =
                new Dictionary<CubicalCoordinate, CubicalCoordinate>();

            Dictionary<CubicalCoordinate, float> gScore = new Dictionary<CubicalCoordinate, float>
            {
                [start] = 0
            };
//
//            Dictionary<CubicalCoordinate, float> fScore = new Dictionary<CubicalCoordinate, float>
//            {
//                [start] = CubicalCoordinate.DistanceBetween(start, goal)
//            };

            SimplePriorityQueue<CubicalCoordinate> queue = new SimplePriorityQueue<CubicalCoordinate>();
            queue.Enqueue(start, 0);

            while (queue.Count > 0)
            {
                CubicalCoordinate current = queue.Dequeue();
                if (current == goal)
                {
                    List<CubicalCoordinate> totalPath = new List<CubicalCoordinate>() {current};
                    while (current != start)
                    {
                        current = cameFrom[current];
                        totalPath.Add(current);
                    }
                    totalPath.Add(start);
                    return totalPath;
                }

                closedSet.Add(current);

                foreach (Tuple<CubicalCoordinate, byte> tuple in GetNeighbours(current))
                {
                    // Have already processed neighbour
                    if (closedSet.Contains(tuple.Item1))
                    {
                        continue;
                    }

                    float traverseCost = CalculateGScore(tuple.Item1);

                    // If tile is not traversible skip
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (traverseCost == float.MaxValue)
                    {
                        continue;
                    }

                    float tentativeGScore = gScore[current] + traverseCost;

                    // Neighbour is new
                    if (!queue.Contains(tuple.Item1))
                    {
                        queue.Enqueue(tuple.Item1,
                            tentativeGScore + CubicalCoordinate.DistanceBetween(tuple.Item1, goal));
                    }
                    // This path is not better
                    else if (tentativeGScore >= gScore[tuple.Item1])
                    {
                        continue;
                    }

                    // This path is the best we've found so far

                    cameFrom[tuple.Item1] = current;
                    gScore[tuple.Item1] = tentativeGScore;
                }
            }

            return null;
        }

        // TODO Include unit skill
        private float CalculateGScore(CubicalCoordinate cc)
        {
            switch ((TileType) this[cc])
            {
                case TileType.Grass:
                    return 1;
                case TileType.WaterShallow:
                    return float.MaxValue;
                case TileType.Desert:
                    return 2;
                default:
                    return float.MaxValue;
            }
        }
    }
}