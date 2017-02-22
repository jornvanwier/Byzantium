using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.Map;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Networking.Types;

namespace Map.Pathfinding
{
    public class NavMesh
    {
        private const int MaxNodeDistance = 4;

        public Dictionary<CubicalCoordinate, NavMeshNode> Nodes { get; private set; }

        public NavMesh(HexBoard hexBoard)
        {
            GenerateNavMesh(hexBoard);
        }

        public NavMeshNode ClosestNodeTo(CubicalCoordinate cc)
        {
            if (Nodes.ContainsKey(cc))
            {
                return Nodes[cc];
            }
            NavMeshNode current = null;
            int distance = int.MaxValue;

            foreach (NavMeshNode node in Nodes.Values)
            {
                int nodeDist = node.Position.DistanceTo(cc);
                if (distance == 1)
                {
                    return node;
                }
                if (nodeDist >= distance)
                {
                    continue;
                }
                current = node;
                distance = node.Position.DistanceTo(cc);
            }

            return current;
        }

        private struct NeighbourRing
        {
            public NeighbourRing(CubicalCoordinate coordinate, int ring)
            {
                this.Coordinate = coordinate;
                this.Ring = ring;
            }

            public CubicalCoordinate Coordinate { get; }
            public int Ring { get; }
        }

        private bool IsNodeNear(CubicalCoordinate coordinate, HexBoard hexBoard)
        {
            if (Nodes.ContainsKey(coordinate))
            {
                return true;
            }

            var queue = new Queue<NeighbourRing>();
            var closedSet = new HashSet<CubicalCoordinate>();

            List<Tuple<CubicalCoordinate, byte>> immediateNeighbours = hexBoard.GetNeighbours(coordinate);

            foreach (Tuple<CubicalCoordinate, byte> neighbour in immediateNeighbours)
            {
                queue.Enqueue(new NeighbourRing(neighbour.Item1, 1));
            }

            while (queue.Count > 0)
            {
                NeighbourRing item = queue.Dequeue();
                if (closedSet.Contains(item.Coordinate))
                {
                    continue;
                }
                closedSet.Add(item.Coordinate);
                if (item.Ring > MaxNodeDistance)
                {
                    return false;
                }

                if (Nodes.ContainsKey(item.Coordinate))
                {
                    return true;
                }

                List<Tuple<CubicalCoordinate, byte>> neighbours = hexBoard.GetNeighbours(item.Coordinate);
                foreach (Tuple<CubicalCoordinate, byte> neighbour in neighbours)
                {
                    if(!closedSet.Contains(neighbour.Item1))
                    {
                        queue.Enqueue(new NeighbourRing(neighbour.Item1, item.Ring + 1));
                    }
                }
            }

            return false;
        }

        private void GenerateNavMesh(HexBoard hexBoard)
        {
            Utils.LogOperationTime("generate nav mesh", () =>
            {
                Utils.LogOperationTime("generate border nodes", () => GenerateBorderNodes(hexBoard));
                Utils.LogOperationTime("connect nodes", () => ConnectNodes(hexBoard));
                Utils.LogOperationTime("generate middle nodes", () => GenerateMiddleNodes(hexBoard));
            });
        }

        private void GenerateBorderNodes(HexBoard hexBoard)
        {
            Nodes = new Dictionary<CubicalCoordinate, NavMeshNode>();
            for (int x = 0; x < hexBoard.Storage.GetLength(1); x++)
            {
                for (int y = 0; y < hexBoard.Storage.GetLength(1); y++)
                {
                    CubicalCoordinate cc = new OddRCoordinate(x,y).ToCubical();
                    List<Tuple<CubicalCoordinate, byte>> neighbours = hexBoard.GetNeighbours(cc);

                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        if (neighbours[i].Item2 != hexBoard[cc])
                        {
                            Nodes.Add(cc, new NavMeshNode(cc));
                            break;
                        }
                    }
                }
            }
        }

        private void ConnectNodes(HexBoard hexBoard)
        {
            foreach (KeyValuePair<CubicalCoordinate, NavMeshNode> node in Nodes)
            {
                List<Tuple<CubicalCoordinate, byte>> neighbours = hexBoard.GetNeighbours(node.Key);

                foreach (Tuple<CubicalCoordinate, byte> neighbour in neighbours)
                {
                    if(Nodes.ContainsKey(neighbour.Item1))
                    {
                        node.Value.Connections.Add(Nodes[neighbour.Item1]);
                    }
                }
            }
        }

        private void GenerateMiddleNodes(HexBoard hexBoard)
        {
            for (int x = 0; x < hexBoard.Storage.GetLength(1); x++)
            {
                for (int y = 0; y < hexBoard.Storage.GetLength(1); y++)
                {
                    CubicalCoordinate tileCoordinate = new OddRCoordinate(x,y).ToCubical();
                    if (!IsNodeNear(tileCoordinate, hexBoard))
                    {
                        Nodes.Add(tileCoordinate, new NavMeshNode(tileCoordinate));
                        hexBoard[tileCoordinate] = (byte) TileType.WaterDeep;
                        // Skip next tiles because we just placed a node near them
                        y += MaxNodeDistance;
                    }
                }
            }
        }
    }
}