using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class MoldyCrow : GenericUpgrade
    {
        public MoldyCrow() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Moldy Crow",
                UpgradeType.Title,
                cost: 3, 
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Hwk290.Hwk290)),
                abilityType: typeof(Abilities.FirstEdition.MoldyCrowAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class MoldyCrowAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase += KeepFocusTokens;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase -= KeepFocusTokens;
        }

        private void KeepFocusTokens(GenericToken token, ref bool remove)
        {
            if (token is FocusToken) remove = false;
        }
    }
}