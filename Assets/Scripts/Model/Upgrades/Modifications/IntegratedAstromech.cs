using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship.XWing;
using Ship.T70XWing;
using Abilities;
using System.Linq;
using DamageDeckCard;

namespace UpgradesList
{
    public class IntegratedAstromech : GenericUpgrade
    {
        public IntegratedAstromech() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Integrated Astromech";
            Cost = 0;

            UpgradeAbilities.Add(new IntegratedAstromechAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing || ship is T70XWing;
        }
    }
}

namespace Abilities
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
            if (HostShip.UpgradeBar.GetInstalledUpgrades().Count(n => n.Type == UpgradeType.Astromech) != 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskUseIntegratedAstromechAbility);
            }
        }

        private void AskUseIntegratedAstromechAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    SubPhases.DecisionSubPhase.ConfirmDecision();
                });
        }

        private bool IsShouldUseAbility()
        {
            bool result = false;

            if (HostShip.Hull == 2) result = true;
            if (Combat.CurrentCriticalHitCard.IsFaceup) result = true;

            return result;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            GenericUpgrade astromech = HostShip.UpgradeBar.GetInstalledUpgrades().Find(n => n.Type == UpgradeType.Astromech);
            if (astromech != null)
            {
                Sounds.PlayShipSound("R2D2-Killed");
                Messages.ShowInfo("Integrated Astromech is used");
                Messages.ShowInfo(astromech.Name + " is discarded");
                Combat.CurrentCriticalHitCard = null;
                astromech.Discard(DiscardModification);
            }
            else
            {
                Messages.ShowError("Error: No astromech to disacard!");
                SubPhases.DecisionSubPhase.ConfirmDecision();
            }
        }

        private void DiscardModification()
        {
            HostUpgrade.Discard(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

    }
}