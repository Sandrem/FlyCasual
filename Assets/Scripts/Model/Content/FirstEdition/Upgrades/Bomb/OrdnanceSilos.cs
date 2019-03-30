using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class OrdnanceSilos : GenericUpgrade
    {
        public OrdnanceSilos() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ordnance Silos",
                UpgradeType.Bomb,
                cost: 2,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.BSF17Bomber.BSF17Bomber)),
                abilityType: typeof(Abilities.FirstEdition.OrdnanceSilosAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class OrdnanceSilosAbility : GenericAbility
    {
        private Dictionary<GenericUpgrade, int> upgradesWithOrdnanceTokens = new Dictionary<GenericUpgrade, int>();
        private string OrdnanceTokenMarker = " (4)";

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
                if (upgrade.HasType(UpgradeType.Bomb))
                {
                    SetOrdnanceTokens(upgrade);
                }
            }
        }

        private void SetOrdnanceTokens(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceTokens.Add(upgrade, 3);
            upgrade.NamePostfix += OrdnanceTokenMarker;

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public void RemoveOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceTokens[GenericUpgrade.CurrentUpgrade]--;

            int tokensLeft = upgradesWithOrdnanceTokens[GenericUpgrade.CurrentUpgrade];
            Debug.Log(tokensLeft);
            if (tokensLeft == 0)
            {
                upgrade.NamePostfix = upgrade.NamePostfix.Replace(" (2)", "");
                upgradesWithOrdnanceTokens.Remove(GenericUpgrade.CurrentUpgrade);
            }
            else
            {
                Debug.Log(GenericUpgrade.CurrentUpgrade.UpgradeInfo.Name);
                upgrade.NamePostfix = upgrade.NamePostfix.Replace(" (" + (tokensLeft + 2) + ")", " (" + (tokensLeft + 1) + ")");
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
            Messages.ShowInfo("An ordinance token has been discarded instead of " + GenericUpgrade.CurrentUpgrade.UpgradeInfo.Name + ".");

            RemoveOrdnanceToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            callback();
        }
    }
}