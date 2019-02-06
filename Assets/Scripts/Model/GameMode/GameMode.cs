using UnityEngine;
using SubPhases;
using Players;
using System;
using GameCommands;
using Actions;
using System.Collections.Generic;

namespace GameModes
{ 
    public abstract class GameMode
    {
        public abstract string Name { get; }

        public static GameMode CurrentGameMode;

        public abstract void ExecuteCommand(GameCommand command);

        public abstract void ExecuteServerCommand(GameCommand command);

        public abstract void RevertSubPhase();

        public abstract void AssignManeuver(string maneuverCode);

        public abstract void GiveInitiativeToRandomPlayer();

        public abstract void StartBattle();

        public abstract void TryConfirmBarrelRollPosition(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition);

        public abstract void StartBarrelRollExecution();
        public abstract void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems);
        public abstract void FinishBarrelRoll();

        public abstract void TryConfirmDecloakPosition(Vector3 shipBasePosition, string helperName, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles);
        public abstract void StartDecloakExecution(Ship.GenericShip ship);
        public abstract void CancelDecloak();
        public abstract void FinishDecloak();

        public abstract void TryConfirmBoostPosition(string selectedBoostHelper);
        public abstract void StartBoostExecution();
        public abstract void CancelBoost(List<ActionFailReason> boostProblems);
        public abstract void FinishBoost();

        public abstract void SetSwarmManagerManeuver(string maneuverCode);

        public abstract void ReturnToMainMenu();
        public abstract void QuitToDesktop();

        public abstract void GenerateDamageDeck(PlayerNo playerNo, int seed);
    }
}
