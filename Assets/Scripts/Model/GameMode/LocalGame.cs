﻿using UnityEngine;
using System;
using System.Collections.Generic;
using SubPhases;
using Players;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameCommands;
using Actions;

namespace GameModes
{
    public class LocalGame : GameMode
    {
        public override string Name { get { return "Local"; } }

        public override void ExecuteCommand(GameCommand command)
        {
            GameController.SendCommand(command);
        }

        public override void ExecuteServerCommand(GameCommand command)
        {
            GameController.SendCommand(command);
        }

        public override void RevertSubPhase()
        {
            (Phases.CurrentSubPhase as SelectShipSubPhase).CallRevertSubPhase();
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

        public override void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems)
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).CancelBarrelRoll(barrelRollProblems);
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

        public override void CancelDecloak(List<ActionFailReason> decloakProblems)
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).CancelDecloak(decloakProblems);
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

        public override void CancelBoost(List<ActionFailReason> boostProblems)
        {
            (Phases.CurrentSubPhase as BoostPlanningSubPhase).CancelBoost(boostProblems);
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

        public override void ReturnToMainMenu()
        {
            Phases.EndGame();
            SceneManager.LoadScene("MainMenu");
        }

        public override void QuitToDesktop()
        {
            Application.Quit();
        }
    }
}
