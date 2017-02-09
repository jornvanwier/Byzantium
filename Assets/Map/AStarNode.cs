using Priority_Queue;

namespace Map
{
    public class AStarNode : FastPriorityQueueNode
    {
        public CubicalCoordinate Coordinate { get; }
        public bool IsStart { get; }

        public AStarNode(CubicalCoordinate coordinate, bool isStart = false)
        {
            Coordinate = coordinate;
            IsStart = isStart;
        }
    }
}