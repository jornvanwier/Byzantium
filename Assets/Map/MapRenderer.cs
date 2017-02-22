using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Map.Generation;
using Map;
using UnityEngine.WSA;

namespace Assets.Map
{
    public class MapRenderer : MonoBehaviour
    {
        private ComputeBuffer _computeBuffer;
        private HexBoard _hexBoard;

        private Texture2DArray _albedoMaps;
        private Texture2DArray _normalMaps;
        private Texture2DArray _amboccMaps;
        private Texture2DArray _glossyMaps;
        private Texture2DArray _metallMaps;

        private const int TextureSize = 1024;
        private int[,] _data;

        private readonly List<Int2> _selectedSet = new List<Int2>();



        public int MapSize;
        private List<TextureSet> _textureSets;

        public Mesh Mesh;

        public Material HexMaterial;
        private MaterialPropertyBlock _block;
        private MeshRenderer _mrenderer;

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

        public GameObject Test;


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

            _hexBoard = new HexBoard(MapSize) {Generator = new TestGenerator()};
            _hexBoard.GenerateMap();

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
            gameObject.transform.localScale = new Vector3(MapSize, MapSize, 0);
        }

        private void SetupShader()
        {
            _computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.Raw);

            _data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            {
                for (int y = 0; y < MapSize; ++y)
                {
                    TileData t = new TileData((TileType)_hexBoard.Storage[x, y], false);
                    int k = t.GetAsInt();
                    _data[x, y] = k;
                }
            }

            _computeBuffer.SetData(_data);

            _block = new MaterialPropertyBlock();

            _block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
            _block.SetBuffer(Shader.PropertyToID("_HexagonBuffer"), _computeBuffer);

            _block.SetTexture("_AlbedoMaps", _albedoMaps);
            _block.SetTexture("_NormalMaps", _normalMaps);
            _block.SetTexture("_AmbOccMaps", _amboccMaps);
            _block.SetTexture("_GlossyMaps", _glossyMaps);
            _block.SetTexture("_MetallMaps", _metallMaps);


            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            _mrenderer = gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = Mesh;
            _mrenderer.material = HexMaterial;

            _mrenderer.SetPropertyBlock(_block);

        }

        [UsedImplicitly]
        private void Update()
        {
            if (Test != null)
            {
                Vector3 normalized = WorldToNormalizedWorldPosition(Test.transform.position);
                HexagonData d = NormalizedWorldToHexagonPosition(normalized);

                if (d.hexagonPositionOffset.x > 0 && d.hexagonPositionOffset.x < MapSize - 1 &&
                    d.hexagonPositionOffset.y > 0 && d.hexagonPositionOffset.y < MapSize)
                {
                    MarkTileSelectedForNextFrame(d.hexagonPositionOffset.x, d.hexagonPositionOffset.y);
                    for (int i = -1; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j)
                        {
                            if (i == 0 && j == 0)
                                continue;
                            MarkTileSelectedForNextFrame(d.hexagonPositionOffset.x + i, d.hexagonPositionOffset.y + j);
                        }
                    }
                }
            }
            UpdateSelectedSet();
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            _computeBuffer?.Dispose();
        }

        private float Remap (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }


        public Vector2 WorldToNormalizedWorldPosition(Vector3 worldPosition)
        {
            Vector2 position = new Vector2(worldPosition.x, worldPosition.z);

            //scale to 0 - 1
            float x = Remap(position.x, -(gameObject.transform.localScale.x / 2), gameObject.transform.localScale.x / 2, 0, 1);
            float y = Remap(position.y, -(gameObject.transform.localScale.y / 2), gameObject.transform.localScale.y / 2, 0, 1);

            return new Vector2(x,y);
        }

        public HexagonData NormalizedWorldToHexagonPosition(Vector2 worldPosition)
        {
            float hexSize = Mathf.Sqrt(3)/3;

            float posX = worldPosition.x * MapSize - 0.075f * MapSize;
            float posY = worldPosition.y * MapSize - 0.005f * MapSize;

            float cubeX = posX * 2/3 / hexSize;
            float cubeZ = (-posX / 3 + Mathf.Sqrt(3)/3 * posY) / hexSize;
            float cubeY = -cubeX-cubeZ;

            int rX = Mathf.RoundToInt(cubeX);
            int rZ = Mathf.RoundToInt(cubeZ);
            int rY = Mathf.RoundToInt(cubeY);

            float xDiff = Mathf.Abs(cubeX - rX);
            float zDiff = Mathf.Abs(cubeZ - rZ);
            float yDiff = Mathf.Abs(cubeY - rY);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rX = -rY-rZ;
            }
            else if (yDiff > zDiff)
            {
                rY = -rX-rZ;
            }
            else
            {
                rZ = -rX-rY;
            }

            int x = (int)(rZ + (rX - (rX & 1)) / 2.0f),
                z = rX;

            HexagonData data;

            data.hexagonPositionOffset = new Int2(x,z);
            data.hexagonPositionCubical = new Int3(rX,rZ,rY);
            data.hexagonPositionFloat = new Float2(posX, posY);

            return data;
        }

        private void UpdateSelectedSet()
        {
            foreach(Int2 tile in _selectedSet)
            {
                int data = _data[tile.y, tile.x];
                TileData src = new TileData(data);
                src.SetSelected(true);
                _data[tile.y, tile.x] = src.GetAsInt();
            }

            _computeBuffer.SetData(_data);

            foreach(Int2 tile in _selectedSet)
            {
                int data = _data[tile.y, tile.x];
                TileData src = new TileData(data);
                src.SetSelected(false);
                _data[tile.y, tile.x] = src.GetAsInt();
            }
            _selectedSet.Clear();
        }

        public void MarkTileSelectedForNextFrame(int offsetX, int offsetY)
        {
            _selectedSet.Add(new Int2(offsetX, offsetY));
        }

    }
}