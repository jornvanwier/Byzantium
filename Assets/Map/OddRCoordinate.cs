namespace Map
{
    public struct OddRCoordinate
    {
        public int Q { get; set; }
        public int R { get; set; }

        public OddRCoordinate(int q, int r)
        {
            Q = q;
            R = r;
        }

        public CubicalCoordinate ToCubical()
        {
            return new CubicalCoordinate(
                Q - (R - (R & 1)) / 2,
                R
            );

        }

        public override string ToString()
        {
            return $"Q: {Q} R: {R}";
        }
    }
}