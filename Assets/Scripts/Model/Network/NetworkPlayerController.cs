using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameModes;
using SquadBuilderNS;
using Players;
using SubPhases;
using UnityEngine.SceneManagement;

public partial class NetworkPlayerController : NetworkBehaviour {

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (isLocalPlayer)
        {
            Network.CurrentPlayer = this;

            if (Network.ReadyToStartMatch)
            {
                Network.ReadyToStartMatch = false;
                SquadBuilder.StartNetworkGame();
            }
        }
        
    }

    public bool IsServer
    {
        get { return isServer; }
    }

    // TESTS

    [Command]
    public void CmdTest()
    {
        RpcTest();
    }

    [ClientRpc]
    private void RpcTest()
    {
        Messages.ShowInfo("Network test\nLocal: " + isLocalPlayer + "; Client: " + isClient + "; Server: " + isServer);
    }

    [Command]
    public void CmdCallBacksTest()
    {
        new NetworkExecuteWithCallback("Test", CmdRosterTest, CmdShowVariable);
    }

    [Command]
    public void CmdRosterTest()
    {
        Network.AllShipNames = "";
        RpcRosterTest();
    }

    [ClientRpc]
    private void RpcRosterTest()
    {
        /*string text = (isServer) ? "Hello from server" : "Hello from client";
        text += "\nMy first ship is " + RosterBuilder.TestGetNameOfFirstShipInRoster();
        Network.UpdateAllShipNames(RosterBuilder.TestGetNameOfFirstShipInRoster() + "\n");
        Network.ShowMessage(text);

        Network.FinishTask();*/
    }

    [Command]
    public void CmdUpdateAllShipNames(string text)
    {
        Network.AllShipNames += text;
    }

    [Command]
    public void CmdShowVariable()
    {
        RpcShowVariable(Network.AllShipNames);
    }

    [ClientRpc]
    private void RpcShowVariable(string text)
    {
        Messages.ShowInfo(text);
    }

    // START OF BATTLE

    [Command]
    public void CmdStartNetworkGame()
    {
        new NetworkExecuteWithCallback(
            "Wait sync squad list, then start battle",
            delegate { new NetworkExecuteWithCallback("Wait gather squad lists, then sync squad lists", CmdGetSquadList, CmdSendSquadToOpponent); },
            delegate { new NetworkExecuteWithCallback("Wait battle scene loading finish, then start battle", CmdLoadBattleScene, CmdStartBattle); }
        );
    }

    [Command]
    public void CmdGetSquadList()
    {
        Network.SquadJsons = new JSONObject();
        RpcGetSquadList();
    }

    [ClientRpc]
    private void RpcGetSquadList()
    {
        GameMode.CurrentGameMode = new NetworkGame();

        Global.ToggelLoadingScreen(true);

        JSONObject localSquadList = SquadBuilder.GetSquadList(PlayerNo.Player1).SavedConfiguration;
        Network.StoreSquadList(localSquadList.ToString(), isServer);

        if (IsServer)
        {
            SquadBuilder.SwitchPlayers();
        }

        Network.FinishTask();
    }

    [Command]
    public void CmdStoreSquadList(string squadJson, bool isServer)
    {
        Network.ImportSquad(squadJson, isServer);
    }

    [Command]
    public void CmdSendSquadToOpponent()
    {
        RpcSendSquadToOpponent(Network.SquadJsons.ToString());
    }

    [ClientRpc]
    private void RpcSendSquadToOpponent(string squadsJsonString)
    {
        JSONObject squadsJson = new JSONObject(squadsJsonString);

        if (squadsJson.HasField("Server"))
        {
            JSONObject squadJson = squadsJson["Server"];
            SquadBuilder.GetSquadList(PlayerNo.Player1).SavedConfiguration = squadJson;
        }

        if (squadsJson.HasField("Client"))
        {
            JSONObject squadJson = squadsJson["Client"];
            SquadBuilder.GetSquadList(PlayerNo.Player2).SavedConfiguration = squadJson;
            Network.FinishTask();
        }
    }

    [Command]
    public void CmdLoadBattleScene()
    {
        RpcLoadBattleScene();
    }

    [ClientRpc]
    private void RpcLoadBattleScene()
    {
        //RosterBuilder.GeneratePlayersShipConfigurations();

        SquadBuilder.GetSquadList(PlayerNo.Player1).PlayerType = (isServer) ? typeof(HumanPlayer) : typeof(NetworkOpponentPlayer);
        SquadBuilder.GetSquadList(PlayerNo.Player2).PlayerType = (isServer) ? typeof(NetworkOpponentPlayer) : typeof(HumanPlayer);

        SquadBuilder.LoadBattleScene();
    }

    [Command]
    public void CmdStartBattle()
    {
        RpcStartBattle();
    }

    [ClientRpc]
    private void RpcStartBattle()
    {
        if (isServer) Sounds.PlaySoundGlobal("Notification");
        Global.StartBattle();
    }

    // DECISIONS

    [Command]
    public void CmdTakeDecision(string decisionName)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdTakeDecision");
        RpcTakeDecision(decisionName);
    }

    [ClientRpc]
    private void RpcTakeDecision(string decisionName)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcTakeDecision");
        if (Phases.CurrentSubPhase as DecisionSubPhase == null)
        {
            Console.Write("Syncronization error, subphase is " + Phases.CurrentSubPhase.GetType(), LogTypes.Errors, true, "red");
            Messages.ShowError("Syncronization error, subphase is " + Phases.CurrentSubPhase.GetType());
        }

        (Phases.CurrentSubPhase as DecisionSubPhase).ExecuteDecision(decisionName);
    }

    // SETUP

    [Command]
    public void CmdConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
    {
        RpcConfirmShipSetup(shipId, position, angles);
    }

    [ClientRpc]
    private void RpcConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
    {
        (Phases.CurrentSubPhase as SetupSubPhase).ConfirmShipSetup(shipId, position, angles);
    }

    // ASSIGN MANEUVER

    [Command]
    public void CmdAssignManeuver(int shipId, string maneuverCode)
    {
        RpcAssignManeuver(shipId, maneuverCode);
    }

    [ClientRpc]
    private void RpcAssignManeuver(int shipId, string maneuverCode)
    {
        ShipMovementScript.AssignManeuver(shipId, maneuverCode);
    }

    // NEXT BUTTON
    
    [Command]
    public void CmdNextButtonEffect()
    {
        RpcNextButtonEffect();
    }

    [ClientRpc]
    private void RpcNextButtonEffect()
    {
        UI.NextButtonEffect();
    }

    // SKIP BUTTON

    [Command]
    public void CmdSkipButtonEffect()
    {
        RpcSkipButtonEffect();
    }

    [ClientRpc]
    private void RpcSkipButtonEffect()
    {
        UI.SkipButtonEffect();
    }

    // SHIP MOVEMENT

    [Command]
    public void CmdPerformStoredManeuver(int shipId)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdPerformStoredManeuver");

        new NetworkExecuteWithCallback(
            "Wait maneuver execution",
            delegate { CmdLaunchStoredManeuver(shipId); },
            delegate { CmdFinishManeuver(shipId); }
        );
    }

    [Command]
    public void CmdLaunchStoredManeuver(int shipId)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdLaunchStoredManeuver");
        RpcLaunchStoredManeuver(shipId);
    }

    [ClientRpc]
    private void RpcLaunchStoredManeuver(int shipId)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcLaunchStoredManeuver");
        ShipMovementScript.PerformStoredManeuver(shipId);
    }

    [Command]
    public void CmdFinishManeuver(int shipId)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdFinishManeuver");
        RpcFinishManeuver(shipId);
    }

    [ClientRpc]
    private void RpcFinishManeuver(int shipId)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: RpcFinishManeuver");

        Selection.ActiveShip.CallExecuteMoving(Triggers.FinishTrigger);
    }

    // BARREL ROLL

    [Command]
    public void CmdPerformBarrelRoll()
    {
        new NetworkExecuteWithCallback(
            "Wait barrel roll execution",
            CmdLaunchBarrelRoll,
            CmdFinishBarrelRoll
        );
    }

    [Command]
    public void CmdLaunchBarrelRoll()
    {
        RpcLaunchBarrelRoll();
    }

    [ClientRpc]
    private void RpcLaunchBarrelRoll()
    {
        (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).StartBarrelRollExecution();
    }

    [Command]
    public void CmdFinishBarrelRoll()
    {
        RpcFinishBarrelRoll();
    }

    [ClientRpc]
    private void RpcFinishBarrelRoll()
    {
        (Phases.CurrentSubPhase as BarrelRollExecutionSubPhase).FinishBarrelRollAnimation();
    }

    [Command]
    public void CmdCancelBarrelRoll()
    {
        RpcCancelBarrelRoll();
    }

    [ClientRpc]
    private void RpcCancelBarrelRoll()
    {
        (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).CancelBarrelRoll();
    }

    // BOOST

    [Command]
    public void CmdPerformBoost()
    {
        new NetworkExecuteWithCallback(
            "Wait boost execution",
            CmdLaunchBoost,
            CmdFinishBoost
        );
    }

    [Command]
    public void CmdLaunchBoost()
    {
        RpcLaunchBoost();
    }

    [ClientRpc]
    private void RpcLaunchBoost()
    {
        (Phases.CurrentSubPhase as BoostPlanningSubPhase).StartBoostExecution();
    }

    [Command]
    public void CmdFinishBoost()
    {
        RpcFinishBoost();
    }

    [ClientRpc]
    private void RpcFinishBoost()
    {
        Phases.FinishSubPhase(typeof(BoostExecutionSubPhase));
    }

    [Command]
    public void CmdCancelBoost()
    {
        RpcCancelBoost();
    }

    [ClientRpc]
    private void RpcCancelBoost()
    {
        (Phases.CurrentSubPhase as BoostPlanningSubPhase).CancelBoost();
    }

    // DECLOAK

    [Command]
    public void CmdPerformDecloak()
    {
        new NetworkExecuteWithCallback(
            "Wait decloak execution",
            CmdLaunchDecloak,
            CmdFinishDecloak
        );
    }

    [Command]
    public void CmdLaunchDecloak()
    {
        RpcLaunchDecloak();
    }

    [ClientRpc]
    private void RpcLaunchDecloak()
    {
        (Phases.CurrentSubPhase as DecloakPlanningSubPhase).StartDecloakExecution(Selection.ThisShip);
    }

    [Command]
    public void CmdFinishDecloak()
    {
        RpcFinishDecloak();
    }

    [ClientRpc]
    private void RpcFinishDecloak()
    {
        (Phases.CurrentSubPhase as DecloakExecutionSubPhase).FinishDecloakAnimation();
    }

    [Command]
    public void CmdCancelDecloak()
    {
        RpcCancelDecloak();
    }

    [ClientRpc]
    private void RpcCancelDecloak()
    {
        (Phases.CurrentSubPhase as DecloakPlanningSubPhase).CancelDecloak();
    }

    // DECLARE ATTACK TARGET

    [Command]
    public void CmdDeclareTarget(int attackerId, int defenderId)
    {
        RpcDeclareTarget(attackerId, defenderId);
    }

    [ClientRpc]
    private void RpcDeclareTarget(int attackerId, int defenderId)
    {
        Combat.DeclareIntentToAttack(attackerId, defenderId);
    }

    // SELECT TARGET SHIP

    [Command]
    public void CmdSelectTargetShip(int targetId)
    {
        RpcSelectTargetShip(targetId);
    }

    [ClientRpc]
    private void RpcSelectTargetShip(int targetId)
    {
        SelectShipSubPhase currentSubPhase = (Phases.CurrentSubPhase as SelectShipSubPhase);
        currentSubPhase.TargetShip = Roster.GetShipById("ShipId:" + targetId);
        currentSubPhase.InvokeFinish();
    }

    [Command]
    public void CmdRevertSubPhase()
    {
        RpcRevertSubPhase();
    }

    [ClientRpc]
    private void RpcRevertSubPhase()
    {
        (Phases.CurrentSubPhase as SelectShipSubPhase).CallRevertSubPhase();
    }

    // CONFIRM DICE RESULTS MODIFICATIONS

    [Command]
    public void CmdConfirmDiceResults()
    {
        RpcConfirmDiceResults();
    }

    [ClientRpc]
    private void RpcConfirmDiceResults()
    {
        Combat.ConfirmDiceResultsClient();
    }

    [Command]
    public void CmdSwitchToOwnDiceModifications()
    {
        RpcSwitchToOwnDiceModifications();
    }

    [ClientRpc]
    private void RpcSwitchToOwnDiceModifications()
    {
        Combat.SwitchToOwnDiceModificationsClient();
    }

    [Command]
    public void CmdCompareResultsAndDealDamage()
    {
        RpcCompareResultsAndDealDamage();
    }

    [ClientRpc]
    private void RpcCompareResultsAndDealDamage()
    {
        Combat.CompareResultsAndDealDamageClient();
    }

    // CONFIRM DICE ROLL CHECK

    [Command]
    public void CmdConfirmDiceRollCheckResults()
    {
        new NetworkExecuteWithCallback("Wait all confirm dice results", CmdShowDiceRollCheckConfirmButton, CmdConfirmDiceRerollCheckResults);
    }

    [Command]
    private void CmdShowDiceRollCheckConfirmButton()
    {
        RpcShowDiceRollCheckConfirmButton();
    }

    [ClientRpc]
    private void RpcShowDiceRollCheckConfirmButton()
    {
        (Phases.CurrentSubPhase as DiceRollCheckSubPhase).ShowDiceRollCheckConfirmButton();
    }

    [Command]
    private void CmdConfirmDiceRerollCheckResults()
    {
        RpcConfirmDiceRerollCheckResults();
    }

    [ClientRpc]
    private void RpcConfirmDiceRerollCheckResults()
    {
        (Phases.CurrentSubPhase as DiceRollCheckSubPhase).Confirm();
    }

    // CONFIRM INFORM CRIT WINDOW

    [Command]
    public void CmdCallInformCritWindow()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdCallInformCritWindow");
        new NetworkExecuteWithCallback("Wait all confirm faceup crit card results", CmdShowInformCritWindow, CmdHideInformCritWindow);
    }

    [Command]
    private void CmdShowInformCritWindow()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdShowInformCritWindow");
        RpcShowInformCritWindow();
    }

    [ClientRpc]
    private void RpcShowInformCritWindow()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcShowInformCritWindow");
        InformCrit.ShowPanelVisible();
    }

    [Command]
    private void CmdHideInformCritWindow()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdHideInformCritWindow");
        RpcHideInformCritWindow();
    }

    [ClientRpc]
    private void RpcHideInformCritWindow()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcHideInformCritWindow");
        InformCrit.HidePanel();
        Triggers.FinishTrigger();
    }

    // DICE ROLL SYNC

    [Command]
    public void CmdSyncDiceResults()
    {
        new NetworkExecuteWithCallback(
            "Wait sync dice results than calculate attack results prediction",
            CmdSendDiceRollResultsToClients,
            CmdCalculateDiceRoll
        );
    }

    [Command]
    public void CmdSendDiceRollResultsToClients()
    {
        RpcSendDiceRollResultsToClients(DiceRoll.CurrentDiceRoll.ResultsArray);
    }

    [ClientRpc]
    private void RpcSendDiceRollResultsToClients(DieSide[] dieSideResults)
    {
        Network.CompareDiceSidesAgainstServer(dieSideResults);
    }

    [Command]
    public void CmdCalculateDiceRoll()
    {
        RpcCalculateDiceRoll();
    }

    [ClientRpc]
    private void RpcCalculateDiceRoll()
    {
        if (DiceRoll.CurrentDiceRoll.CheckType == DiceRollCheckType.Combat)
        {
            (Phases.CurrentSubPhase as DiceRollCombatSubPhase).CalculateDice();
        }
        else if (DiceRoll.CurrentDiceRoll.CheckType == DiceRollCheckType.Check)
        {
            (Phases.CurrentSubPhase as DiceRollCheckSubPhase).CalculateDice();
        }
    }

    // DICE REROLL SYNC

    // DICE ROLL SYNC

    [Command]
    public void CmdStartDiceRerollExecution()
    {
        RpcStartDiceRerollExecution();
    }

    [ClientRpc]
    private void RpcStartDiceRerollExecution()
    {
        DiceRerollManager.CurrentDiceRerollManager.ConfirmReroll();
    }

    [Command]
    public void CmdSyncDiceRerollResults()
    {
        new NetworkExecuteWithCallback(
            "Wait sync dice reroll results then calculate attack results prediction",
            CmdSendDiceRollResultsToClients,
            CmdCalculateDiceReroll
        );
    }

    [Command]
    public void CmdCalculateDiceReroll()
    {
        RpcCalculateDiceReroll();
    }

    [ClientRpc]
    private void RpcCalculateDiceReroll()
    {
        DiceRerollManager.CurrentDiceRerollManager.UnblockButtons();
    }

    // DICE ROLL IN SYNC

    [Command]
    public void CmdSyncDiceRollInResults()
    {
        new NetworkExecuteWithCallback(
            "Wait sync dice roll in results then calculate attack results prediction",
            CmdSendDiceRollResultsToClients,
            CmdCalculateDiceRollIn
        );
    }

    [Command]
    public void CmdCalculateDiceRollIn()
    {
        RpcCalculateDiceRollIn();
    }

    [ClientRpc]
    private void RpcCalculateDiceRollIn()
    {
        DiceRoll.CurrentDiceRoll.UnblockButtons();
    }

    // DICE MODIFICATIONS

    [Command]
    public void CmdUseDiceModification(string diceModificationName)
    {
        RpcUseDiceModification(diceModificationName);
    }

    [ClientRpc]
    private void RpcUseDiceModification(string diceModificationName)
    {
        Combat.UseDiceModification(diceModificationName);
    }

    // BARREL ROLL PLANNING

    [Command]
    public void CmdTryConfirmBarrelRoll(string templateName, Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        RpcTryConfirmBarrelRoll(templateName, shipPosition, movementTemplatePosition);
    }

    [ClientRpc]
    private void RpcTryConfirmBarrelRoll(string templateName, Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).TryConfirmBarrelRollNetwork(templateName, shipPosition, movementTemplatePosition);
    }

    // DECLOAK PLANNING

    [Command]
    public void CmdTryConfirmDecloak(Vector3 shipPosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
    {
        RpcTryConfirmDecloak(shipPosition, decloakHelper, movementTemplatePosition, movementTemplateAngles);
    }

    [ClientRpc]
    private void RpcTryConfirmDecloak(Vector3 shipPosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
    {
        (Phases.CurrentSubPhase as DecloakPlanningSubPhase).TryConfirmDecloakNetwork(shipPosition, decloakHelper, movementTemplatePosition, movementTemplateAngles);
    }

    // BOOST PLANNING

    [Command]
    public void CmdTryConfirmBoostPosition(string SelectedBoostHelper)
    {
        RpcTryConfirmBoostPosition(SelectedBoostHelper);
    }

    [ClientRpc]
    private void RpcTryConfirmBoostPosition(string SelectedBoostHelper)
    {
        (Phases.CurrentSubPhase as BoostPlanningSubPhase).TryConfirmBoostPositionNetwork(SelectedBoostHelper);
    }

    // SELECTED DICE SYNC

    [Command]
    public void CmdSyncSelectedDiceAndReroll()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdSyncSelectedDice");

        int[] selectedDiceIds = new int[DiceRoll.CurrentDiceRoll.SelectedCount];
        for (int i = 0; i < DiceRoll.CurrentDiceRoll.SelectedCount; i++)
        {
            selectedDiceIds[i] = int.Parse(DiceRoll.CurrentDiceRoll.Selected[i].Model.name.Replace("DiceN", ""));
        }

        RpcSyncSelectedDiceAndReroll(selectedDiceIds);
    }

    [ClientRpc]
    private void RpcSyncSelectedDiceAndReroll(int[] selectedDiceIds)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcSyncSelectedDice");

        foreach (var die in DiceRoll.CurrentDiceRoll.DiceList)
        {
            int diceId = int.Parse(die.Model.name.Replace("DiceN", ""));
            bool isFound = false;
            for (int i = 0; i < selectedDiceIds.Length; i++)
            {
                if (selectedDiceIds[i] == diceId)
                {
                    isFound = true;
                    break;
                }
            }
            die.ToggleSelected(isFound);
        }

        DiceRoll.CurrentDiceRoll.RandomizeAndRerollSelected();
    }

    // SYNC DECKS

    [Command]
    public void CmdSyncDecks(int playerNo, int seed)
    {
        RpcSyncDecks(playerNo, seed);
    }

    [ClientRpc]
    private void RpcSyncDecks(int playerNo, int seed)
    {
        DamageDecks.GetDamageDeck(Tools.IntToPlayer(playerNo)).ShuffleDeck(seed);
    }

    // MESSAGES

    [Command]
    public void CmdShowMessage(string text)
    {
        RpcShowMessage(text);
    }

    [ClientRpc]
    private void RpcShowMessage(string text)
    {
        Messages.ShowInfo(text);
    }

    // CALLBACKS

    [Command]
    public void CmdFinishTask()
    {
        Network.ServerFinishTask();
    }

    // Swarm Manager

    [Command]
    public void CmdSetSwarmManagerManeuver(string maneuverCode)
    {
        RpcSetSwarmManagerManeuver(maneuverCode);
    }

    [ClientRpc]
    private void RpcSetSwarmManagerManeuver(string maneuverCode)
    {
        SwarmManager.SetManeuver(maneuverCode);
    }

    // Combat Activation

    [Command]
    public void CmdCombatActivation(int shipId)
    {
        RpcCombatActivation(shipId);
    }

    [ClientRpc]
    private void RpcCombatActivation(int shipId)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        Selection.ThisShip.CallCombatActivation(delegate { (Phases.CurrentSubPhase as CombatSubPhase).ChangeSelectionMode(Team.Type.Enemy); });
    }

    // Notification SubPhase Sync

    [Command]
    public void CmdSyncNotifications()
    {
        new NetworkExecuteWithCallback(
            "Wait sync notifications",
            CmdStartSyncNotificationSubphase,
            CmdFinishNotificationSubphase
        );
    }

    [Command]
    public void CmdStartSyncNotificationSubphase()
    {
        RpcWaitForFinishNotificationSubphase();
    }

    [ClientRpc]
    private void RpcWaitForFinishNotificationSubphase()
    {
        (Phases.CurrentSubPhase as NotificationSubPhase).FinishAfterDelay();
    }

    [Command]
    public void CmdFinishNotificationSubphase()
    {
        RpcFinishNotificationSubphase();
    }

    [ClientRpc]
    private void RpcFinishNotificationSubphase()
    {
        (Phases.CurrentSubPhase as NotificationSubPhase).Next();
    }

    // Sync decision preparation

    [Command]
    public void CmdSyncDecisionPreparation()
    {
        new NetworkExecuteWithCallback(
            "Wait sync decision preparation",
            CmdStartSyncDecisionPreparation,
            CmdFinishDecisionPreparation
        );
    }

    [Command]
    public void CmdStartSyncDecisionPreparation()
    {
        RpcStartSyncDecisionPreparation();
    }

    [ClientRpc]
    private void RpcStartSyncDecisionPreparation()
    {
        DecisionSubPhase decisionSubPhase = (Phases.CurrentSubPhase as DecisionSubPhase);

        if (decisionSubPhase != null)
        {
            decisionSubPhase.PrepareDecision(decisionSubPhase.StartIsFinished);
        }
        else
        {
            Console.Write("Waiting to sync decision subphase...", LogTypes.Everything, true, "orange");

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(0.5f, RpcStartSyncDecisionPreparation);
        }
    }

    [Command]
    public void CmdFinishDecisionPreparation()
    {
        RpcFinishDecisionPreparation();
    }

    [ClientRpc]
    private void RpcFinishDecisionPreparation()
    {
        (Phases.CurrentSubPhase as DecisionSubPhase).DecisionOwner.TakeDecision();
    }

    // Sync select ship preparation

    [Command]
    public void CmdSyncSelectShipPreparation()
    {
        new NetworkExecuteWithCallback(
            "Wait sync select ship preparation",
            CmdStartSyncSelectShipPreparation,
            CmdFinishSelectShipPreparation
        );
    }

    [Command]
    public void CmdStartSyncSelectShipPreparation()
    {
        RpcStartSyncSelectShipPreparation();
    }

    [ClientRpc]
    private void RpcStartSyncSelectShipPreparation()
    {
        SelectShipSubPhase selectShipSubPhase = (Phases.CurrentSubPhase as SelectShipSubPhase);

        if (selectShipSubPhase != null)
        {
            GameMode.CurrentGameMode.FinishSyncSelectShipPreparation();
        }
        else
        {
            Console.Write("Waiting to sync select ship subphase...", LogTypes.Everything, true, "orange");

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(0.5f, RpcStartSyncSelectShipPreparation);
        }
    }

    [Command]
    public void CmdFinishSelectShipPreparation()
    {
        RpcFinishSelectShipPreparation();
    }

    [ClientRpc]
    private void RpcFinishSelectShipPreparation()
    {
        (Phases.CurrentSubPhase as SelectShipSubPhase).HighlightShipsToSelect();
    }

    // Return To Main Menu

    [Command]
    public void CmdReturnToMainMenu(bool isServerSurrendered)
    {
        RpcReturnToMainMenu(isServerSurrendered);
    }

    [ClientRpc]
    private void RpcReturnToMainMenu(bool isServerSurrendered)
    {
        Phases.EndGame();

        // For opponent
        if (IsServer != isServerSurrendered)
        {
            UI.ShowGameResults("Opponent Disconnected");
        }
        else // For surrendered
        {
            Global.ToggelLoadingScreen(true);
            Network.Disconnect(delegate {
                SceneManager.LoadScene("MainMenu");
                Global.ToggelLoadingScreen(false);
            });
        }
    }

    // Quit to desktop

    [Command]
    public void CmdQuitToDesktop(bool isServerSurrendered)
    {
        RpcQuitToDesktop(isServerSurrendered);
    }

    [ClientRpc]
    private void RpcQuitToDesktop(bool isServerSurrendered)
    {
        Phases.EndGame();

        // For opponent
        if (IsServer != isServerSurrendered)
        {
            UI.ShowGameResults("Opponent Disconnected");
        }
        else // For surrendered
        {
            Global.ToggelLoadingScreen(true);
            Network.Disconnect(Application.Quit);
        }
    }

}
