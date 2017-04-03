using System;

namespace Assets.Scripts.Game.Units.Formation
{
    public class FormationIncompatibleException : Exception
    {
        public FormationIncompatibleException(UnitBase unit) : 
            base("The current formation cannot be appplied to an unit of type " + unit.GetType())
        {
        }
    }
}