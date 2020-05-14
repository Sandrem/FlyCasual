using UnityEngine;
using System;
using System.Collections.Generic;
using SubPhases;
using Players;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameCommands;
using Actions;
using Ship;

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

        public override void GiveInitiativeToPlayer(int playerNo)
        {
            if (ReplaysManager.Mode == ReplaysMode.Write)
            {
                GameController.SendCommand
                (
                    RulesList.InitiativeRule.GenerateInitiativeDecisionOwnerCommand(playerNo)
                );
            }
            else if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                GameController.CheckExistingCommands();
            }
        }

        public override void StartBattle()
        {
            Global.BattleIsReady();
        }

        public override void GenerateDamageDeck(PlayerNo playerNo, int seed)
        {
            SyncDamageDeckSeed(playerNo, seed);
        }

        private void SyncDamageDeckSeed(PlayerNo playerNo, int seed)
        {
            // TODO: Move to player types

            if (ReplaysManager.Mode == ReplaysMode.Write)
            {
                GameController.SendCommand
                (
                    DamageDecks.GenerateDeckShuffleCommand(playerNo, seed)
                );
            }
            else if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                GameController.CheckExistingCommands();
            }
        }

        public override void ReturnToMainMenu()
        {
            Global.ReturnToMainMenu();
        }

        public override void QuitToDesktop()
        {
            Application.Quit();
        }
    }
}
