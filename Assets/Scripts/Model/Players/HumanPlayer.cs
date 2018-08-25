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
            Type = PlayerType.Human;
            Name = "Human";
        }

        public override void PerformAttack()
        {
            base.PerformAttack();

            UI.ShowSkipButton();
        }

        public override void UseDiceModifications(DiceModificationTimingType type)
        {
            base.UseDiceModifications(type);

            Combat.ShowDiceModificationButtons(type);
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

        public override void OnTargetNotLegalForAttack()
        {
            // TODO: Better explanations
            if (!Rules.TargetIsLegalForShot.IsLegal())
            {
                //automatic error messages
            }
            else if (!Combat.ShotInfo.IsShotAvailable)
            {
                Messages.ShowErrorToHuman("Target is outside your firing arc");
            }
            else if (Combat.ShotInfo.Range > Combat.ChosenWeapon.MaxRange || Combat.ShotInfo.Range < Combat.ChosenWeapon.MinRange)
            {
                Messages.ShowErrorToHuman("Target is outside your firing range");
            }

            //TODO: except non-legal targets, bupmed for example, biggs?
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            UI.ShowSkipButton();
            UI.HighlightNextButton();

            if (Phases.CurrentSubPhase is SubPhases.ExtraAttackSubPhase)
            {
                (Phases.CurrentSubPhase as SubPhases.ExtraAttackSubPhase).RevertSubphase();
            }
        }

        private bool FilterShipsToAttack(GenericShip ship)
        {
            return ship.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer;
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(callback, filter);
        }

        public override void SelectManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(callback, filter);
        }

        public override void SelectShipForAbility()
        {
            (Phases.CurrentSubPhase as SelectShipSubPhase).HighlightShipsToSelect();
        }

        public override void SelectObstacleForAbility()
        {
            (Phases.CurrentSubPhase as SelectObstacleSubPhase).HighlightObstacleToSelect();
        }

        public override void RerollManagerIsPrepared()
        {
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
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(1, subphase.SkipButton);
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
            base.PerformSystemsActivation();
            UI.ShowSkipButton();
        }
    }

}
