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

    public class AdvancedOptics : GenericUpgrade
    {
        public AdvancedOptics() : base()
        {
            Type = UpgradeType.Tech;
            Name = "Advanced Optics";
            Cost = 2;

            UpgradeAbilities.Add(new AdvancedOpticsAbility());
        }
    }

}

namespace Abilities
{
    public class AdvancedOpticsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeTokenIsAssigned += AdvancedOpticsRestriction;
            HostShip.OnTokenIsAssigned += AdvancedOpticsBonus;

            ToggleBonusOfExistingFocusTokens(true);
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeTokenIsAssigned -= AdvancedOpticsRestriction;
            HostShip.OnTokenIsAssigned -= AdvancedOpticsBonus;

            ToggleBonusOfExistingFocusTokens(false);
        }

        private void AdvancedOpticsRestriction(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(FocusToken) && ship.HasToken(typeof(FocusToken)))
            {
                Messages.ShowError("Advanced Optics: Cannon have more than 1 Focus token");
                ship.TokenToAssign = null;
            }
        }

        private void AdvancedOpticsBonus(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(FocusToken))
            {
                ToggleBonusOfExistingFocusTokens(true);
            }
        }

        private void ToggleBonusOfExistingFocusTokens(bool isActive)
        {
            FocusToken FocusToken = (FocusToken)HostShip.GetToken(typeof(FocusToken));
            if (FocusToken != null)
            {
                FocusToken.Temporary = !isActive;
            }
        }
    }
}
