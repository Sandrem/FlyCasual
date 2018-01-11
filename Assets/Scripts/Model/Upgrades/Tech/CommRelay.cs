using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Tokens;

namespace UpgradesList
{

    public class CommRelay : GenericUpgrade
    {
        public CommRelay() : base()
        {
            Type = UpgradeType.Tech;
            Name = "Comm Relay";
            Cost = 3;

            UpgradeAbilities.Add(new CommRelayAbility());
        }
    }

}

namespace Abilities
{
    public class CommRelayAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeTokenIsAssigned += CommRelayRestriction;
            HostShip.OnTokenIsAssigned += CommRelayBonus;

            ToggleBonusOfExistingEvadeTokens(true);
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeTokenIsAssigned -= CommRelayRestriction;
            HostShip.OnTokenIsAssigned -= CommRelayBonus;

            ToggleBonusOfExistingEvadeTokens(false);
        }

        private void CommRelayRestriction(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(EvadeToken) && ship.HasToken(typeof(EvadeToken)))
            {
                Messages.ShowError("Comm Relay: Cannon have more than 1 Evade token");
                ship.TokenToAssign = null;
            }
        }

        private void CommRelayBonus(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(EvadeToken))
            {
                ToggleBonusOfExistingEvadeTokens(true);
            }
        }

        private void ToggleBonusOfExistingEvadeTokens(bool isActive)
        {
            EvadeToken evadeToken = (EvadeToken)HostShip.GetToken(typeof(EvadeToken));
            if (evadeToken != null)
            {
                evadeToken.Count = 1;
                evadeToken.Temporary = !isActive;
            }
        }
    }
}
