using UnityEngine;
using System.Collections;

public abstract class DrawableUnit : UnitBase
{
    private Mesh mesh;

    Defense defenseType;
    Weapon weaponType;
    Movement movementType;

    public DrawableUnit(Mesh mesh, Defense defense = Defense.Armor, Weapon weapon = Weapon.Sword, Movement movement = Movement.Foot)
    {
        this.mesh = mesh;

        defenseType = defense;
        weaponType = weapon;
        movementType = movement;
    }

    



}
