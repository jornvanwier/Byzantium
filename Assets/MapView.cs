using System.Collections;
using System.Collections.Generic;
using HexBoard;
using UnityEngine;

public class MapView : MonoBehaviour
{
    private ComputeBuffer _computeBuffer;
    private Map _map;

    public int MapSize = 1;
    public Mesh mesh;

    [Range(0,25)]
    public int iterations = 1;

    public Material HexMaterial;

	void Start () {
		_map = new Map(MapSize);
	    _map.generateMap();

	    _computeBuffer = new ComputeBuffer(MapSize * MapSize, sizeof(int), ComputeBufferType.GPUMemory);

	    int[,] data = new int[MapSize,MapSize];

	    for (int x = 0; x < MapSize; ++x)
	    {
	        for (int y = 0; y < MapSize; ++y)
	        {
	            data[x, y] = _map.getMapArray()[x,y];
	        }
	    }

	    _computeBuffer.SetData(data);

	    MaterialPropertyBlock block = new MaterialPropertyBlock();
	    block.SetFloat(Shader.PropertyToID("_ArraySize"), MapSize);
	    block.SetBuffer(Shader.PropertyToID("hexProps"), _computeBuffer);

	    var filter = gameObject.AddComponent<MeshFilter>();
	    var mrenderer = gameObject.AddComponent<MeshRenderer>();
	    filter.sharedMesh = mesh;
	    mrenderer.material = HexMaterial;
	    mrenderer.SetPropertyBlock(block);
	}

	void Update () {
		
	}
}
