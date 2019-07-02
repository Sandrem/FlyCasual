﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;
using SubPhases;
using BoardTools;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class IntelligenceAgent : GenericUpgrade
    {
        public IntelligenceAgent() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Intelligence Agent",
                UpgradeType.Crew,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.IntelligenceAgentAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class IntelligenceAgentAbility : GenericAbility
    {
        protected virtual int MinRange { get { return 1; } }
        protected virtual int MaxRange { get { return 2; } }

        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToUseIntelligenceAgentAbility);
            }
        }

        private void AskToUseIntelligenceAgentAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                delegate {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    StartSelectTargetForAbility(sender, e);
                },
                descriptionLong: "Do you want to choose 1 enemy ship at Range 1-2 to look at that ship's chosen maneuver?",
                imageHolder: HostUpgrade
            );
        }

        protected void StartSelectTargetForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                SeeAssignedManuver,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose a ship to look at it's chosen maneuver",
                HostUpgrade
            );
        }

        protected virtual void SeeAssignedManuver()
        {
            if (TargetShip.Owner is Players.HotacAiPlayer)
            {
                Messages.ShowErrorToHuman("This ability is useless against HotAC AI =)");
            }
            else
            {
                Roster.ToggleManeuverVisibility(TargetShip, true);
                TargetShip.AlwaysShowAssignedManeuver = true;
            }

            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy })
                && FilterTargetsByRange(ship, MinRange, MaxRange)
                && ship.AssignedManeuver != null;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}