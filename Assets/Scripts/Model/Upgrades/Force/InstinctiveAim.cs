using Upgrade;
using UnityEngine;
using Ship;
using Abilities;
using RuleSets;
using Tokens;
using System.Collections.Generic;

namespace UpgradesList
{
    public class InstinctiveAim : GenericUpgrade
    {
        public InstinctiveAim() : base()
        {
            Types.Add(UpgradeType.Force);
            Name = "Instinctive Aim";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new InstinctiveAimAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
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
            if (HostShip.Force > 0)
            {
                GenericToken forceToken = HostShip.Tokens.GetToken(typeof(ForceToken));
                waysToPay.Add(forceToken);
            }
        }
    }
}