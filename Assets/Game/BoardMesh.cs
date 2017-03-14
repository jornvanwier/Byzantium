using System;
using System.Runtime.InteropServices;
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

        public Vector3 DrawOffset { get; set; }
        protected MapRenderer MapRenderer;

        [UsedImplicitly]
        protected virtual void Start()
        {
            MapRenderer = GameObject.Find("Map").GetComponent<MapRenderer>();
        }

        [UsedImplicitly]
        protected virtual void Update()
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