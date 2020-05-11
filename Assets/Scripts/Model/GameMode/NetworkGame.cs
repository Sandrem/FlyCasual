using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SubPhases;
using Players;
using UnityEngine.UI;
using GameCommands;
using Actions;
using Ship;

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

        public override void GiveInitiativeToPlayer(int playerNo)
        {
            Network.SendServerCommand
            (
                RulesList.InitiativeRule.GenerateInitiativeDecisionOwnerCommand(playerNo)
            );

            GameController.CheckExistingCommands();
        }

        private static void StorePlayerWithInitiative(int[] randomHolder)
        {
            Phases.PlayerWithInitiative = Tools.IntToPlayer(randomHolder[0]);
        }

        public override void StartBattle()
        {
            Network.FinishTask();
        }

        public override void GenerateDamageDeck(PlayerNo playerNo, int seed)
        {
            Network.SyncDecks(playerNo, seed);
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
