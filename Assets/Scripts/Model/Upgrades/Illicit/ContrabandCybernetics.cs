using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;

namespace UpgradesList
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        private bool CanPerformActionsWhileStressedOriginal;
        private bool CanPerformRedManeuversWhileStressedOriginal;

        public ContrabandCybernetics() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Contraband Cybernetics";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnMovementActivation += RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = Name,
                TriggerType = TriggerTypes.OnMovementActivation,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = AskUseContrabandCybernetics
            });
        }

        private void AskUseContrabandCybernetics(object sender, System.EventArgs e)
        {
            ContrabandCyberneticsDecisionSubPhase newSubPhase = (ContrabandCyberneticsDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Contraband Cybernetics Dicision",
                typeof(ContrabandCyberneticsDecisionSubPhase),
                Triggers.FinishTrigger
            );
            newSubPhase.ContrabandCyberneticsUpgrade = this;
            newSubPhase.Start();
        }

        public void ActivateAbility()
        {
            Host.OnMovementActivation -= RegisterTrigger;
            Phases.OnEndPhaseStart_NoTriggers += DeactivateAbility;

            Host.Tokens.AssignToken(new Tokens.StressToken(Host), RemoveRestrictions);
        }

        private void RemoveRestrictions()
        {
            CanPerformActionsWhileStressedOriginal = Host.CanPerformActionsWhileStressed;
            Host.CanPerformActionsWhileStressed = true;

            CanPerformRedManeuversWhileStressedOriginal = Host.CanPerformRedManeuversWhileStressed;
            Host.CanPerformRedManeuversWhileStressed = true;
        }

        public void DeactivateAbility()
        {
            Phases.OnEndPhaseStart_NoTriggers -= DeactivateAbility;

            Host.CanPerformActionsWhileStressed = CanPerformActionsWhileStressedOriginal;
            Host.CanPerformRedManeuversWhileStressed = CanPerformRedManeuversWhileStressedOriginal;
        }
    }
}

namespace SubPhases
{

    public class ContrabandCyberneticsDecisionSubPhase : DecisionSubPhase
    {
        public UpgradesList.ContrabandCybernetics ContrabandCyberneticsUpgrade;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Use ability of Contraband Cybernetics?";
            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            AddDecision("Yes", UseContrabandCyberneticsAbility);
            AddDecision("No", DontUseContrabandCyberneticsAbility);

            DefaultDecisionName = "No";

            callBack();
        }

        private void UseContrabandCyberneticsAbility(object sender, System.EventArgs e)
        {
            ContrabandCyberneticsUpgrade.ActivateAbility();
            ContrabandCyberneticsUpgrade.TryDiscard(ConfirmDecision);
        }

        private void DontUseContrabandCyberneticsAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
