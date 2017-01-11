namespace Map
{
    public struct CubicalCoordinate
    {
        public int X { get; set; }
        public int Z { get; set; }
        public int Y => -X - Z;

        public CubicalCoordinate(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static CubicalCoordinate operator +(CubicalCoordinate a, CubicalCoordinate b)
        {
            return new CubicalCoordinate(a.X + b.X, a.Z + b.Z);
        }

        public OddRCoordinate ToOddR()
        {
            return new OddRCoordinate(
                X + (Z - (Z & 1)) / 2,
                Z
            );
        }
    }
}