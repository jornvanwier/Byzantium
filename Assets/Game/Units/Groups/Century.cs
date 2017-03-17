<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class Century : UnitBase, IMultipleUnits<Contubernium>
{
    public void AddUnit(Contubernium unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Contubernium unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Contubernium> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        public void AddUnit(Contubernium unit)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Contubernium> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(Contubernium unit)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
>>>>>>> 828546e54cf1a61a949323272da9e5e8b9a713ec
