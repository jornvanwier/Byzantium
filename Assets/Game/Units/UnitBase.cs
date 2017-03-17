using UnityEngine;

namespace Assets.Game.Units
{
    public abstract class UnitBase
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}