using Ship;
using Ship.UWing;
using Upgrade;
using Abilities;
using System;
using SubPhases;
using UnityEngine;
using RuleSets;

namespace UpgradesList
{
    public class PivotWingAttack : GenericDualUpgrade, ISecondEditionUpgrade
    {
        public PivotWingAttack() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Pivot Wing (Attack)";
            Cost = 0;

            UpgradeAbilities.Add(new PivotWingAttackAbility());

            AnotherSide = typeof(PivotWingLanding);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Name = "Pivot Wing (Open)";
            Cost = 0;

            Types.RemoveAll(t => t == UpgradeType.Title);
            Types.Add(UpgradeType.Configuration);

            UpgradeAbilities.RemoveAll(n => n is PivotWingAttackAbility);
            UpgradeAbilities.Add(new PivotWingAttackAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is UWing;
        }
    }

    public class PivotWingLanding : GenericDualUpgrade, ISecondEditionUpgrade
    {
        public PivotWingLanding() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Pivot Wing (Landing)";
            Cost = 0;

            UpgradeAbilities.Add(new PivotWingLandingAbility());

            AnotherSide = typeof(PivotWingAttack);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Name = "Pivot Wing (Closed)";
            Cost = 0;

            Types.RemoveAll(t => t == UpgradeType.Title);
            Types.Add(UpgradeType.Configuration);

            UpgradeAbilities.RemoveAll(n => n is PivotWingLandingAbility);
            UpgradeAbilities.Add(new PivotWingLandingAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is UWing;
        }
    }
}

namespace Abilities
{
    public class PivotWingAttackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            HostShip.OnMovementFinish += RegisterAskToUseFlip;
            HostShip.ChangeAgilityBy(+1);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterAskToUseFlip;
            HostShip.ChangeAgilityBy(-1);
        }

        protected virtual void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, DoFlipSide, infoText: "Flip to Landing position?");
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            HostShip.WingsClose();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        protected void ChangeInitialWingsPosition()
        {
            HostShip.WingsOpen();
        }
    }

    public class PivotWingAttackAbilitySE : PivotWingAttackAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            HostShip.OnMovementActivation += RegisterAskToUseFlip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivation -= RegisterAskToUseFlip;
        }

        protected override void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskToFlip);
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
            AskToUseAbility(AlwaysUseByDefault, DoFlipSide, infoText: "Flip to Attack position?");
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
            AskToUseAbility(NeverUseByDefault, RotateShip, infoText: "Rotate this ship?");
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

    public class PivotWingLandingAbilitySE: PivotWingLandingAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            HostShip.OnMovementActivation += RegisterAskToFlip;
            HostShip.OnManeuverIsRevealed += RegisterAskToRotate;
            HostShip.ChangeAgilityBy(-1);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivation -= RegisterAskToFlip;
            HostShip.OnManeuverIsRevealed -= RegisterAskToRotate;
            HostShip.ChangeAgilityBy(+1);
        }

        protected override void AskToRotate(object sender, EventArgs e)
        {
            PivotWindDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<PivotWindDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.InfoText = "Rotate the ship?";

            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("180", Rotate180);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            subphase.Start();
        }

        private void Rotate180(object sender, EventArgs e)
        {
            HostShip.Rotate180(DecisionSubPhase.ConfirmDecision);
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            HostShip.Rotate90Clockwise(DecisionSubPhase.ConfirmDecision);
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            HostShip.Rotate90Counterclockwise(DecisionSubPhase.ConfirmDecision);
        }

        private class PivotWindDecisionSubphase : DecisionSubPhase { };
    }
}
