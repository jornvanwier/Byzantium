using Map;
using UnityEngine;

namespace Assets.Game
{
    public interface IBoardPlaceable
    {
        CubicalCoordinate Position { get; set; }
        Vector2 DrawOffset { get; set; }
    }
}