using UnityEngine;
using Worldgen;
using System.Collections.Generic;


public enum TileType : byte
{
    Grass,
    Water,
}


public class Generator : MonoBehaviour {

    List<GameObject> HexagonGameObjects;
    MaterialPropertyBlock GrassProps;
    MaterialPropertyBlock WaterProps;
    Material mat;
    TileType[,] Hexes;
    byte[,] UpdateSet;


    
    GameObject thisObject;

    public int size = 1;

    // Use this for initialization
    void Start () {
        Hexes = new TileType[size,size];
        UpdateSet = new byte[size,size];

        HexagonGameObjects = new List<GameObject>();

        GrassProps = new MaterialPropertyBlock();
        WaterProps = new MaterialPropertyBlock();

        GrassProps.SetColor("_Color", new Color(0, 1, 0));
        WaterProps.SetColor("_Color", new Color(0, 0, 1));
        mat = Resources.Load("HexMat", typeof(Material)) as Material;


        thisObject = GameObject.Find("Core");
        Mesh HexMesh = WorldGenerator.GenerateHexagonMesh(0.5f);



        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                GameObject g = new GameObject();
                MeshFilter f = g.AddComponent<MeshFilter>();
                MeshRenderer r = g.AddComponent<MeshRenderer>();
                f.sharedMesh = HexMesh;
                r.material = mat;

                g.name = "TILE: " + x + " - " + y;
                g.transform.position = new Vector3(x,0,y);
                g.transform.parent = thisObject.transform;
                g.transform.localEulerAngles = new Vector3(90,0,0);
                HexagonGameObjects.Add(g);
            }
        }


        System.Random ra = new System.Random(238947);

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                Hexes[x, y] = (TileType)ra.Next(0, 2);
                UpdateSet[x, y] = 1;
            }
        }


    }
	
	// Update is called once per frame
	void Update () {
        foreach (GameObject g in HexagonGameObjects)
        {
            if (UpdateSet[(int)g.transform.position.x, (int)g.transform.position.y] == 0)
                continue;
            //UpdateSet[(int)g.transform.position.x, (int)g.transform.position.y] = 0;

            if (Hexes[(int)g.transform.position.x, (int)g.transform.position.z] == TileType.Grass)
            {
                MeshRenderer rd = g.GetComponent<MeshRenderer>();
                rd.SetPropertyBlock(GrassProps);
            }
            else
            {
                MeshRenderer rd = g.GetComponent<MeshRenderer>();
                rd.SetPropertyBlock(WaterProps);
            }
        }
    }
}
