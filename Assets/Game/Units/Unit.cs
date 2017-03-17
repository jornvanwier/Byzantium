using System;

namespace Assets.Game.Units
{
    public abstract class Unit : MovableBoardObject
    {
        protected Unit()
        {
        }

        public Unit Parent { get; set; }

        public virtual bool ShouldPathfind => Parent == null;

        protected override void Update()
        {
            if (ShouldPathfind)
            {
                base.Update();
            }
        }

        public abstract void Order();
    }
}