using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets
{
    public class Floor : MonoBehaviour
    {
        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            this.transform.Translate(0, 0.1f, 0);

            byte val = Convert.ToByte("10101110", 2);
            byte mask = Convert.ToByte("00000011", 2);

            Debug.Log("Terraintype: " + (val >> 2));
            Debug.Log("Metadata: " + (val & mask));
        }
    }
}