﻿using UnityEngine;
using System;
using System.Collections.Generic;
using SubPhases;
using Players;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameCommands;

namespace GameModes
{
    public class LocalGame : GameMode
    {
        public override string Name { get { return "Local"; } }

        public override void RevertSubPhase()
        {
            (Phases.CurrentSubPhase as SelectShipSubPhase).CallRevertSubPhase();
        }

        public override void ConfirmCrit()
        {
            InformCrit.HidePanel();
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).ConfirmCrit();
        }

        public override void DeclareTarget(int thisShipId, int anotherShipId)
        {
            Combat.SendIntentToAttackCommand(thisShipId, anotherShipId);
        }

        public override void NextButtonEffect()
        {
            UI.SendNextButtonCommand();
        }

        public override void SkipButtonEffect()
        {
            UI.SendSkipButtonCommand();
        }

        public override void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
        {
            SetupSubPhase.SendPlaceShipCommand(shipId, position, angles);
        }

        public override void ActivateShipForMovement(int shipId)
        {
            ShipMovementScript.SendActivateAndMoveCommand(shipId);
        }

        public override void LaunchMovement(Action callback)
        {
            ShipMovementScript.LaunchMovement(callback);
        }

        public override void ActivateSystemsOnShip(int shipId)
        {
            (Phases.CurrentSubPhase as SystemsSubPhase).ActivateSystemsOnShipClient(Roster.GetShipById("ShipId:" + shipId));
        }

        public override void AssignManeuver(string maneuverCode)
        {
            ShipMovementScript.SendAssignManeuverCommand(Selection.ThisShip.ShipId, maneuverCode);
        }

