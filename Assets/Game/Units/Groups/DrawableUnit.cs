using UnityEngine;
using System.Collections;

public abstract class DrawableUnit : UnitBase
{
    private Mesh mesh;

    public DrawableUnit(Mesh mesh)
    {
        this.mesh = mesh;
    }

    
}
