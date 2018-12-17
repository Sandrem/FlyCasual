﻿using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class InstinctiveAim : GenericUpgrade
    {
        public InstinctiveAim() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Instinctive Aim",
                UpgradeType.Force,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.InertialDampenersAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small),
                seImageNumber: 20
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class InstinctiveAimAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateAvailableAttackPaymentList += AddForceTokenAsAlternative;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateAvailableAttackPaymentList -= AddForceTokenAsAlternative;
        }

        private void AddForceTokenAsAlternative(List<GenericToken> waysToPay)
        {
            if (HostShip.State.Force > 0)
            {
                GenericToken forceToken = HostShip.Tokens.GetToken(typeof(ForceToken));
                waysToPay.Add(forceToken);
            }
        }
    }
}