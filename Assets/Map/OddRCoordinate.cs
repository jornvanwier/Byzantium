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

        public static bool operator ==(OddRCoordinate a, OddRCoordinate b)
        {
            return a.Q == b.Q && a.R == b.R;
        }

        public static bool operator !=(OddRCoordinate a, OddRCoordinate b)
        {
            return !(a == b);
        }

        public bool Equals(OddRCoordinate other)
        {
            return Q == other.Q && R == other.R;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Q * 397) ^ R;
            }
        }
    }
}