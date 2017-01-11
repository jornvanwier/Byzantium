namespace Map.Generation
{
    public interface IMapGenerator
    {
        byte[,] Generate(int size, float borderPercentage);
    }
}