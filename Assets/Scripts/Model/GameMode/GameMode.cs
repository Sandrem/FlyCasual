using UnityEngine;
using SubPhases;
using Players;

namespace GameModes
{ 
    public abstract class GameMode
    {
        public static GameMode CurrentGameMode;

        public abstract void RevertSubPhase();

        public abstract void ConfirmCrit();

        public abstract void DeclareTarget(int thisShip, int AnotherShip);

        public abstract void NextButtonEffect();
        public abstract void SkipButtonEffect();

        public abstract void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles);

        public abstract void AssignManeuver(string maneuverCode);

        public abstract void PerformStoredManeuver(int shipId);

        public abstract void GiveInitiativeToRandomPlayer();

        public abstract void ShowInformCritPanel();

        public abstract void StartBattle();

        public abstract void TryConfirmBarrelRollPosition(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition);

        public abstract void StartBarrelRollExecution();
        public abstract void CancelBarrelRoll();
        public abstract void FinishBarrelRoll();

        public abstract void TryConfirmDecloakPosition(Vector3 shipBasePosition, string helperName, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles);
        public abstract void StartDecloakExecution(Ship.GenericShip ship);
        public abstract void CancelDecloak();
        public abstract void FinishDecloak();

        public abstract void TryConfirmBoostPosition(string selectedBoostHelper);
        public abstract void StartBoostExecution();
        public abstract void CancelBoost();
        public abstract void FinishBoost();

        public abstract void UseDiceModification(string effectName);

        public abstract void ConfirmDiceResults();
        public abstract void CompareResultsAndDealDamage();
        public abstract void SwitchToOwnDiceModifications();

        public abstract void TakeDecision(Decision decision, GameObject button);

        public abstract void FinishMovementExecution();

        public abstract void SetSwarmManagerManeuver(string maneuverCode);

        public abstract void ReturnToMainMenu();
        public abstract void QuitToDesktop();

        public abstract void GenerateDamageDeck(PlayerNo playerNo, int seed);

        public abstract void CombatActivation(int shipId);

        public abstract void StartSyncNotificationSubPhase();
        public abstract void FinishNotificationSubPhase();

        public abstract void StartSyncDecisionPreparation();
        public abstract void FinishSyncDecisionPreparation();

        public abstract void StartSyncSelectShipPreparation();
        public abstract void FinishSyncSelectShipPreparation();

        public abstract void StartDiceRerollExecution();
    }
}
