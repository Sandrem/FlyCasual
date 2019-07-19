using Ship;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class PatternAnalyzer : GenericUpgrade
    {
        public PatternAnalyzer() : base()
        {
            // TODO: Too many bugs reported, full rework is required
            IsHidden = true;

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
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UsePatternAnalyzer,
                descriptionLong: "Do you want to resolve the \"Check Pilot Stress\" step after the \"Perform Action\" step (instead of before that step)?",
                imageHolder: HostUpgrade
            );
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
                Name = "Check stress ( " + ship.PilotInfo.PilotName + " )",
                Skippable = true,
                TriggerOwner = HostShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                EventHandler = RulesList.StressRule.CheckStress
            });
        }
    }
}
