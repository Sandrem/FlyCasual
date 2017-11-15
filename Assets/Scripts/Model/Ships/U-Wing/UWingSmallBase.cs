using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace UWing
    {
        public class UWingSmallBase : UWing
        {
            public UWingSmallBase() : base()
            {
                ShipBaseSize = BaseSize.Small;
            }
        }
    }
}