        public override void GiveInitiativeToRandomPlayer()
        {
            if (ReplaysManager.Mode == ReplaysMode.Write)
            {
                int randomPlayer = UnityEngine.Random.Range(1, 3);

                JSONObject parameters = new JSONObject();
                parameters.AddField("player", Tools.IntToPlayer(randomPlayer).ToString());

                GameController.SendCommand(
                    GameCommandTypes.SyncPlayerWithInitiative,
                    null,
                    parameters.ToString()
                );

                Console.Write("Command is executed: " + GameCommandTypes.SyncPlayerWithInitiative, LogTypes.GameCommands, true, "aqua");
                GameController.GetCommand().Execute();
            }
            else if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                GameCommand command = GameController.GetCommand();

                if (command.Type == GameCommandTypes.SyncPlayerWithInitiative)
                {
                    Console.Write("Command is executed: " + command.Type, LogTypes.GameCommands, true, "aqua");
                    command.Execute();
                }
            }
        }

        public override void ShowInformCritPanel()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;

            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).InformAboutCrit();
        }

        public override void StartBattle()
        {
            Global.BattleIsReady();
        }

        // BARREL ROLL

        public override void TryConfirmBarrelRollPosition(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition)
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).TryConfirmBarrelRollPosition();
        }

        public override void StartBarrelRollExecution()
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).StartBarrelRollExecution();
        }

        public override void FinishBarrelRoll()
        {
            (Phases.CurrentSubPhase as BarrelRollExecutionSubPhase).FinishBarrelRollAnimation();
        }

        public override void CancelBarrelRoll()
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).CancelBarrelRoll();
        }

        // DECLOAK

        public override void TryConfirmDecloakPosition(Vector3 shipBasePosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).TryConfirmDecloakPosition();
        }

        public override void StartDecloakExecution(Ship.GenericShip ship)
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).StartDecloakExecution(ship);
        }

        public override void FinishDecloak()
        {
            (Phases.CurrentSubPhase as DecloakExecutionSubPhase).FinishDecloakAnimation();
        }

        public override void CancelDecloak()
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).CancelDecloak();
        }

        // BOOST

        public override void TryConfirmBoostPosition(string selectedBoostHelper)
        {
            (Phases.CurrentSubPhase as BoostPlanningSubPhase).TryConfirmBoostPosition();
        }

        public override void StartBoostExecution()
        {
            (Phases.CurrentSubPhase as BoostPlanningSubPhase).StartBoostExecution();
        }

        public override void FinishBoost()
        {
            Phases.FinishSubPhase(typeof(BoostExecutionSubPhase));
        }

        public override void CancelBoost()
        {
            (Phases.CurrentSubPhase as BoostPlanningSubPhase).CancelBoost();
        }

        public override void UseDiceModification(string effectName)
        {
            Combat.SendUseDiceModificationCommand(effectName);
        }

        public override void ConfirmDiceResults()
        {
            Combat.SendUseDiceModificationCommand("OK");
        }

        public override void CompareResultsAndDealDamage()
        {
            Combat.CompareResultsAndDealDamageClient();
        }

        public override void SwitchToRegularDiceModifications()
        {
            Combat.SwitchToRegularDiceModificationsClient();
        }

        public override void SwitchToAfterRolledDiceModifications()
        {
            Combat.SwitchToAfterRolledDiceModificationsClient();
        }

        public override void TakeDecision(Decision decision, GameObject button)
        {
            DecisionSubPhase.SendDecisionCommand(decision.Name);
        }

        public override void FinishMovementExecution()
        {
            Triggers.FinishTrigger();
        }

        // Swarm Manager

        public override void SetSwarmManagerManeuver(string maneuverCode)
        {
            SwarmManager.SetManeuver(maneuverCode);
        }

        public override void GenerateDamageDeck(PlayerNo playerNo, int seed)
        {
            SyncDamageDeckSeed(playerNo, seed);
        }

        private void SyncDamageDeckSeed(PlayerNo playerNo, int seed)
        {
            if (ReplaysManager.Mode == ReplaysMode.Write)
            {
                JSONObject parameters = new JSONObject();
                parameters.AddField("player", playerNo.ToString());
                parameters.AddField("seed", seed.ToString());

                GameController.SendCommand(
                    GameCommandTypes.DamageDecksSync,
                    null,
                    parameters.ToString()
                );

                Console.Write("Command is executed: " + GameCommandTypes.DamageDecksSync, LogTypes.GameCommands, true, "aqua");
                GameController.GetCommand().Execute();
            }
            else if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                GameCommand command = GameController.GetCommand();

                if (command.Type == GameCommandTypes.DamageDecksSync)
                {
                    Console.Write("Command is executed: " + command.Type, LogTypes.GameCommands, true, "aqua");
                    command.Execute();
                }
            }
        }

        public override void CombatActivation(int shipId)
        {
            Selection.ChangeActiveShip("ShipId:" + shipId);
            Selection.ThisShip.CallCombatActivation(delegate { (Phases.CurrentSubPhase as CombatSubPhase).ChangeSelectionMode(Team.Type.Enemy); });
        }

        public override void StartSyncNotificationSubPhase()
        {
            (Phases.CurrentSubPhase as NotificationSubPhase).FinishAfterDelay();
        }

        public override void FinishNotificationSubPhase()
        {
            (Phases.CurrentSubPhase as NotificationSubPhase).Next();
        }

        public override void StartSyncDecisionPreparation()
        {
            (Phases.CurrentSubPhase as DecisionSubPhase).PrepareDecision((Phases.CurrentSubPhase as DecisionSubPhase).StartIsFinished);
        }

        public override void FinishSyncDecisionPreparation()
        {
            (Phases.CurrentSubPhase as DecisionSubPhase).DecisionOwner.TakeDecision();
        }

        public override void StartSyncSelectShipPreparation()
        {
            FinishSyncSelectShipPreparation();
        }

        public override void FinishSyncSelectShipPreparation()
        {
            (Phases.CurrentSubPhase as SelectShipSubPhase).HighlightShipsToSelect();
        }

        public override void StartSyncSelectObstaclePreparation()
        {
            FinishSyncSelectObstaclePreparation();
        }

        public override void FinishSyncSelectObstaclePreparation()
        {
            (Phases.CurrentSubPhase as SelectObstacleSubPhase).HighlightObstacleToSelect();
        }

        public override void StartDiceRerollExecution()
        {
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceRerollSelected();
        }

        public override void ReturnToMainMenu()
        {
            Phases.EndGame();
            SceneManager.LoadScene("MainMenu");
        }

        public override void QuitToDesktop()
        {
            Application.Quit();
        }

        public override void PlaceObstacle(string obstacleName, Vector3 position, Vector3 angles)
        {
            ObstaclesPlacementSubPhase.SendPlaceObstacleCommand(obstacleName, position, angles);
        }

        public override void SelectObstacle(string obstacleName)
        {
            (Phases.CurrentSubPhase as SelectObstacleSubPhase).ConfirmSelectionOfObstacleClient(obstacleName);
        }
    }
}
