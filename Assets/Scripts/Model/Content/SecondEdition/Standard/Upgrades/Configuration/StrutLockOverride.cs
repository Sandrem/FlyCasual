using Actions;
using ActionsList;
using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class StrutLockOverride : GenericUpgrade
    {
        public StrutLockOverride() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Strut-Lock Override",
                UpgradeType.Configuration,
                cost: 0,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.StrutLockOverrideAbility)
            );
            
            ImageUrl = "https://i.imgur.com/enlmZTV.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StrutLockOverrideAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToIgnoreObstacles);
            }
        }

        private void AskToIgnoreObstacles(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                ActivateStrutAbility,
                descriptionLong: "Do you want to spend 1 Charge to ignore obstacles while you move through them this round?"
            );
        }

        private void ActivateStrutAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.IsIgnoreObstacles = true;
            HostShip.OnMovementActivationFinish += RemoveStrutsAbility;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RemoveStrutsAbility(GenericShip ship)
        {
            HostShip.IsIgnoreObstacles = false;
            HostShip.OnMovementActivationFinish -= RemoveStrutsAbility;
        }
    }
}