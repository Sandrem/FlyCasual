using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;

namespace Upgrade
{

    abstract public class GenericContactMineSE : GenericContactMineFE
    {
        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Editions.Edition.Current.TimedBombActivationTime(host);
        }

        protected override void PerformDropBombAction(GenericShip ship) {}
    }

}