using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Tokens;

namespace UpgradesList
{

    public class PatternAnalyzer : GenericUpgrade
    {
        public PatternAnalyzer() : base()
        {
            Types.Add(UpgradeType.Tech);
            Name = "Pattern Analyzer";
            Cost = 2;

            UpgradeAbilities.Add(new PatternAnalyzerAbility());
        }
    }

}

namespace Abilities
{
    public class PatternAnalyzerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += RegisterPatternAnalyzer;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= RegisterPatternAnalyzer;
        }

        private void RegisterPatternAnalyzer(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, ShowUsePatternAnalyzerDecision);
        }

        private void ShowUsePatternAnalyzerDecision(object sender, System.EventArgs e)
        {
            // give user the option to use ability
            AskToUseAbility(AlwaysUseByDefault, UsePatternAnalyzer);
        }

        private void UsePatternAnalyzer(object sender, System.EventArgs e)
        {
            HostShip.OnMovementExecuted -= Rules.Stress.PlanCheckStress;
            HostShip.OnActionDecisionSubphaseEnd += PlanCheckStress;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
        
        public void PlanCheckStress(GenericShip ship)
        {             
            HostShip.OnMovementExecuted += Rules.Stress.PlanCheckStress;
            HostShip.OnActionDecisionSubphaseEnd -= PlanCheckStress;
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Check stress ( " + ship.PilotName + " )",
                Skippable = true,
                TriggerOwner = HostShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                EventHandler = RulesList.StressRule.CheckStress
            });
        }
    }
}
