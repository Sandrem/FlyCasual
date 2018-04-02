using Ship;
using Ship.UWing;
using Upgrade;
using Abilities;
using System;
using SubPhases;
using UnityEngine;

namespace UpgradesList
{
    public class PivotWingAttack : GenericDualUpgrade
    {
        public PivotWingAttack() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Pivot Wing (Attack)";
            Cost = 0;

            UpgradeAbilities.Add(new PivotWingAttackAbility());

            AnotherSide = typeof(PivotWingLanding);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is UWing;
        }
    }

    public class PivotWingLanding : GenericDualUpgrade
    {
        public PivotWingLanding() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Pivot Wing (Landing)";
            Cost = 0;

            UpgradeAbilities.Add(new PivotWingLandingAbility());

            AnotherSide = typeof(PivotWingAttack);
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

        private void RegisterAskToUseFlip(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, DoFlipSide);
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            HostShip.WingsClose();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChangeInitialWingsPosition()
        {
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
            HostShip.OnMovementFinish -= RegisterAskToFlip;
            HostShip.OnManeuverIsRevealed -= RegisterAskToRotate;
        }

        private void RegisterAskToFlip(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, DoFlipSide);
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            HostShip.WingsOpen();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        private void RegisterAskToRotate(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Stationary)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToRotate);
            }
        }

        private void AskToRotate(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, RotateShip);
        }

        private void RotateShip(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 180°", typeof(KoiogranTurnSubPhase), DecisionSubPhase.ConfirmDecision);
        }

        private void ChangeInitialWingsPosition()
        {
            HostShip.WingsClose();
        }
    }
}
