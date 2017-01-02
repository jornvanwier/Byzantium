using JetBrains.Annotations;
using UnityEngine;

namespace Assets
{
    public class Map : MonoBehaviour
    {
        public Mesh DrawMesh;
        public int Size = 512;

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            HexBoard hb = new HexBoard(DrawMesh, Size);

//            hb.LogString();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
        }
    }
}