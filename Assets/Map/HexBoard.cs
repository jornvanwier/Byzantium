﻿using System;
using System.Collections.Generic;
using Assets.Map.Pathfinding;
using Map;
using Map.Generation;
using Map.Pathfinding;
using Priority_Queue;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Map
{
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
        private NodeGraph NodeGraph { get; set; }


        public HexBoard(int size)
        {
            this.size = size;
        }

        public void GenerateMap()
        {
            Storage = Generator.Generate(size, BorderPercentage);
            NodeGraph = new NodeGraph(size);
        }

        public byte this[CubicalCoordinate cc]
        {
            get
            {
                OddRCoordinate oc = cc.ToOddR();
//                Debug.Log("OUT " + oc);
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
            return oc.Q >= 0 && oc.Q < size && oc.R >= 0 && oc.R < size;
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
            } while (this[cc] == (byte) TileType.WaterDeep);

            return cc;
        }

        public List<Tuple<CubicalCoordinate, byte>> GetNeighbours(CubicalCoordinate cc)
        {
            var neighbours = new List<Tuple<CubicalCoordinate, byte>>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (CubicalCoordinate direction in Directions)
            {
                CubicalCoordinate neighbour = cc + direction;
                if (!CheckCoordinate(neighbour)) continue;

                var tuple = new Tuple<CubicalCoordinate, byte>(neighbour, this[neighbour]);
                neighbours.Add(tuple);
            }
            return neighbours;
        }

        public List<AStarNode> GetNeighbouringNodes(CubicalCoordinate cc)
        {
            var nodes = new List<AStarNode>();

            foreach (CubicalCoordinate direction in Directions)
            {
                CubicalCoordinate neighbour = cc + direction;
                if (!CheckCoordinate(neighbour)) continue;

                nodes.Add(NodeGraph[neighbour]);
            }

            return nodes;
        }

        // TODO Replace start with unit or legion
        public List<CubicalCoordinate> FindPath(CubicalCoordinate start, CubicalCoordinate goal)
        {
            var closedSet = new HashSet<AStarNode>();

            var cameFrom = new Dictionary<AStarNode, AStarNode>();

            AStarNode startNode = NodeGraph[start];
            AStarNode goalNode = NodeGraph[goal];

            var gScore = new Dictionary<AStarNode, float>
            {
                [startNode] = 0
            };

            var queue = new FastPriorityQueue<AStarNode>(size * size);
            queue.Enqueue(startNode, 0);

            while (queue.Count > 0)
            {
                AStarNode current = queue.Dequeue();
                // Backtrack path
                if (current == goalNode)
                {
                    var totalPath = new List<CubicalCoordinate>() {goal};
                    while (current != startNode)
                    {
                        current = cameFrom[current];
                        totalPath.Add(current.Position);
                    }
                    totalPath.Add(start);
                    return totalPath;
                }

                closedSet.Add(current);

                foreach (AStarNode neighbour in GetNeighbouringNodes(current.Position))
                {
                    // Have already processed neighbour
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float traverseCost = CalculateGScore(neighbour.Position);

                    // If tile is not traversible skip
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (traverseCost == float.MaxValue)
                    {
                        continue;
                    }

                    float tentativeGScore = gScore[current] + traverseCost;

                    // Neighbour is new
                    if (!queue.Contains(neighbour))
                    {
                        queue.Enqueue(neighbour,
                            tentativeGScore + CubicalCoordinate.DistanceBetween(neighbour.Position, goalNode.Position));
                    }
                    // This path is not better
                    else if (tentativeGScore >= gScore[neighbour])
                    {
                        continue;
                    }

                    // This path is the best we've found so far

                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                }
            }
            Debug.LogWarning($"No path found between {start} and {goal} after checking {closedSet.Count}");
            return null;
        }

        // TODO Include unit skill
        private float CalculateGScore(CubicalCoordinate cc)
        {
            switch ((TileType) this[cc])
            {
                case TileType.GrassLand:
                    return 2;
                case TileType.WaterShallow:
                    return 20;
                case TileType.WaterDeep:
                    return float.MaxValue;
                case TileType.TemperateDesert:
                    return 11;
                case TileType.Beach:
                    return 4;
                case TileType.Path:
                    return 0;
                case TileType.Snow:
                    return 15;
                case TileType.Tundra:
                    return 10;
                case TileType.Bare:
                    return 11;
                case TileType.Scorched:
                    return 13;
                case TileType.Taiga:
                    return 8;
                case TileType.Shrubland:
                    return 5;
                case TileType.TemperateRainForest:
                    return 12;
                case TileType.TemperateDeciduousForest:
                    return 7;
                case TileType.TropicalRainForest:
                    return 9;
                case TileType.TropicalSeasonalForest:
                    return 6;
                case TileType.SubTropicalDesert:
                    return 14;
                default:
                    Debug.Log("tile not exist");
                    return float.MaxValue;
            }
        }
    }
}