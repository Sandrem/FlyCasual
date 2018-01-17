using Upgrade;
using System.Collections.Generic;
using Abilities;
using SubPhases;

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
            foreach (var upgrade in HostShip.UpgradeBar.GetInstalledUpgrades())
            {
                if (upgrade.hasType(UpgradeType.Torpedo) || upgrade.hasType(UpgradeType.Missile) || upgrade.hasType(UpgradeType.Bomb))
                {
                    SetOrdnanceToken(upgrade);
                }
            }
        }

        private void SetOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceToken.Add(upgrade);
            if (upgrade.Name != Name) upgrade.Name += ordnanceTokenMarker;

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

        private void AskExtraMunitionsDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Ordnance token is discarded instead of " + GenericUpgrade.CurrentUpgrade.Name);

            RemoveOrdnanceToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            DecisionSubPhase.ConfirmDecision();
        }
    }
}