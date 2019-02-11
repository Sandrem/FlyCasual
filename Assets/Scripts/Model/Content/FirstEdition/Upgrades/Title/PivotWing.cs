using Upgrade;
using Ship;
using System;
using SubPhases;
using System.Collections.Generic;

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
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.UWing.UWing)),
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
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.UWing.UWing)),
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
}