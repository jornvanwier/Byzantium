namespace Assets.Scripts.Map.Generation
{
    public class SquareGenerator : IMapGenerator
    {
        public byte[,] Generate(int size, float borderPercentage)
        {
            var result = new byte[size, size];

            for (int x = 0; x < size; ++x)
            for (int y = 0; y < size; ++y)
                if (x <= borderPercentage * size || x >= size - borderPercentage * size ||
                    y <= borderPercentage * size || y >= size - borderPercentage * size)
                    result[x, y] = (byte) TileType.WaterShallow;
                else
                    result[x, y] = (byte) TileType.GrassLand;

            return result;
        }
    }
}