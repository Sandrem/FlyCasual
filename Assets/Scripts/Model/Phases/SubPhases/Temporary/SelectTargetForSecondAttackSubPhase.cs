using GameCommands;
using GameModes;
using Ship;
using System;
using System.Linq;
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
                ShowSkipButton,
                AbilityName,
                Description,
                ImageSource
            );
        }

        public override void Initialize()
        {
            // If not skipped
            if (Phases.CurrentSubPhase == this) Selection.ThisShip.Owner.StartExtraAttack();
        }

        private bool FilterAttackTargets(GenericShip ship)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Selection.ThisShip, ship);
            return ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && distanceInfo.Range >= minRange && distanceInfo.Range <= maxRange;
        }

        private void FinishActon()
        {
            Selection.AnotherShip = TargetShip;
            // TODO CHECK
            Combat.ChosenWeapon = Selection.ThisShip.PrimaryWeapons.First();
            Combat.ShotInfo = new BoardTools.ShotInfo(Selection.ThisShip, TargetShip, Combat.ChosenWeapon);
            MovementTemplates.ShowFiringArcRange(Combat.ShotInfo);
            ExtraAttackTargetSelected();
        }

        private void ExtraAttackTargetSelected()
        {
            var subphase = Phases.StartTemporarySubPhaseNew(
                "Extra Attack",
                typeof(ExtraAttackSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(ExtraAttackSubPhase));
                    Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
                    CallBack();
                }
            );
            subphase.Start();

            GameCommand command = Combat.GenerateIntentToAttackCommand(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
            if (command != null) GameMode.CurrentGameMode.ExecuteCommand(command);
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

            IsReadyForCommands = true;
            GameController.CheckExistingCommands();
        }

    }
}