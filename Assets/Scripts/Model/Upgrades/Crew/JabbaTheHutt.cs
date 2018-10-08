using Abilities;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class JabbaTheHutt : GenericUpgrade
    {
        public JabbaTheHutt() : base()
        {
            Types.Add(UpgradeType.Crew);
            Types.Add(UpgradeType.Crew);
            Name = "Jabba the Hutt";
            Cost = 5;

            isUnique = true;

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(58, 5));

            UpgradeAbilities.Add(new JabbaTheHuttAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
{
    public class JabbaTheHuttAbility : GenericAbility
    {
        private List<GenericUpgrade> upgradesWithIllicitTokens = new List<GenericUpgrade>();
        private string illicitTokenMarker = " (2)";

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
            upgradesWithIllicitTokens.Add(upgrade);
            upgrade.Name += illicitTokenMarker;

            Roster.UpdateUpgradesPanel(upgrade.Host, upgrade.Host.InfoPanel);
        }

        public void RemoveIllicitToken(GenericUpgrade upgrade)
        {
            upgradesWithIllicitTokens.Remove(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade.Name = GenericUpgrade.CurrentUpgrade.Name.Replace(illicitTokenMarker, "");
            Roster.UpdateUpgradesPanel(GenericUpgrade.CurrentUpgrade.Host, GenericUpgrade.CurrentUpgrade.Host.InfoPanel);
        }

        private void CheckIllicitTokens()
        {
            if (upgradesWithIllicitTokens.Contains(GenericUpgrade.CurrentUpgrade))
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
            Messages.ShowInfo("Illicit token is discarded instead of " + GenericUpgrade.CurrentUpgrade.Name);

            RemoveIllicitToken(GenericUpgrade.CurrentUpgrade);

            GenericUpgrade.CurrentUpgrade = null;

            callback();
        }
    }
}