using Ship;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class PatternAnalyzer : GenericUpgrade
    {
        public PatternAnalyzer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pattern Analyzer",
                UpgradeType.Tech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.PatternAnalyzerAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
