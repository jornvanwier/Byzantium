using System;

namespace Assets.Map
{
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(Int2 coordinate)
        {
            return (float) Math.Sqrt(Math.Pow(x - coordinate.x, 2) + Math.Pow(y - coordinate.y, 2));
        }

        public static Int2 operator +(Int2 i1, Int2 i2)
        {
            return new Int2(i1.x + i2.x, i1.y + i2.y);
        }

        public static Int2 operator -(Int2 i1, Int2 i2)
        {
            return new Int2(i1.x - i2.x, i1.y - i2.y);
        }

        public static Int2 operator *(Int2 i1, Int2 i2)
        {
            return new Int2(i1.x * i2.x, i1.y * i2.y);
        }

        public static Int2 operator *(Int2 i1, float scalar)
        {
            return new Int2((int)(i1.x * scalar), (int)(i1.y * scalar));
        }

        public static Int2 operator *(Int2 i1, int scalar)
        {
            return new Int2(i1.x * scalar, i1.y * scalar);
        }
    }

    public struct Int3
    {
        public int x;
        public int y;
        public int z;

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct Float2
    {
        public float x;
        public float y;

        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Float3
    {
        public float x;
        public float y;
        public float z;

        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}