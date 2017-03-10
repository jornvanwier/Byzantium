using System;
using Assets.Map;
using JetBrains.Annotations;
using Map;
using UnityEngine;

namespace Assets.Game
{
    public class BoardMesh : MonoBehaviour, IBoardPlaceable
    {
        [SerializeField]
        public CubicalCoordinate Position { get; set; }
        public Vector2 DrawOffset { get; set; }
        private MapRenderer mapRenderer;

        [UsedImplicitly]
        private void Start()
        {
            mapRenderer = GameObject.Find("Map").GetComponent<MapRenderer>();
        }

        [UsedImplicitly]
        private void Update()
        {
            transform.position = mapRenderer.CubicalCoordinateToWorld(Position);
        }
    }
}