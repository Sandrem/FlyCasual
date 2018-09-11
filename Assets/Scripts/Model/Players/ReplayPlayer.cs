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

        public override void ConfirmDiceCheck()
        {
            GameController.CheckExistingCommands();
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
            GameController.CheckExistingCommands();
        }
    }

}
