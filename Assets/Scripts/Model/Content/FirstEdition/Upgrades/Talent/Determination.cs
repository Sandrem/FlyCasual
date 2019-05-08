using Ship;
using Upgrade;
using System.Collections.Generic;
using System;

namespace UpgradesList.FirstEdition
{
    public class Determination : GenericUpgrade
    {
        public Determination() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Determination",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.DeterminationAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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

        private void CancelPilotCrits(GenericShip ship, GenericDamageCard crit, EventArgs e)
        {
            if (crit.Type == CriticalCardType.Pilot)
            {
                Messages.ShowInfo("Determination causes " + ship.PilotInfo.PilotName + " to discard " + crit.Name + " since it has the \"Pilot\" trait");
                crit = null;
            }
        }
    }
}