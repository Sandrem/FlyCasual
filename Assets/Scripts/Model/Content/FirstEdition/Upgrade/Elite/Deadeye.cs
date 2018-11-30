using Ship;
using Upgrade;
using Tokens;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class Deadeye : GenericUpgrade
    {
        public Deadeye() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Deadeye",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.DeadeyeAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(38, 3));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DeadeyeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateAvailableAttackPaymentList += AddFocusTokenAsPayment;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateAvailableAttackPaymentList -= AddFocusTokenAsPayment;
        }

        private void AddFocusTokenAsPayment(List<GenericToken> waysToPay)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                GenericToken focus = HostShip.Tokens.GetToken(typeof(FocusToken));
                waysToPay.Add(focus);
            }
        }
    }
}