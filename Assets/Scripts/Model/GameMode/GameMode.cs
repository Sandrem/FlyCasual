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

        public abstract void RevertSubPhase();

        public abstract void GiveInitiativeToRandomPlayer();

        public abstract void StartBattle();

        public abstract void TryConfirmBarrelRollPosition(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition);

        public abstract void StartBarrelRollExecution();
        public abstract void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems);
        public abstract void FinishBarrelRoll();

        public abstract void StartDecloakExecution(Ship.GenericShip ship);
        public abstract void CancelDecloak(List<ActionFailReason> decloakProblems);
        public abstract void FinishDecloak();

        public abstract void SetSwarmManagerManeuver(string maneuverCode);

        public abstract void ReturnToMainMenu();
        public abstract void QuitToDesktop();

        public abstract void GenerateDamageDeck(PlayerNo playerNo, int seed);
    }
}
