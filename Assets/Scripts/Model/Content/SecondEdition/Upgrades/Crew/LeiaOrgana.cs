﻿using Ship;
using Upgrade;
using SubPhases;
using System.Linq;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class LeiaOrgana : GenericUpgrade
    {
        public LeiaOrgana() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Leia Organa",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                charges: 3,
                regensCharges: true,
                abilityType: typeof(Abilities.SecondEdition.LeiaOrganaAbility),
                seImageNumber: 88
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LeiaOrganaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActivationPhaseStart += CheckRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActivationPhaseStart -= CheckRegisterAbility;
        }

        private void CheckRegisterAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges >= 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskLeiaAbility);
            }
        }

        private void AskLeiaAbility(object sender, System.EventArgs e)
        {
            if (HostUpgrade.State.Charges >= 3)
            {
                AskToUseAbility(NeverUseByDefault, UseLeiaAbility);
            }
            else
            {
                Messages.ShowError(HostUpgrade.UpgradeInfo.Name + " does not have enough charges to activate.");
                Triggers.FinishTrigger();
            }
        }

        private void UseLeiaAbility(object sender, System.EventArgs e)
        {
            HostUpgrade.State.SpendCharges(3);

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + "'s ability was successfully activated.");

            GenericShip.OnManeuverIsReadyToBeRevealedGlobal += CheckReduceComplexity;
            Phases.Events.OnRoundEnd += ClearAbility;

            DecisionSubPhase.ConfirmDecision();
        }

        private void CheckReduceComplexity(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                ship.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(ship.AssignedManeuver.ColorComplexity);
            }
        }

        private void ClearAbility()
        {
            GenericShip.OnManeuverIsReadyToBeRevealedGlobal -= CheckReduceComplexity;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }
    }
}