using Upgrade;
using System.Collections.Generic;
using Abilities;
using SubPhases;
using System;
using Ship;
using Ship.BSF17Bomber;
using UnityEngine;

namespace UpgradesList
{
    public class OrdnanceSilos : GenericUpgrade
    {
        public OrdnanceSilos() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Ordnance Silos";
            Cost = 2;

            UpgradeAbilities.Add(new OrdnanceSilosAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is BSF17Bomber;
        }
    }
}

namespace Abilities
{
    public class OrdnanceSilosAbility : GenericAbility
    {
        private Dictionary<GenericUpgrade, int> upgradesWithOrdnanceTokens = new Dictionary<GenericUpgrade, int>();
        private string ordnanceTokenMarker = " (4)";

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
                if (upgrade.HasType(UpgradeType.Bomb))
                {
                    SetOrdnanceTokens(upgrade);
                }
            }
        }

        private void SetOrdnanceTokens(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceTokens.Add(upgrade, 3);
            if (upgrade.Name != HostUpgrade.Name) upgrade.Name += ordnanceTokenMarker;

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public void RemoveOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceTokens[GenericUpgrade.CurrentUpgrade]--;

            int tokensLeft = upgradesWithOrdnanceTokens[GenericUpgrade.CurrentUpgrade];
            Debug.Log(tokensLeft);
            if (tokensLeft == 0)
            {
                GenericUpgrade.CurrentUpgrade.Name = GenericUpgrade.CurrentUpgrade.Name.Replace(" (2)", "");
                upgradesWithOrdnanceTokens.Remove(GenericUpgrade.CurrentUpgrade);
            }
            else
            {
                Debug.Log(GenericUpgrade.CurrentUpgrade.Name);
                GenericUpgrade.CurrentUpgrade.Name = GenericUpgrade.CurrentUpgrade.Name.Replace(" (" + (tokensLeft+2) + ")", " (" + (tokensLeft+1) + ")");
            }

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private void CheckOrdnanceToken()
        {
            if (upgradesWithOrdnanceTokens.ContainsKey(GenericUpgrade.CurrentUpgrade))
            {
                RegisterAbilityTrigger(TriggerTypes.OnDiscard, AskUseTokenInstead);
            }
        }

        private void AskUseTokenInstead(object sender, EventArgs e)
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