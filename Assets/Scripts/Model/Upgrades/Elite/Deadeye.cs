using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using Tokens;
using System.Linq;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class Deadeye : GenericUpgrade, ISecondEditionUpgrade
    {
        public Deadeye() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Deadeye";
            Cost = 1;

            AvatarOffset = new Vector2(38, 3);

            UpgradeAbilities.Add(new DeadeyeAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Name = "Instinctive Aim";

            ImageUrl = "https://i.imgur.com/07f1bDf.png";
        }
    }
}

namespace Abilities
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