using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameModes
{ 
    public abstract class GameMode
    {
        public static GameMode CurrentGameMode;

        public abstract void ConfirmCrit();

        public abstract void DeclareTarget(int thisShip, int AnotherShip);

        public abstract void NextButtonEffect();

        public abstract void SkipButtonEffect();

        public abstract void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles);

        public abstract void AssignManeuver(int shipId, string maneuverCode);

        public abstract void PerformStoredManeuver(int shipId);

        public abstract void GiveInitiativeToRandomPlayer();

        public abstract void ShowInformCritPanel();

        public abstract void StartBattle();

        public abstract void TryConfirmBarrelRollPosition(Vector3 shipBasePosition, Vector3 movementTemplatePosition);

        public abstract void StartBarrelRollExecution(Ship.GenericShip ship);

        public abstract void CancelBarrelRoll();

        public abstract void FinishBarrelRoll();

        public abstract void TryConfirmBoostPosition(string selectedBoostHelper);

        public abstract void StartBoostExecution(Ship.GenericShip ship);

        public abstract void CancelBoost();

        public abstract void FinishBoost();

        public abstract void UseDiceModification(string effectName);

        public abstract void ConfirmDiceResults();

        public abstract void GetCritCard(Action callBack);

        public abstract void TakeDecision(KeyValuePair<string, EventHandler> decision, GameObject button);

        public abstract void FinishMovementExecution();
    }
}
