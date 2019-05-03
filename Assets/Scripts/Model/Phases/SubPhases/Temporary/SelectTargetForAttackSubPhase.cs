using GameCommands;
using GameModes;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubPhases
{
    public class SelectTargetForAttackSubPhase : SelectShipSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectShip, GameCommandTypes.DeclareAttack, GameCommandTypes.PressSkip }; } }

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

            // Not needed anymore - target is selected as "Select a ship" AI
            // if (Phases.CurrentSubPhase == this) Selection.ThisShip.Owner.StartExtraAttack();
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
            GameCommand command = Combat.GenerateIntentToAttackCommand(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
            if (command != null) GameMode.CurrentGameMode.ExecuteCommand(command);
        }

        public override void RevertSubPhase() { }

        public override void SkipButton()
        {
            UI.HideSkipButton();
            Selection.ThisShip.IsAttackPerformed = true;
            CallBack();
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            HideSubphaseDescription();

            Combat.ExtraAttackFilter = null;
            Combat.PayExtraAttackCost = null;

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