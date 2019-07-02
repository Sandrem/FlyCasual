﻿using Upgrade;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class IntegratedAstromech : GenericUpgrade
    {
        public IntegratedAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Integrated Astromech",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.IntegratedAstromechAbility),
                restriction: new ShipRestriction(
                    typeof(Ship.FirstEdition.XWing.XWing),
                    typeof(Ship.FirstEdition.T70XWing.T70XWing)
                )
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class IntegratedAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RegisterIntegratedAstromechTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= RegisterIntegratedAstromechTrigger;
        }

        private void RegisterIntegratedAstromechTrigger(GenericShip ship)
        {
            if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n.HasType(UpgradeType.Astromech)) != 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskUseIntegratedAstromechAbility);
            }
        }

        private void AskUseIntegratedAstromechAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    SubPhases.DecisionSubPhase.ConfirmDecision();
                },
                descriptionLong: "Do you want to discard Astromech to discard current Damage card?",
                imageHolder: HostUpgrade
            );
        }

        private bool IsShouldUseAbility()
        {
            bool result = false;

            if (HostShip.State.HullCurrent == 2) result = true;
            if (Combat.CurrentCriticalHitCard.IsFaceup) result = true;

            return result;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            GenericUpgrade astromech = HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Find(n => n.HasType(UpgradeType.Astromech));
            if (astromech != null)
            {
                Sounds.PlayShipSound("R2D2-Killed");
                Messages.ShowInfo("Integrated Astromech causes " + astromech.UpgradeInfo.Name + " to take a hit instead of " + HostShip.PilotInfo.PilotName);
                Messages.ShowInfo(astromech.UpgradeInfo.Name + " has been discarded");
                Combat.CurrentCriticalHitCard = null;
                astromech.Discard(DiscardModification);
            }
            else
            {
                Messages.ShowError("Error: This ship has no astromech!");
                SubPhases.DecisionSubPhase.ConfirmDecision();
            }
        }

        private void DiscardModification()
        {
            HostUpgrade.Discard(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

    }
}