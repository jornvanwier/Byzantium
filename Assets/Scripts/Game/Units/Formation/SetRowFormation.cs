using System;
using System.Collections.Generic;
using Assets.Scripts.Map;
using Game.Units;
using Game.Units.Formation;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    public class SetRowFormation : FormationBase
    {
        public override void Order(Legion unit)
        {
            throw new NotImplementedException();
        }

        public override void Order(Contubernium unit)
        {
            SquareFormation.OrderAnySetRow<Contubernium, MeshDrawableUnit>(4, unit);
        }

        public override void Order(Cavalry unit)
        {
            SquareFormation.OrderAnySetRow<Cavalry, MeshDrawableUnit>(3, unit);
        }

        public override void Order(Cohort unit)
        {
            SquareFormation.OrderAnySetRow<Cohort, Century>(3, unit);
        }

        public override void Order(Century unit)
        {
            SquareFormation.OrderAnySetRow<Century, Contubernium>(2, unit);
        }
    }
}