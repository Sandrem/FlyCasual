using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
using UnityEngine;

namespace Players
{

    public partial class HumanPlayer : GenericPlayer
    {

        public HumanPlayer() : base()
        {
            PlayerType = PlayerType.Human;
            Name = "Human";
        }

        public override void PerformAttack()
        {
            UI.ShowSkipButton();

            base.PerformAttack();
        }

        public override void TakeDecision()
        {
            SubPhases.DecisionSubPhase subphase = (Phases.CurrentSubPhase as SubPhases.DecisionSubPhase);
            subphase.ShowDecisionWindowUI();

            if (subphase.IsForced)
            {
                GameCommand command = SubPhases.DecisionSubPhase.GenerateDecisionCommand(subphase.GetDecisions().First().Name);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
        }

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowConfirmButton();
        }

        public override void ToggleCombatDiceResults(bool isActive)
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).ToggleConfirmButton(isActive);
        }

        public override bool IsNeedToShowManeuver(GenericShip ship)
        {
            return true;
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            base.ChangeManeuver(callback, filter);

            DirectionsMenu.Show(callback, filter);
        }

        public override void SelectManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(callback, filter);

            base.SelectManeuver(callback, filter);
        }

        public override void SelectShipForAbility()
        {
            (Phases.CurrentSubPhase as SelectShipSubPhase).HighlightShipsToSelect();

            base.SelectShipForAbility();
        }

        public override void SelectObstacleForAbility()
        {
            (Phases.CurrentSubPhase as SelectObstacleSubPhase).HighlightObstacleToSelect();

            base.SelectObstacleForAbility();
        }

        public override void SetupShipMidgame()
        {
            (Phases.CurrentSubPhase as SetupShipMidgameSubPhase).ShowDescription();

            base.SetupShipMidgame();
        }

        public override void RerollManagerIsPrepared()
        {
            base.RerollManagerIsPrepared();
            DiceRerollManager.CurrentDiceRerollManager.ShowConfirmButton();
        }

        public override void PerformTractorBeamReposition(GenericShip ship)
        {
            RulesList.TractorBeamRule.PerfromManualTractorBeamReposition(ship, this);
        }

        public override void PlaceObstacle()
        {
            base.PlaceObstacle();

            SubPhases.ObstaclesPlacementSubPhase subphase = Phases.CurrentSubPhase as SubPhases.ObstaclesPlacementSubPhase;
            if (subphase.IsRandomSetupSelected[this.PlayerNo])
            {
                if (subphase.IsRandomSetupSelected[Roster.AnotherPlayer(this.PlayerNo)])
                {
                    subphase.SkipButton();
                }
                else
                {
                    GameManagerScript.Wait(1, subphase.SkipButton);
                }
            }
            else
            {
                SubPhases.ObstaclesPlacementSubPhase.IsLocked = false;
                if (!Network.IsNetworkGame) UI.ShowSkipButton("Random");
            }
        }

        public override void PerformSystemsActivation()
        {
            UI.ShowSkipButton();

            base.PerformSystemsActivation();
        }

        public override void InformAboutCrit()
        {
            base.InformAboutCrit();

            InformCrit.ShowConfirmButton();
        }

        public override void SyncDiceResults()
        {
            base.SyncDiceResults();

            GameMode.CurrentGameMode.ExecuteServerCommand(DiceRoll.GenerateSyncDiceCommand());
        }
    }

}
