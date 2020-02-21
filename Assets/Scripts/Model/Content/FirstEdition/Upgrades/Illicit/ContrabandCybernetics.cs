using ActionsList;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Contraband Cybernetics",
                UpgradeType.Illicit,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.ContrabandCyberneticsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (IsAbilityCanBeUsed())
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

        protected virtual bool IsAbilityCanBeUsed()
        {
            return true;
        }

        private void AskUseContrabandCybernetics(object sender, System.EventArgs e)
        {
            if (IsAbilityCanBeUsed())
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

            PayActivationCost(RemoveRestrictions);
        }

        protected virtual void PayActivationCost(Action callback)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), callback);
        }

        private void RemoveRestrictions()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " allows " + HostShip.PilotInfo.PilotName + " to perform actions and red maneuvers even while stressed");

            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += AllowRedActionsWhileStressed;
            HostShip.OnTryCanPerformRedManeuverWhileStressed += AllowRedManeuversWhileStressed;

            FinishAbility();
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

        protected virtual void FinishAbility()
        {
            HostUpgrade.TryDiscard(Triggers.FinishTrigger);
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