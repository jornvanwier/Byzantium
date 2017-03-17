<<<<<<< HEAD
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohort : UnitBase, IMultipleUnits<Century>
{
    public void AddUnit(Century unit)
    {
        throw new NotImplementedException();
    }

    public void RemoveUnit(Century unit)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Century> GetEnumerator()
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
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        public void AddUnit(Century unit)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Century> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(Century unit)
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
