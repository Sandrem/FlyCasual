using ActionsList;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;
namespace UpgradesList.SecondEdition
{
    public class PatternAnalyzer : GenericUpgrade
    {
        public PatternAnalyzer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pattern Analyzer",
                UpgradeType.Tech,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.PatternAnalyzerAbility)//,
                //seImageNumber: 69
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ace963fc4fe9d97f64ab8564dc4beae7.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PatternAnalyzerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += RegisterPatternAnalyzer;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= RegisterPatternAnalyzer;
        }

        private void RegisterPatternAnalyzer(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, UsePatternAnalyzer);
            }
        }

        private void UsePatternAnalyzer(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}