using System;

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

        public static CubicalCoordinate operator -(CubicalCoordinate a, CubicalCoordinate b)
        {
            return new CubicalCoordinate(a.X - b.X, a.Z - b.Z);
        }

        public int DistanceTo(CubicalCoordinate other)
        {
            return DistanceBetween(this, other);
        }

        public static int DistanceBetween(CubicalCoordinate a, CubicalCoordinate b)
        {
            return (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z)) / 2;
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