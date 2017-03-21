namespace Assets.Scripts.Map.Pathfinding
{
    public class NodeGraph
    {
        private readonly int size;

        public NodeGraph(int size)
        {
            this.size = size;
            CreateNodes();
        }

        public AStarNode[,] NodeStorage { get; set; }

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

        public void CreateNodes()
        {
            NodeStorage = new AStarNode[size, size];
            for (var q = 0; q < size; ++q)
            for (var r = 0; r < size; ++r)
            {
                CubicalCoordinate coord = new OddRCoordinate(q, r).ToCubical();
                NodeStorage[r, q] = new AStarNode(coord);
            }
        }
    }
}