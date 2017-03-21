using Priority_Queue;

namespace Assets.Scripts.Map.Pathfinding
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