using System;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public struct Int2
    {
        public int X;
        public int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public float Distance(Int2 coordinate)
        {
            return (float) Math.Sqrt(Math.Pow(X - coordinate.X, 2) + Math.Pow(Y - coordinate.Y, 2));
        }

        public static Int2 operator +(Int2 i1, Int2 i2)
        {
            return new Int2(i1.X + i2.X, i1.Y + i2.Y);
        }

        public static Int2 operator -(Int2 i1, Int2 i2)
        {
            return new Int2(i1.X - i2.X, i1.Y - i2.Y);
        }

        public static Int2 operator *(Int2 i1, Int2 i2)
        {
            return new Int2(i1.X * i2.X, i1.Y * i2.Y);
        }

        public static Vector2 operator *(Int2 i1, float scalar)
        {
            return new Vector2(i1.X * scalar, i1.Y * scalar);
        }

        public static Int2 operator *(Int2 i1, int scalar)
        {
            return new Int2(i1.X * scalar, i1.Y * scalar);
        }

        public static implicit operator Vector2(Int2 src)
        {
            return new Vector2(src.X, src.Y);
        }
    }

    public struct Int3
    {
        public int X;
        public int Y;
        public int Z;

        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Float2
    {
        public float X;
        public float Y;

        public Float2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;

        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}