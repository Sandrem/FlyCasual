using Upgrade;
using Ship;
using System;
using SubPhases;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class PivotWingAttack : GenericDualUpgrade
    {
        public PivotWingAttack() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pivot Wing (Attack)",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.UT60DUWing.UT60DUWing)),
                abilityType: typeof(Abilities.FirstEdition.PivotWingAttackAbility)
            );

            AnotherSide = typeof(PivotWingLanding);
        }
    }

    public class PivotWingLanding : GenericDualUpgrade
    {
        public PivotWingLanding() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pivot Wing (Landing)",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.UT60DUWing.UT60DUWing)),
                abilityType: typeof(Abilities.FirstEdition.PivotWingLandingAbility)
            );

            AnotherSide = typeof(PivotWingAttack);
        }
    }
}

namespace Abilities.FirstEdition
{
    public class PivotWingAttackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += ChangeInitialWingsPosition;
            HostShip.OnMovementFinish += RegisterAskToUseFlip;
            HostShip.ChangeAgilityBy(+1);
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= ChangeInitialWingsPosition;
            HostShip.OnMovementFinish -= RegisterAskToUseFlip;
            HostShip.ChangeAgilityBy(-1); // Works only if flipped during battle, for squad builder ActivateAbilityForSquadBuilder is used
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ShipInfo.Agility++;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ShipInfo.Agility--;
        }

        protected virtual void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoFlipSide,
                descriptionLong: "Do you want to flip to Landing position?",
                imageHolder: HostUpgrade
            );
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            HostShip.WingsClose();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        protected void ChangeInitialWingsPosition()
        {
            Phases.Events.OnGameStart -= ChangeInitialWingsPosition;

            HostShip.WingsOpen();
        }
    }

    public class PivotWingLandingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            HostShip.OnMovementFinish += RegisterAskToFlip;
            HostShip.OnManeuverIsRevealed += RegisterAskToRotate;
        }

        public override void DeactivateAbility()
        {
            HostShip.WingsOpen();
            HostShip.OnMovementFinish -= RegisterAskToFlip;
            HostShip.OnManeuverIsRevealed -= RegisterAskToRotate;
        }

        protected void RegisterAskToFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                DoFlipSide,
                descriptionLong: "Do you want to flip to Attack position?",
                imageHolder: HostUpgrade
            );
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            HostShip.WingsOpen();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        protected void RegisterAskToRotate(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Stationary)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToRotate);
            }
        }

        protected virtual void AskToRotate(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                RotateShip,
                descriptionLong: "Do you want to rotate this ship?",
                imageHolder: HostUpgrade
            );
        }

        private void RotateShip(object sender, EventArgs e)
        {
            HostShip.Rotate180(DecisionSubPhase.ConfirmDecision);
        }

        protected void ChangeInitialWingsPosition()
        {
            HostShip.WingsClose();
        }
    }
}

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

            NameCanonical = "pivotwing-sideb";

            AnotherSide = typeof(PivotWingClosed);
        }
    }

    public class PivotWingClosed : GenericDualUpgrade
    {
        public PivotWingClosed() : base()
        {
            IsHidden = true;

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

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToFlip);
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

            subphase.DescriptionShort = "Pivot Wing";
            subphase.DescriptionLong = "Rotate the ship?";
            subphase.ImageSource = HostUpgrade;

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
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }
    }
}