using Priority_Queue;

namespace Map.Pathfinding
{
    public class AStarNode : FastPriorityQueueNode
    {
        public CubicalCoordinate Position;

        public AStarNode(CubicalCoordinate position)
        {
            Position = position;
        }
    }
}