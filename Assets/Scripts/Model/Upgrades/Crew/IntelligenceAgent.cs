using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;
using Abilities;
using System.Collections.Generic;
using BoardTools;
using UnityEngine;

namespace UpgradesList
{
    public class IntelligenceAgent : GenericUpgrade
    {
        public IntelligenceAgent() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Intelligence Agent";
            Cost = 1;
            
            UpgradeAbilities.Add(new IntelligenceAgentAbility());
        }
    }
}

namespace Abilities
{
    public class IntelligenceAgentAbility : GenericAbility
    {
        protected virtual int MinRange { get { return 1; } }
        protected virtual int MaxRange { get { return 2; } }

        public override void ActivateAbility()
        {
            Phases.OnActivationPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnActivationPhaseStart -= RegisterAbilityTrigger;
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
                NeverUseByDefault,
                delegate {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    StartSelectTargetForAbility(sender, e);
                }
            );
        }

        protected void StartSelectTargetForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                SeeAssignedManuver,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostUpgrade.Name,
                "Choose a ship to look at it's chosen maneuver.",
                HostUpgrade.ImageUrl
            );
        }

        protected virtual void SeeAssignedManuver()
        {
            if (TargetShip.Owner is Players.GenericAiPlayer)
            {
                Messages.ShowErrorToHuman("Useless against HotAC AI =)");
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
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, MinRange, MaxRange);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}
