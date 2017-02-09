using Map;

namespace Assets.Map.Generation
{
    internal class TerrainType
    {
        public TerrainType(TileType type, float height)
        {
            Type = type;
            Height = height;
        }

        public TileType Type { get; }

        public float Height { get; }
    }
}