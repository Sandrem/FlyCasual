using Upgrade;
using Ship;
using System;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class PivotWingOpen : GenericDualUpgrade
    {
        public PivotWingOpen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pivot Wing (Open)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.UT60DUWing.UT60DUWing)),
                abilityType: typeof(Abilities.SecondEdition.PivotWingOpenAbility),
                seImageNumber: 107
            );

            AnotherSide = typeof(PivotWingClosed);
        }
    }

    public class PivotWingClosed : GenericDualUpgrade
    {
        public PivotWingClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pivot Wing (Closed)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.UT60DUWing.UT60DUWing)),
                abilityType: typeof(Abilities.SecondEdition.PivotWingClosedAbility),
                seImageNumber: 107
            );

            IsSecondSide = true;

            AnotherSide = typeof(PivotWingOpen);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PivotWingOpenAbility : Abilities.FirstEdition.PivotWingAttackAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += ChangeInitialWingsPosition;
            HostShip.OnMovementActivationStart += RegisterAskToUseFlip;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= ChangeInitialWingsPosition;
            HostShip.OnMovementActivationStart -= RegisterAskToUseFlip;
        }

        protected override void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskToFlip);
        }

        public override void ActivateAbilityForSquadBuilder() {}
        public override void DeactivateAbilityForSquadBuilder() {}
    }

    public class PivotWingClosedAbility : Abilities.FirstEdition.PivotWingLandingAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            HostShip.OnMovementActivationStart += RegisterAskToFlip;
            HostShip.OnMovementExecuted += RegisterAskToRotate;

            HostShip.AfterGotNumberOfDefenceDice += DecreaseDice;
            HostShip.Tokens.AssignCondition(new Conditions.PivotWingCondition(HostShip, HostUpgrade));
        }

        public override void DeactivateAbility()
        {
            HostShip.WingsOpen();
            HostShip.OnMovementActivationStart -= RegisterAskToFlip;
            HostShip.OnMovementExecuted -= RegisterAskToRotate;

            HostShip.AfterGotNumberOfDefenceDice -= DecreaseDice;
            HostShip.Tokens.RemoveCondition(typeof(Conditions.PivotWingCondition));
        }

        private void DecreaseDice(ref int count)
        {
            Messages.ShowInfo("Pivot Wing Ability: This ship has -1 defense die");
            count--;
        }

        protected override void AskToRotate(object sender, EventArgs e)
        {
            PivotWindDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<PivotWindDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.InfoText = "Rotate the ship?";

            subphase.AddDecision("180", Rotate180, isCentered: true);
            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }, isCentered: true);

            subphase.Start();
        }

        private void Rotate180(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate180(Triggers.FinishTrigger);
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate90Clockwise(Triggers.FinishTrigger);
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate90Counterclockwise(Triggers.FinishTrigger);
        }

        private class PivotWindDecisionSubphase : DecisionSubPhase { };
    }
}

namespace Conditions
{
    public class PivotWingCondition : GenericToken
    {
        public PivotWingCondition(GenericShip host, GenericUpgrade source) : base(host)
        {
            Name = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }
    }
}