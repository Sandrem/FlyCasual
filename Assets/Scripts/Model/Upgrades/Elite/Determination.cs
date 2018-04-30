using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

namespace UpgradesList
{

    public class Determination : GenericUpgrade
    {
        public Determination() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Determination";
            Cost = 1;

            UpgradeAbilities.Add(new DeterminationAbility());
        }
    }
}

namespace Abilities
{
    public class DeterminationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAssignCrit += CancelPilotCrits;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAssignCrit -= CancelPilotCrits;
        }

        private void CancelPilotCrits(GenericShip ship, DamageDeckCard.GenericDamageCard crit, EventArgs e)
        {
            if (crit.Type == CriticalCardType.Pilot) {
                Messages.ShowInfo("Determination: Crit with \"Pilot\" trait is discarded");
                crit = null;
            }
        }
    }
}
