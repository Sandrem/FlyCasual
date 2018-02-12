using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class Determination : GenericUpgrade
    {

        public Determination() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Determination";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAssignCrit += CancelPilotCrits;
        }

        private void CancelPilotCrits(Ship.GenericShip ship, DamageDeckCard.GenericDamageCard crit, EventArgs e)
        {
            if (crit.Type == CriticalCardType.Pilot) {
                Messages.ShowInfo("Determination: Crit with \"Pilot\" trait is discarded");
                crit = null;
            }
        }

    }

}
