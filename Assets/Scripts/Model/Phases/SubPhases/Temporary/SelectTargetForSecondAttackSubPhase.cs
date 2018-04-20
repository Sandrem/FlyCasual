using Ship;
using System;
using UnityEngine;

namespace SubPhases
{
    public class SelectTargetForSecondAttackSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            Selection.ThisShip.IsAttackPerformed = false;
            Combat.IsAttackAlreadyCalled = false;

            PrepareByParameters(
                FinishActon,
                FilterAttackTargets,
                null,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                AbilityName,
                Description,
                ImageUrl
            );

            Selection.ThisShip.Owner.StartExtraAttack();
        }

        private bool FilterAttackTargets(GenericShip ship)
        {
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship);
            return ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && distanceInfo.Range >= minRange && distanceInfo.Range <= maxRange;
        }

        private void FinishActon()
        {
            Selection.AnotherShip = TargetShip;
            Combat.ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
            Combat.ShotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, TargetShip, Combat.ChosenWeapon);
            MovementTemplates.ShowFiringArcRange(Combat.ShotInfo);
            ExtraAttackTargetSelected();
        }

        private void ExtraAttackTargetSelected()
        {
            Phases.StartTemporarySubPhaseNew(
                "Extra Attack",
                typeof(ExtraAttackSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(ExtraAttackSubPhase));
                    Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
                    CallBack();
                }
            );
            Combat.DeclareIntentToAttack(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
        }

        public override void RevertSubPhase() { }

        public override void SkipButton()
        {
            UI.HideSkipButton();
            Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
            CallBack();
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            HideSubphaseDescription();

            Combat.ExtraAttackFilter = null;

            Phases.CurrentSubPhase = PreviousSubPhase;
        }

        public override void Resume()
        {
            base.Resume();

            UpdateHelpInfo();
            UI.ShowSkipButton();
        }

    }
}