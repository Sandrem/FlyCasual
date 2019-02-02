using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class JabbaTheHutt : GenericUpgrade
    {
        public JabbaTheHutt() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jabba The Hutt",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.JabbaTheHuttAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(58, 5));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class JabbaTheHuttAbility : GenericAbility
    {
        private List<GenericUpgrade> UpgradesWithIllicitTokens = new List<GenericUpgrade>();
        private string IllicitTokenMarker = " (2)";

        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += SetIllicitTokens;
            GenericShip.OnDiscardUpgradeGlobal += CheckIllicitTokens;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= SetIllicitTokens;
            Phases.Events.OnGameEnd += ClearAbility;
        }

        private void ClearAbility()
        {
            Phases.Events.OnGameStart -= ClearAbility;
            GenericShip.OnDiscardUpgradeGlobal -= CheckIllicitTokens;
        }

        private void SetIllicitTokens()
        {
            foreach (var ship in HostShip.Owner.Ships.Values)
            {
                foreach (var upgrade in ship.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    if (upgrade.HasType(UpgradeType.Illicit))
                    {
                        SetIllicit(upgrade);
                    }
                }
            }

        }

        private void SetIllicit(GenericUpgrade upgrade)
        {
            UpgradesWithIllicitTokens.Add(upgrade);
            upgrade.NamePostfix += IllicitTokenMarker;

            Roster.UpdateUpgradesPanel(upgrade.HostShip, upgrade.HostShip.InfoPanel);
        }

        public void RemoveIllicitToken(GenericUpgrade upgrade)
        {
            UpgradesWithIllicitTokens.Remove(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade.NamePostfix.Replace(IllicitTokenMarker, "");
            Roster.UpdateUpgradesPanel(GenericUpgrade.CurrentUpgrade.HostShip, GenericUpgrade.CurrentUpgrade.HostShip.InfoPanel);
        }

        private void CheckIllicitTokens()
        {
            if (UpgradesWithIllicitTokens.Contains(GenericUpgrade.CurrentUpgrade))
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
            Messages.ShowInfo("Illicit token is discarded instead of " + GenericUpgrade.CurrentUpgrade.UpgradeInfo.Name);

            RemoveIllicitToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            callback();
        }
    }
}