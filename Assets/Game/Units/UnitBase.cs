using UnityEngine;

namespace Assets.Game.Units
{
    public abstract class UnitBase
    {
        private Vector3 position        = Vector3.zero;
        private Quaternion rotation     = Quaternion.identity;
        private IFormation formation    = new VerticalLineFormation();

        public virtual Vector3 Position     { get { return position;  } set { position = value;     } }
        public virtual Quaternion Rotation  { get { return rotation;  } set { rotation = value;     } }
        public virtual IFormation Formation { get { return formation; } set { formation = value;    } }

        public abstract void Draw();
        public abstract float WalkSpeed();
        public abstract void  WalkSpeed(float speed);
    }
}