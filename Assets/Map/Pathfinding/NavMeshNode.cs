using System;
using System.Collections.Generic;
using Priority_Queue;

namespace Map.Pathfinding
{
    public class NavMeshNode : FastPriorityQueueNode
    {
        public CubicalCoordinate Position { get; private set; }
        public List<NavMeshNode> Connections { get; private set; }

        public NavMeshNode(CubicalCoordinate position)
        {
            Position = position;
            Connections = new List<NavMeshNode>();
        }

        public NavMeshNode(CubicalCoordinate position, List<NavMeshNode> connections)
        {
            Position = position;
            Connections = connections;
        }
    }
}