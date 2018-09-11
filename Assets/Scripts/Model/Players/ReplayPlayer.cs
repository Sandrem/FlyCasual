using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class ReplayPlayer : GenericPlayer
    {
        public ReplayPlayer() : base()
        {
            Type = PlayerType.Replay;
            Name = "Replay";
        }

        public override void TakeDecision()
        {
            GameController.CheckExistingCommands();
        }

        public override void PlaceObstacle()
        {
            base.PlaceObstacle();
            GameController.CheckExistingCommands();
        }

        public override void SetupShip()
        {
            base.SetupShip();
            GameController.CheckExistingCommands();
        }

        public override void AssignManeuver()
        {
            base.AssignManeuver();
            GameController.CheckExistingCommands();
        }

        public override void PressNext()
        {
            GameController.CheckExistingCommands();
        }

        public override void PressSkip()
        {
            GameController.CheckExistingCommands();
        }

        public override void PerformManeuver()
        {
            base.PerformManeuver();
            GameController.CheckExistingCommands();
        }

        public override void PerformAttack()
        {
            GameController.CheckExistingCommands();
        }

        public override void UseDiceModifications(DiceModificationTimingType type)
        {
            base.UseDiceModifications(type);
            Combat.ShowDiceModificationButtons(type);

            GameController.CheckExistingCommands();
        }

        public override void SelectShipForAbility()
        {
            GameController.CheckExistingCommands();
        }

        public override void SyncDiceResults()
        {
            GameController.CheckExistingCommands();
        }

        public override void SyncDiceRerollSelected()
        {
            GameController.CheckExistingCommands();
        }

        public override void RerollManagerIsPrepared()
        {
            DiceRerollManager.CurrentDiceRerollManager.ConfirmRerollButtonIsPressed();
        }

        public override void InformAboutCrit()
        {
            base.InformAboutCrit();
            GameController.CheckExistingCommands();
        }

        public override void ConfirmDiceCheck()
        {
            GameController.CheckExistingCommands();
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
            GameController.CheckExistingCommands();
        }

        public override void PerformSystemsActivation()
        {
            GameController.CheckExistingCommands();
        }

        public override void SelectObstacleForAbility()
        {
            GameController.CheckExistingCommands();
        }
    }

}
