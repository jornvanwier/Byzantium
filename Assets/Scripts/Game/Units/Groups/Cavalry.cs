using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

        private Cavalry(Faction faction)
        {
            Commander = new Commander(this, faction);
        }
        
        public override int Health
        {
            get { return drawableUnits[0].Health; }
            set
            {
                foreach (MeshDrawableUnit meshDrawableUnit in drawableUnits)
                    meshDrawableUnit.Health = value;
            }
        }
        
        public override float DefaultSpeed => 1.5f;

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (UnitBase child in this)
                    child.Rotation = value;
            }
        }

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public static Cavalry CreatePikeUnit(Faction faction)
        {
            var cavalry = new Cavalry(faction)
            {
                Formation = new SquareFormation()
            };

            for (int i = 0; i < 64; i++)
            {
                cavalry.AddUnit(new MeshDrawableUnit(
                    Defense.None,
                    Weapon.Pike,
                    Soldier.Mounted
                    ));
            }

            return cavalry;
        }

        public override int UnitCount => drawableUnits.Count;

        public override Vector2 DrawSize => Vector2.Scale(drawableUnits[0].DrawSize * 2, ChildrenDimensions);

        public override IEnumerable<MeshDrawableUnit> AllUnits => drawableUnits;

        public void AddUnit(MeshDrawableUnit unit)
        {
            drawableUnits.Add(unit);
        }

        public void RemoveUnit(MeshDrawableUnit unit)
        {
            int index = drawableUnits.IndexOf(unit);
            drawableUnits.RemoveAt(index);
        }

        public override void Draw()
        {
            foreach (MeshDrawableUnit unit in this)
                unit.Draw();
        }

        IEnumerator<MeshDrawableUnit> IEnumerable<MeshDrawableUnit>.GetEnumerator()
        {
            return drawableUnits.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<MeshDrawableUnit>)this).GetEnumerator();
        }
    }
}