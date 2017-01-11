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

        public static CubicalCoordinate operator +(CubicalCoordinate a, CubicalCoordinate b)
        {
            return new CubicalCoordinate(a.X + b.X, a.Z + b.Z);
        }

        public static CubicalCoordinate operator -(CubicalCoordinate a, CubicalCoordinate b)
        {
            return new CubicalCoordinate(a.X - b.X, a.Z - b.Z);
        }

        public static bool operator ==(CubicalCoordinate a, CubicalCoordinate b)
        {
            return a.X == b.X && a.Z == b.Z;
        }

        public static bool operator !=(CubicalCoordinate a, CubicalCoordinate b)
        {
            return !(a == b);
        }

        public bool Equals(CubicalCoordinate other)
        {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CubicalCoordinate && Equals((CubicalCoordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Z;
            }
        }
    }
}