using Map;

namespace Assets.Map.Generation
{
    public class Biome
    {
        public Biome(TileType type, float moisture)
        {
            Type = type;
            Moisture = moisture;
        }

        public TileType Type { get; }
        public float Moisture { get; }
    }
}