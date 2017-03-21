namespace Assets.Scripts.Map.Generation
{
    public interface IMapGenerator
    {
        byte[,] Generate(int size, float borderPercentage);
    }
}