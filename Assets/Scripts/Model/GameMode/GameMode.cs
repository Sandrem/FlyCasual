using UnityEngine;
using SubPhases;
using Players;
using System;
using GameCommands;
using Actions;
using System.Collections.Generic;
using Ship;

namespace GameModes
{ 
    public abstract class GameMode
    {
        public abstract string Name { get; }

        public static GameMode CurrentGameMode;

        public abstract void ExecuteCommand(GameCommand command);

        public abstract void ExecuteServerCommand(GameCommand command);

        public abstract void GiveInitiativeToPlayer(int playerNo);

        public abstract void StartBattle();

        public abstract void ReturnToMainMenu();
        public abstract void QuitToDesktop();

        public abstract void GenerateDamageDeck(PlayerNo playerNo, int seed);

        internal void ExecuteServerCommand(object generateCancelShipSelectionCommand)
        {
            throw new NotImplementedException();
        }
    }
}
