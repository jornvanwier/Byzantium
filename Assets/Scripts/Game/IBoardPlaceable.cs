using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public interface IBoardPlaceable
    {
        CubicalCoordinate Position { get; set; }
        Vector3 DrawOffset { get; set; }
    }
}