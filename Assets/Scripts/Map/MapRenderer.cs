﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Map.Generation;
using Assets.Scripts.Map.Pathfinding;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapRenderer : MonoBehaviour, IDisposable
    {
        private const int TextureSize = 1024;

        private readonly List<Int2> selectedSet = new List<Int2>();

        private Texture2DArray albedoMaps;

        public Texture2D[] AlbedoMaps;
        private Texture2DArray amboccMaps;
        private MaterialPropertyBlock block;
        private ComputeBuffer computeBuffer;
        private int[,] data;

        public Texture2D DefaultAlbedoMap;
        public Texture2D DefaultAmbOccMap;
        public Texture2D DefaultGlossyMap;
        public Texture2D DefaultHeightMap;
        public Texture2D DefaultMetallMap;
        public Texture2D DefaultNormalMap;


        private TextureSet defaultTextureSet;
        public GameObject Foliage;
        private FoliageRenderer foliageRenderer;
        private Texture2DArray glossyMaps;

        public Material HexMaterial;


        public int MapSize;

        public Mesh Mesh;
        private MeshRenderer meshRenderer;
        private Texture2DArray metallMaps;
        private Texture2DArray normalMaps;

        private List<TextureSet> textureSets;
        public HexBoard HexBoard { get; private set; }

        public void Dispose()
        {
            computeBuffer?.Dispose();
        }

        [UsedImplicitly]
        private void Start()
        {
            defaultTextureSet = new TextureSet
            {
                AlbedoMap = DefaultAlbedoMap,
                AmbOccMap = DefaultAmbOccMap,
                GlossyMap = DefaultGlossyMap,
                MetallMap = DefaultMetallMap,
                NormalMap = DefaultNormalMap
            };

            textureSets = new List<TextureSet>();

            TextureSet.SetDefaultTextures(defaultTextureSet);
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
                textureSets.Add((TextureSet) defaultTextureSet.Clone());
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
                textureSets[i].AlbedoMap = AlbedoMaps[i];

            albedoMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            normalMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            amboccMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            glossyMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            metallMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);

            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
            for (int j = 0; j < Convert.ToInt32(Mathf.Log(TextureSize, 2) + 1); ++j)
            {
                Graphics.CopyTexture(textureSets[i].AlbedoMap, 0, j, albedoMaps, i, j);
                Graphics.CopyTexture(textureSets[i].AmbOccMap, 0, j, amboccMaps, i, j);
                Graphics.CopyTexture(textureSets[i].GlossyMap, 0, j, glossyMaps, i, j);
                Graphics.CopyTexture(textureSets[i].MetallMap, 0, j, metallMaps, i, j);
                Graphics.CopyTexture(textureSets[i].NormalMap, 0, j, normalMaps, i, j);
            }

            HexBoard = new HexBoard(MapSize) {Generator = new PerlinGenerator()};
            HexBoard.GenerateMap();

            SetupShader();
            gameObject.transform.localScale = new Vector3(MapSize, MapSize, 0);

            PathfindingJobManager.Init(HexBoard);

            Foliage = Instantiate(Foliage);
            Foliage.name = "Foliage";
            foliageRenderer = Foliage.GetComponent<FoliageRenderer>();
            foliageRenderer.AttachBoard(HexBoard, this);
        }

        private void SetupShader()
        {
            computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.Raw);

            data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            for (int y = 0; y < MapSize; ++y)
            {
                data[x, y] = HexBoard.Storage[x, y];
                var t = new TileData((TileType) HexBoard.Storage[x, y], false);
                int k = t.GetAsInt();
                data[x, y] = k;
            }

            computeBuffer.SetData(data);

            block = new MaterialPropertyBlock();

            block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
            block.SetBuffer(Shader.PropertyToID("_HexagonBuffer"), computeBuffer);

            block.SetTexture("_AlbedoMaps", albedoMaps);
            block.SetTexture("_NormalMaps", normalMaps);
            block.SetTexture("_AmbOccMaps", amboccMaps);
            block.SetTexture("_GlossyMaps", glossyMaps);
            block.SetTexture("_MetallMaps", metallMaps);


            var filter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = Mesh;
            meshRenderer.material = HexMaterial;

            meshRenderer.SetPropertyBlock(block);
        }

        [UsedImplicitly]
        private void Update()
        {
            UpdateSelectedSet();
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            computeBuffer?.Dispose();
        }

        private static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public Vector3 CubicalCoordinateToWorld(CubicalCoordinate cc)
        {
            float hexSize = Mathf.Sqrt(3) / 3;

            float x = hexSize * (3 / 2f) * cc.Z;
            float z = hexSize * Mathf.Sqrt(3) * (cc.X + cc.Z / 2);

            float zOffset = cc.ToOddR().IsUneven()
                ? (gameObject.transform.localScale.y - MapSize * 0.005f) / 2 / MapSize
                : 0;

            float tX = x - (gameObject.transform.localScale.x - MapSize * 0.075f * 2) / 2;
            float tZ = z - (gameObject.transform.localScale.y - MapSize * 0.005f * 2) / 2 + zOffset;

            return new Vector3(tX, 0, tZ);
        }

        public CubicalCoordinate WorldToCubicalCoordinate(Vector3 worldPos)
        {
            return NormalizedWorldToHexagonPosition(WorldToNormalizedWorldPosition(worldPos));
        }


        private Vector2 WorldToNormalizedWorldPosition(Vector3 worldPosition)
        {
            var position = new Vector2(worldPosition.x, worldPosition.z);

            //scale to 0 - 1
            float x = Remap(position.x, -(gameObject.transform.localScale.x / 2), gameObject.transform.localScale.x / 2,
                0, 1);
            float y = Remap(position.y, -(gameObject.transform.localScale.y / 2), gameObject.transform.localScale.y / 2,
                0, 1);

            return new Vector2(x, y);
        }

        private CubicalCoordinate NormalizedWorldToHexagonPosition(Vector2 worldPosition)
        {
            float hexSize = Mathf.Sqrt(3) / 3;

            float posX = worldPosition.x * MapSize - 0.075f * MapSize;
            float posY = worldPosition.y * MapSize - 0.005f * MapSize;

            float cubeX = posX * 2 / 3 / hexSize;
            float cubeZ = (-posX / 3 + Mathf.Sqrt(3) / 3 * posY) / hexSize;
            float cubeY = -cubeX - cubeZ;

            int rX = Mathf.RoundToInt(cubeX);
            int rZ = Mathf.RoundToInt(cubeZ);
            int rY = Mathf.RoundToInt(cubeY);

            float xDiff = Mathf.Abs(cubeX - rX);
            float zDiff = Mathf.Abs(cubeZ - rZ);
            float yDiff = Mathf.Abs(cubeY - rY);

            if (xDiff > yDiff && xDiff > zDiff)
                rX = -rY - rZ;
            else if (yDiff > zDiff)
                rY = -rX - rZ;
            else
                rZ = -rX - rY;

            return new CubicalCoordinate(rZ, rX);
        }

        private void UpdateSelectedSet()
        {
            foreach (Int2 tile in selectedSet)
            {
                int tileData = data[tile.Y, tile.X];
                var src = new TileData(tileData);
                src.SetSelected(true);
                data[tile.Y, tile.X] = src.GetAsInt();
            }

            computeBuffer.SetData(data);

            foreach (Int2 tile in selectedSet)
            {
                int tileData = data[tile.Y, tile.X];
                var src = new TileData(tileData);
                src.SetSelected(false);
                data[tile.Y, tile.X] = src.GetAsInt();
            }
            selectedSet.Clear();
        }

        public void MarkTileSelectedForNextFrame(CubicalCoordinate cc)
        {
            MarkTileSelectedForNextFrame(cc.ToOddR());
        }

        public void MarkTileSelectedForNextFrame(OddRCoordinate oc)
        {
            selectedSet.Add(new Int2(oc.Q, oc.R));
        }
    }
}