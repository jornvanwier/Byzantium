﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Map;
using Map.Generation;

namespace Assets.Map
{
    public class MapRenderer : MonoBehaviour
    {
        private ComputeBuffer _computeBuffer;
        private HexBoard hexBoard;

        private Texture2DArray _albedoMaps;
        private Texture2DArray _normalMaps;
        private Texture2DArray _amboccMaps;
        private Texture2DArray _glossyMaps;
        private Texture2DArray _metallMaps;

        private const int TextureSize = 1024;

        public int MapSize;
        private List<TextureSet> _textureSets;

        public Mesh Mesh;

        public Material HexMaterial;

        public Texture2D DefaultAlbedoMap;
        public Texture2D DefaultHeightMap;
        public Texture2D DefaultNormalMap;
        public Texture2D DefaultAmbOccMap;
        public Texture2D DefaultGlossyMap;
        public Texture2D DefaultMetallMap;

        public Texture2D WaterDeepAlbedo;
        public Texture2D WaterShallowAlbedo;
        public Texture2D GrassAlbedo;
        public Texture2D ForestAlbedo;
        public Texture2D MountainLowAlbedo;
        public Texture2D MountainHightAlbedo;
        public Texture2D MountainTopAlbedo;
        public Texture2D BeachAlbedo;
        public Texture2D DesertAlbedo;
        public Texture2D PathAlbedo;

        private TextureSet _defaultTextureSet;

        [UsedImplicitly]
        private void Start()
        {
            _defaultTextureSet = new TextureSet
            {
                AlbedoMap = DefaultAlbedoMap,
                AmbOccMap = DefaultAmbOccMap,
                GlossyMap = DefaultGlossyMap,
                MetallMap = DefaultMetallMap,
                NormalMap = DefaultNormalMap
            };

            _textureSets = new List<TextureSet>();

            TextureSet.SetDefaultTextures(_defaultTextureSet);

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = WaterDeepAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = WaterShallowAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = GrassAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = ForestAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = MountainLowAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = MountainHightAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = MountainTopAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = BeachAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = DesertAlbedo;

            _textureSets.Add(new TextureSet());
            _textureSets.Last().AlbedoMap = PathAlbedo;

            _albedoMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.DXT5, true);
            _normalMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.DXT5, true);
            _amboccMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.DXT5, true);
            _glossyMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.DXT5, true);
            _metallMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.DXT5, true);

            for (var i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
            {
                for (int j = 0; j < Convert.ToInt32(Mathf.Log(TextureSize, 2) + 1); ++j)
                {
                    Graphics.CopyTexture(_textureSets[i].AlbedoMap, 0, j, _albedoMaps, i, j);
                    Graphics.CopyTexture(_textureSets[i].AmbOccMap, 0, j, _amboccMaps, i, j);
                    Graphics.CopyTexture(_textureSets[i].GlossyMap, 0, j, _glossyMaps, i, j);
                    Graphics.CopyTexture(_textureSets[i].MetallMap, 0, j, _metallMaps, i, j);
                    Graphics.CopyTexture(_textureSets[i].NormalMap, 0, j, _normalMaps, i, j);
                }
            }

            hexBoard = new HexBoard(MapSize) {Generator = new TestGenerator()};
            hexBoard.GenerateMap();


            CubicalCoordinate start = hexBoard.RandomValidTile();

            CubicalCoordinate goal = hexBoard.RandomValidTile();

//            float tStart = Time.realtimeSinceStartup;
//            List<CubicalCoordinate> path = hexBoard.FindPath(start, goal);
//            Debug.Log($"Ran pathfinding in {Time.realtimeSinceStartup - tStart} seconds");
//
//            if (path != null)
//            {
//                foreach (CubicalCoordinate hex in path)
//                {
//                    hexBoard[hex] = (byte) TileType.WaterDeep;
//                }
//            }
//            else
//            {
//                Debug.LogWarning($"No path found between {start} and {goal}");
//            }
//
//
//            hexBoard[start] = (byte) TileType.WaterDeep;
//            hexBoard[goal] = (byte) TileType.WaterDeep;

            SetupShader();
        }

        private void SetupShader()
        {
            _computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.GPUMemory);

            int[,] data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            {
                for (int y = 0; y < MapSize; ++y)
                {
                    data[x, y] = hexBoard.Storage[x, y];
                }
            }

            _computeBuffer.SetData(data);

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
            block.SetBuffer(Shader.PropertyToID("_HexagonBuffer"), _computeBuffer);

            block.SetTexture("_AlbedoMaps", _albedoMaps);
            block.SetTexture("_NormalMaps", _normalMaps);
            block.SetTexture("_AmbOccMaps", _amboccMaps);
            block.SetTexture("_GlossyMaps", _glossyMaps);
            block.SetTexture("_MetallMaps", _metallMaps);


            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer mrenderer = gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = Mesh;
            mrenderer.material = HexMaterial;

            mrenderer.SetPropertyBlock(block);

        }

        [UsedImplicitly]
        private void Update()
        {
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            _computeBuffer?.Dispose();
        }
    }
}