using JetBrains.Annotations;
using Map;
using UnityEngine;

namespace Assets.Game
{
    public class BoardMesh : MonoBehaviour, IBoardPlaceable
    {
        public CubicalCoordinate Position { get; set; }
        public Vector2 DrawOffset { get; set; }

        [UsedImplicitly]
        private void Start()
        {
            transform.position = new Vector3();
        }
    }
}