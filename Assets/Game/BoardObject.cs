using Assets.Map;
using JetBrains.Annotations;
using Map;
using UnityEngine;

namespace Assets.Game
{
    public class BoardObject : MonoBehaviour, IBoardPlaceable
    {
        public MapRenderer MapRenderer;

        [SerializeField]
        public CubicalCoordinate Position { get; set; }

        public Vector3 DrawOffset { get; set; }

        [UsedImplicitly]
        public virtual void Start()
        {
            MapRenderer = GameObject.Find("Map").GetComponent<MapRenderer>();
        }

        [UsedImplicitly]
        public virtual void Update()
        {
            SetWorldPos(MapRenderer.CubicalCoordinateToWorld(Position) + DrawOffset);
        }

        protected virtual void SetWorldPos(Vector3 worldPos)
        {
            transform.position = worldPos;
        }

        protected virtual void SetWorldRotation(Quaternion rotation)
        {
        }
    }
}