using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ExtraMunitions : GenericUpgrade
    {
        public ExtraMunitions() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Extra Munitions",
                UpgradeType.Torpedo,
                cost: 2,
                feIsLimitedPerShip: true,
                abilityType: typeof(Abilities.FirstEdition.ExtraMunitionsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ExtraMunitionsAbility : GenericAbility
    {
        private List<GenericUpgrade> UpgradesWithOrdnanceToken = new List<GenericUpgrade>();
        private string OrdnanceTokenMarker = " (EM)";

        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += SetOrdnanceTokens;
            HostShip.OnDiscardUpgrade += CheckOrdnanceToken;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= SetOrdnanceTokens;
            HostShip.OnDiscardUpgrade -= CheckOrdnanceToken;
        }

        private void SetOrdnanceTokens()
        {
            foreach (var upgrade in HostShip.UpgradeBar.GetUpgradesOnlyFaceup())
            {
                if (upgrade.HasType(UpgradeType.Torpedo) || upgrade.HasType(UpgradeType.Missile) || upgrade.HasType(UpgradeType.Bomb))
                {
                    SetOrdnanceToken(upgrade);
                }
            }
        }

        private void SetOrdnanceToken(GenericUpgrade upgrade)
        {
            UpgradesWithOrdnanceToken.Add(upgrade);
            upgrade.NamePostfix += OrdnanceTokenMarker;

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public void RemoveOrdnanceToken(GenericUpgrade upgrade)
        {
            UpgradesWithOrdnanceToken.Remove(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade.NamePostfix.Replace(OrdnanceTokenMarker, "");
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private void CheckOrdnanceToken()
        {
            if (UpgradesWithOrdnanceToken.Contains(GenericUpgrade.CurrentUpgrade))
            {
                RegisterAbilityTrigger(TriggerTypes.OnDiscard, AskExtraMunitionsDecision);
            }
        }

        private void AskExtraMunitionsDecision(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbilityDecision, null, null, true);
            }
            else
            {
                DiscardTokenInstead(Triggers.FinishTrigger);
            }
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            DiscardTokenInstead(DecisionSubPhase.ConfirmDecision);
        }

        private void DiscardTokenInstead(Action callback)
        {
            Messages.ShowInfo("Ordnance token is discarded instead of " + GenericUpgrade.CurrentUpgrade.UpgradeInfo.Name);

            RemoveOrdnanceToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            callback();
        }
    }
}