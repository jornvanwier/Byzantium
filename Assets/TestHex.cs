using JetBrains.Annotations;
using UnityEngine;

namespace Assets
{
    public class TestHex : MonoBehaviour
    {
        public Texture Texture;

        // Use this for initialization
        [UsedImplicitly]
        private void Start () {
            //add a mesh filter to the GO the script is attached to; cache it for later
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            //add a mesh renderer to the GO the script is attached to
            gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = PrimitiveMesh.Hexagon();
            gameObject.GetComponent<Renderer>().material.mainTexture = Texture;
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update () {
		
        }
    }
}
