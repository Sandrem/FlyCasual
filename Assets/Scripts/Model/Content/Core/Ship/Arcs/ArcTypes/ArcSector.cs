using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;
using ActionsList;

namespace Arcs
{

    public class ArcSector : ArcSingleTurret
    {
        public ArcSector(GenericShipBase shipBase) : base(shipBase)
        {

        }

        protected override void SubscribeToShipSetup(GenericShip host)
        {
            
        }

        public override void ShowMobileArcPointer()
        {

        }
    }
}
