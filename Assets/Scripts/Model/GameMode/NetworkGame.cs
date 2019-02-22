﻿using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SubPhases;
using Players;
using UnityEngine.UI;
using GameCommands;
using Actions;

namespace GameModes
{ 
    public class NetworkGame : GameMode
    {
        public override string Name { get { return "Network"; } }

        public override void ExecuteCommand(GameCommand command)
        {
            Network.SendCommand(command);
        }

        public override void ExecuteServerCommand(GameCommand command)
        {
            if (Network.IsServer) Network.SendCommand(command);
        }

        public override void RevertSubPhase()
        {
            Network.RevertSubPhase();
        }

        public override void AssignManeuver(string maneuverCode)
        {
            Network.AssignManeuver(Selection.ThisShip.ShipId, maneuverCode);
        }

        public override void GiveInitiativeToRandomPlayer()
        {
            if (DebugManager.DebugNetwork) UI.AddTestLogEntry("NetworkGame.GiveInitiativeToRandomPlayer");
            Network.GenerateRandom(
                new Vector2(1, 2),
                1,
                StorePlayerWithInitiative,
                Triggers.FinishTrigger
            );
        }

        private static void StorePlayerWithInitiative(int[] randomHolder)
        {
            Phases.PlayerWithInitiative = Tools.IntToPlayer(randomHolder[0]);
        }

        public override void StartBattle()
        {
            Network.FinishTask();
        }

        // BARREL ROLL

        public override void TryConfirmBarrelRollPosition(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition)
        {
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HumanPlayer))
            {
                Network.TryConfirmBarrelRoll(templateName, shipBasePosition, movementTemplatePosition);
            }
        }

        public override void StartBarrelRollExecution()
        {
            Network.PerformBarrelRoll();
        }

        public override void FinishBarrelRoll()
        {
            Network.FinishTask();
        }

        public override void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems)
        {
            //TODONETWORK
            Network.CancelBarrelRoll();
        }

        // DECLOAK

        public override void TryConfirmDecloakPosition(Vector3 shipBasePosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
        {
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HumanPlayer))
            {
                Network.TryConfirmDecloak(shipBasePosition, decloakHelper, movementTemplatePosition, movementTemplateAngles);
            }
        }

        public override void StartDecloakExecution(Ship.GenericShip ship)
        {
            Network.PerformDecloak();
        }

        public override void FinishDecloak()
        {
            Network.FinishTask();
        }

        public override void CancelDecloak(List<ActionFailReason> decloakProblems)
        {
            //TODONETWORK
            Network.CancelDecloak();
        }

        // BOOST

        public override void TryConfirmBoostPosition(string selectedBoostHelper)
        {
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HumanPlayer))
            {
                Network.TryConfirmBoostPosition(selectedBoostHelper);
            }
        }

        public override void StartBoostExecution()
        {
            Network.PerformBoost();
        }

        public override void FinishBoost()
        {
            Network.FinishTask();
        }

        public override void CancelBoost(List<ActionFailReason> boostProblems)
        {
            // TODONETWORK - pass
            Network.CancelBoost();
        }

        // Swarm Manager

        public override void SetSwarmManagerManeuver(string maneuverCode)
        {
            Network.SetSwarmManagerManeuver(maneuverCode);
        }

        public override void GenerateDamageDeck(PlayerNo playerNo, int seed)
        {
            Network.SyncDecks(Tools.PlayerToInt(playerNo), seed);
        }

        public override void ReturnToMainMenu()
        {
            Network.ReturnToMainMenu();
        }

        public override void QuitToDesktop()
        {
            Network.QuitToDesktop();
        }

    }
}
