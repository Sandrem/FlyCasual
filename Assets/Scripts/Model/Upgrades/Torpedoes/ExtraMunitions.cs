using Upgrade;
using System.Collections.Generic;
using Abilities;
using SubPhases;
using System;

namespace UpgradesList
{
    public class ExtraMunitions : GenericUpgrade
    {
        public ExtraMunitions() : base()
        {
            Types.Add(UpgradeType.Torpedo);
            Name = "Extra Munitions";
            Cost = 2;
            isLimited = true;

            UpgradeAbilities.Add(new ExtraMunitionsAbility());
        }
    }
}

namespace Abilities
{
    public class ExtraMunitionsAbility : GenericAbility
    {
        private List<GenericUpgrade> upgradesWithOrdnanceToken = new List<GenericUpgrade>();
        private string ordnanceTokenMarker = " (EM)";

        public override void ActivateAbility()
        {
            Phases.OnGameStart += SetOrdnanceTokens;
            HostShip.OnDiscardUpgrade += CheckOrdnanceToken;
        }

        public override void DeactivateAbility()
        {
            Phases.OnGameStart -= SetOrdnanceTokens;
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
            upgradesWithOrdnanceToken.Add(upgrade);
            if (upgrade.Name != HostUpgrade.Name) upgrade.Name += ordnanceTokenMarker;

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public void RemoveOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceToken.Remove(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade.Name = GenericUpgrade.CurrentUpgrade.Name.Replace(ordnanceTokenMarker, "");
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private void CheckOrdnanceToken()
        {
            if (upgradesWithOrdnanceToken.Contains(GenericUpgrade.CurrentUpgrade))
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
            Messages.ShowInfo("Ordnance token is discarded instead of " + GenericUpgrade.CurrentUpgrade.Name);

            RemoveOrdnanceToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            callback();
        }
    }
}