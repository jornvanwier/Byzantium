using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Map.Generation;
using Map;

namespace Assets.Map
{
    public class MapRenderer : MonoBehaviour
    {
        private ComputeBuffer computeBuffer;
        private HexBoard hexBoard;

        private Texture2DArray _albedoMaps;
        private Texture2DArray _heightMaps;
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


        private TextureSet _defaultTextureSet;

        [UsedImplicitly]
        private void Start()
        {
            _defaultTextureSet = new TextureSet
            {
                AlbedoMap = DefaultAlbedoMap,
                HeightMap = DefaultHeightMap,
                AmbOccMap = DefaultAmbOccMap,
                GlossyMap = DefaultGlossyMap,
                MetallMap = DefaultMetallMap,
                NormalMap = DefaultNormalMap
            };

            _textureSets = new List<TextureSet>();

            for (var i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
                _textureSets.Add(_defaultTextureSet);

            _albedoMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);
            _heightMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);
            _normalMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);
            _amboccMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);
            _glossyMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);
            _metallMaps = new Texture2DArray(TextureSize, TextureSize, _textureSets.Count, TextureFormat.RGBA32, true);


            Debug.Log("Mip levels: " + Convert.ToInt32(Mathf.Log(TextureSize,2)));
            for (var i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
            {
                    _albedoMaps.SetPixels(_textureSets[i].AlbedoMap.GetPixels(0), i , 0);
                    _heightMaps.SetPixels(_textureSets[i].HeightMap.GetPixels(0), i , 0);
                    _normalMaps.SetPixels(_textureSets[i].NormalMap.GetPixels(0), i , 0);
                    _amboccMaps.SetPixels(_textureSets[i].AmbOccMap.GetPixels(0), i , 0);
                    _glossyMaps.SetPixels(_textureSets[i].GlossyMap.GetPixels(0), i , 0);
                    _metallMaps.SetPixels(_textureSets[i].MetallMap.GetPixels(0), i , 0);
            }

            _albedoMaps.Apply();
            _amboccMaps.Apply();
            _glossyMaps.Apply();
            _heightMaps.Apply();
            _metallMaps.Apply();
            _normalMaps.Apply();


            hexBoard = new HexBoard(MapSize) {Generator = new TestGenerator()};
            hexBoard.GenerateMap();

            /*
            CubicalCoordinate start = hexBoard.RandomValidTile();

            CubicalCoordinate goal = hexBoard.RandomValidTile();

            List<CubicalCoordinate> path = hexBoard.FindPath(start, goal);

            if (path != null)
            {
                foreach (CubicalCoordinate hex in path)
                {
                    hexBoard[hex] = (byte) TileType.Path;
                }
            }


            hexBoard[start] = (byte) TileType.Path;
            hexBoard[goal] = (byte) TileType.Path;
            */
            SetupShader();
        }

        private void SetupShader()
        {
            computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.GPUMemory);

            int[,] data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            {
                for (int y = 0; y < MapSize; ++y)
                {
                    data[x, y] = hexBoard.Storage[x, y];
                }
            }

            computeBuffer.SetData(data);

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
            block.SetBuffer(Shader.PropertyToID("_HexagonBuffer"), computeBuffer);

            block.SetTexture("_AlbedoMaps", _albedoMaps);
            block.SetTexture("_NormalMaps", _normalMaps);
            block.SetTexture("_HeightMaps", _heightMaps);
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
            computeBuffer?.Dispose();
        }
    }
}