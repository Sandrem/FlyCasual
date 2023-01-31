using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class PerceptiveCopilot : GenericUpgrade
    {
        public PerceptiveCopilot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Perceptive Copilot",
                UpgradeType.Crew,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.PerceptiveCopilotAbility),
                seImageNumber: 46
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PerceptiveCopilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is FocusAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterTrigger;
            }
        }

        private void RegisterTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterTrigger;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostName + "'s ability",
                TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = DoReconSpecialistAbility
            });
        }

        private void DoReconSpecialistAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }
}