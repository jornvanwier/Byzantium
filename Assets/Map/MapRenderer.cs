using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Map;
using Map.Generation;
using UnityEngine.WSA;

namespace Assets.Map
{
    public class MapRenderer : MonoBehaviour
    {
        private ComputeBuffer computeBuffer;
        private HexBoard hexBoard;

        private Texture2DArray albedoMaps;
        private Texture2DArray normalMaps;
        private Texture2DArray amboccMaps;
        private Texture2DArray glossyMaps;
        private Texture2DArray metallMaps;

        private const int TextureSize = 1024;
        private int[,] data;

        private readonly List<Int2> selectedSet = new List<Int2>();



        public int MapSize;
        private List<TextureSet> textureSets;

        public Mesh Mesh;

        public Material HexMaterial;
        private MaterialPropertyBlock block;
        private MeshRenderer meshRenderer;

        public Texture2D DefaultAlbedoMap;
        public Texture2D DefaultHeightMap;
        public Texture2D DefaultNormalMap;
        public Texture2D DefaultAmbOccMap;
        public Texture2D DefaultGlossyMap;
        public Texture2D DefaultMetallMap;

        public Texture2D[] AlbedoMaps;

        public GameObject Test;


        private TextureSet defaultTextureSet;

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
            {
                textureSets.Add((TextureSet)defaultTextureSet.Clone());
            }
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
            {
                textureSets[i].AlbedoMap = AlbedoMaps[i];
            }

            albedoMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            normalMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            amboccMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            glossyMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);
            metallMaps = new Texture2DArray(TextureSize, TextureSize, textureSets.Count, TextureFormat.DXT5, true);

            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; ++i)
            {
                for (int j = 0; j < Convert.ToInt32(Mathf.Log(TextureSize, 2) + 1); ++j)
                {
                    Graphics.CopyTexture(textureSets[i].AlbedoMap, 0, j, albedoMaps, i, j);
                    Graphics.CopyTexture(textureSets[i].AmbOccMap, 0, j, amboccMaps, i, j);
                    Graphics.CopyTexture(textureSets[i].GlossyMap, 0, j, glossyMaps, i, j);
                    Graphics.CopyTexture(textureSets[i].MetallMap, 0, j, metallMaps, i, j);
                    Graphics.CopyTexture(textureSets[i].NormalMap, 0, j, normalMaps, i, j);
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
            gameObject.transform.localScale = new Vector3(MapSize, MapSize, 0);
        }

        private void SetupShader()
        {
            computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.Raw);

            data = new int[MapSize, MapSize];

            for (int x = 0; x < MapSize; ++x)
            {
                for (int y = 0; y < MapSize; ++y)
                {
                    data[x, y] = hexBoard.Storage[x, y];
                    var t = new TileData((TileType)hexBoard.Storage[x, y], false);
                    int k = t.GetAsInt();
                    data[x, y] = k;
                }
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
            if (Test != null)
            {
                Vector3 normalized = WorldToNormalizedWorldPosition(Test.transform.position);
                HexagonData d = NormalizedWorldToHexagonPosition(normalized);

                if (d.hexagonPositionOffset.X > 0 && d.hexagonPositionOffset.X < MapSize - 1 &&
                    d.hexagonPositionOffset.Y > 0 && d.hexagonPositionOffset.Y < MapSize)
                {
                    MarkTileSelectedForNextFrame(d.hexagonPositionOffset.X, d.hexagonPositionOffset.Y);
                    for (int i = -1; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j)
                        {
                            if (i == 0 && j == 0)
                                continue;
                            MarkTileSelectedForNextFrame(d.hexagonPositionOffset.X + i, d.hexagonPositionOffset.Y + j);
                        }
                    }
                }
            }
            UpdateSelectedSet();
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            computeBuffer?.Dispose();
        }

        private static float Remap (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }


        public Vector2 WorldToNormalizedWorldPosition(Vector3 worldPosition)
        {
            var position = new Vector2(worldPosition.x, worldPosition.z);

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

            HexagonData hexagonData;

            hexagonData.hexagonPositionOffset = new Int2(x,z);
            hexagonData.hexagonPositionCubical = new Int3(rX,rZ,rY);
            hexagonData.hexagonPositionFloat = new Float2(posX, posY);

            return hexagonData;
        }

        private void UpdateSelectedSet()
        {
            foreach(Int2 tile in selectedSet)
            {
                int tileData = this.data[tile.Y, tile.X];
                var src = new TileData(tileData);
                src.SetSelected(true);
                this.data[tile.Y, tile.X] = src.GetAsInt();
            }

            computeBuffer.SetData(data);

            foreach(Int2 tile in selectedSet)
            {
                int tileData = this.data[tile.Y, tile.X];
                var src = new TileData(tileData);
                src.SetSelected(false);
                this.data[tile.Y, tile.X] = src.GetAsInt();
            }
            selectedSet.Clear();
        }

        public void MarkTileSelectedForNextFrame(int offsetX, int offsetY)
        {
            selectedSet.Add(new Int2(offsetX, offsetY));
        }

    }
}