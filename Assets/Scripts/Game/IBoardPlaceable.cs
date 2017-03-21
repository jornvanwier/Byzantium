using Map;
using UnityEngine;

namespace Assets.Game
{
    public interface IBoardPlaceable
    {
        CubicalCoordinate Position { get; set; }
        Vector3 DrawOffset { get; set; }
    }
}