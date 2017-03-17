<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
{
    public void AddUnit(DrawableUnit unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(DrawableUnit unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<DrawableUnit> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
    {
        public void AddUnit(DrawableUnit unit)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<DrawableUnit> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(DrawableUnit unit)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
>>>>>>> 828546e54cf1a61a949323272da9e5e8b9a713ec
}