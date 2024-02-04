using ActionsList;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Contraband Cybernetics",
                UpgradeType.Illicit,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.ContrabandCyberneticsAbility),
                charges: 1,
                seImageNumber: 58
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ContrabandCyberneticsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = Name,
                    TriggerType = TriggerTypes.OnMovementActivationStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = AskUseContrabandCybernetics
                });
            }
        }

        private void AskUseContrabandCybernetics(object sender, System.EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    ActivateContrabandCyberneticsAbility,
                    descriptionLong: "Do you want to spend 1 Charge? (If you do, until the end of the round, you can perform actions and execute red maneuvers, even while stressed)",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void ActivateContrabandCyberneticsAbility(object sender, System.EventArgs e)
        {
            Phases.Events.OnEndPhaseStart_NoTriggers += DeactivateContrabandCyberneticsAbility;

            HostUpgrade.State.SpendCharge();
            RemoveRestrictions();
        }

        private void RemoveRestrictions()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " allows " + HostShip.PilotInfo.PilotName + " to perform actions and red maneuvers even while stressed");

            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += AllowRedActionsWhileStressed;
            HostShip.OnTryCanPerformRedManeuverWhileStressed += AllowRedManeuversWhileStressed;

            Triggers.FinishTrigger();
        }

        private void AllowRedManeuversWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void AllowRedActionsWhileStressed(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

        public void DeactivateContrabandCyberneticsAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= DeactivateContrabandCyberneticsAbility;

            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed -= AllowRedActionsWhileStressed;
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= AllowRedManeuversWhileStressed;
        }
    }
}