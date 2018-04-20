using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SubPhases;
using Players;
using UnityEngine.UI;

namespace GameModes
{ 
    public class NetworkGame : GameMode
    {
        public override void RevertSubPhase()
        {
            Network.RevertSubPhase();
        }

        public override void ConfirmCrit()
        {
            if (DebugManager.DebugNetwork) UI.AddTestLogEntry("NetworkGame.FinishTask");
            InformCrit.DisableConfirmButton();
            Network.FinishTask();
        }

        public override void DeclareTarget(int thisShipId, int anotherShipId)
        {
            Network.DeclareTarget(thisShipId, anotherShipId);
        }

        public override void NextButtonEffect()
        {
            Network.NextButtonEffect();
        }

        public override void SkipButtonEffect()
        {
            Network.SkipButtonEffect();
        }

        public override void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
        {
            Network.ConfirmShipSetup(shipId, position, angles);
        }

        public override void PerformStoredManeuver(int shipId)
        {
            if (DebugManager.DebugNetwork) UI.AddTestLogEntry("PERFORM STORED MANEUVER");
            Network.PerformStoredManeuver(Selection.ThisShip.ShipId);
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

        public override void ShowInformCritPanel()
        {
            Network.CallInformCritWindow();
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

        public override void CancelBarrelRoll()
        {
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

        public override void CancelDecloak()
        {
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

        public override void CancelBoost()
        {
            Network.CancelBoost();
        }

        public override void UseDiceModification(string effectName)
        {
            Network.UseDiceModification(effectName);
        }

        public override void ConfirmDiceResults()
        {
            Network.ConfirmDiceResults();
        }

        public override void CompareResultsAndDealDamage()
        {
            Network.CompareResultsAndDealDamage();
        }

        public override void SwitchToOwnDiceModifications()
        {
            Network.SwitchToOwnDiceModifications();
        }

        public override void TakeDecision(Decision decision, GameObject button)
        {
            Network.TakeDecision(decision.Name);
        }

        public override void FinishMovementExecution()
        {
            Network.FinishTask();
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

        public override void CombatActivation(int shipId)
        {
            Network.CombatActivation(shipId);
        }

        public override void StartSyncNotificationSubPhase()
        {
            Network.CmdSyncNotifications(); 
        }

        public override void FinishNotificationSubPhase()
        {
            Network.FinishTask();
        }

        public override void StartSyncDecisionPreparation()
        {
            Network.SyncDecisionPreparation();
        }

        public override void FinishSyncDecisionPreparation()
        {
            Network.FinishTask();
        }

        public override void StartSyncSelectShipPreparation()
        {
            Network.SyncSelectShipPreparation();
        }

        public override void FinishSyncSelectShipPreparation()
        {
            Network.FinishTask();
        }

        public override void StartDiceRerollExecution()
        {
            Network.StartDiceRerollExecution();
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
