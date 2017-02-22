using Map;
using Map.Pathfinding;

namespace Assets.Map.Pathfinding
{
    public class NodeGraph
    {
        private readonly int size;
        public AStarNode[,] NodeStorage { get; set; }

        public NodeGraph(int size)
        {
            this.size = size;
            CreateNodes();
        }

        public void CreateNodes()
        {
            NodeStorage = new AStarNode[size,size];
            for (int q = 0; q < size; ++q)
            {
                for (int r = 0; r < size; ++r)
                {
                    CubicalCoordinate coord = new OddRCoordinate(q, r).ToCubical();
                    NodeStorage[q, r] = new AStarNode(coord);
                }
            }
        }

        public AStarNode this[CubicalCoordinate cc]
        {
            get
            {
                OddRCoordinate oc = cc.ToOddR();
                return NodeStorage[oc.R, oc.Q];
            }
            set
            {
                OddRCoordinate oc = cc.ToOddR();
                NodeStorage[oc.R, oc.Q] = value;
            }
        }

    }
}