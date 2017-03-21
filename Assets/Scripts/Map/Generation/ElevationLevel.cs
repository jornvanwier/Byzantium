namespace Assets.Scripts.Map.Generation
{
    public class ElevationLevel
    {
        public ElevationLevel(float height, params Biome[] biomes)
        {
            Biomes = biomes;
            Height = height;
        }

        public Biome[] Biomes { get; }
        public float Height { get; }
    }
}