using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameModes;
using SquadBuilderNS;
using Players;

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
    public void RpcGetSquadList()
    {
        GameMode.CurrentGameMode = new NetworkGame();

        SquadBuilder.ShowOpponentSquad();

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
    public void RpcSendSquadToOpponent(string squadsJsonString)
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
    public void RpcLoadBattleScene()
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
    public void RpcStartBattle()
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
        if (Phases.CurrentSubPhase as SubPhases.DecisionSubPhase == null)
        {
            Console.Write("Syncronization error, subphase is " + Phases.CurrentSubPhase.GetType(), LogTypes.Errors, true, "red");
            Messages.ShowError("Syncronization error, subphase is " + Phases.CurrentSubPhase.GetType());
        }

        (Phases.CurrentSubPhase as SubPhases.DecisionSubPhase).ExecuteDecision(decisionName);
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
        (Phases.CurrentSubPhase as SubPhases.SetupSubPhase).ConfirmShipSetup(shipId, position, angles);
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

        Selection.ActiveShip.CallExecuteMoving(delegate { Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase)); });
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
        (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).StartBarrelRollExecution();
    }

    [Command]
    public void CmdFinishBarrelRoll()
    {
        RpcFinishBarrelRoll();
    }

    [ClientRpc]
    private void RpcFinishBarrelRoll()
    {
        (Phases.CurrentSubPhase as SubPhases.BarrelRollExecutionSubPhase).FinishBarrelRollAnimation();
    }

    [Command]
    public void CmdCancelBarrelRoll()
    {
        RpcCancelBarrelRoll();
    }

    [ClientRpc]
    private void RpcCancelBarrelRoll()
    {
        (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).CancelBarrelRoll();
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
        (Phases.CurrentSubPhase as SubPhases.BoostPlanningSubPhase).StartBoostExecution();
    }

    [Command]
    public void CmdFinishBoost()
    {
        RpcFinishBoost();
    }

    [ClientRpc]
    private void RpcFinishBoost()
    {
        Phases.FinishSubPhase(typeof(SubPhases.BoostExecutionSubPhase));
    }

    [Command]
    public void CmdCancelBoost()
    {
        RpcCancelBoost();
    }

    [ClientRpc]
    private void RpcCancelBoost()
    {
        (Phases.CurrentSubPhase as SubPhases.BoostPlanningSubPhase).CancelBoost();
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
        (Phases.CurrentSubPhase as SubPhases.DecloakPlanningSubPhase).StartDecloakExecution(Selection.ThisShip);
    }

    [Command]
    public void CmdFinishDecloak()
    {
        RpcFinishDecloak();
    }

    [ClientRpc]
    private void RpcFinishDecloak()
    {
        (Phases.CurrentSubPhase as SubPhases.DecloakExecutionSubPhase).FinishDecloakAnimation();
    }

    [Command]
    public void CmdCancelDecloak()
    {
        RpcCancelDecloak();
    }

    [ClientRpc]
    private void RpcCancelDecloak()
    {
        (Phases.CurrentSubPhase as SubPhases.DecloakPlanningSubPhase).CancelDecloak();
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
        SubPhases.SelectShipSubPhase currentSubPhase = (Phases.CurrentSubPhase as SubPhases.SelectShipSubPhase);
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
        (Phases.CurrentSubPhase as SubPhases.SelectShipSubPhase).CallRevertSubPhase();
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
        (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowDiceRollCheckConfirmButton();
    }

    [Command]
    private void CmdConfirmDiceRerollCheckResults()
    {
        RpcConfirmDiceRerollCheckResults();
    }

    [ClientRpc]
    private void RpcConfirmDiceRerollCheckResults()
    {
        (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).Confirm();
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
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).CalculateDice();
        }
        else if (DiceRoll.CurrentDiceRoll.CheckType == DiceRollCheckType.Check)
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).CalculateDice();
        }
    }

    // DICE REROLL SYNC

    // DICE ROLL SYNC

    [Command]
    public void CmdSyncDiceRerollResults()
    {
        new NetworkExecuteWithCallback(
            "Wait sync dice reroll results than calculate attack results prediction",
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
        (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).TryConfirmBarrelRollNetwork(templateName, shipPosition, movementTemplatePosition);
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
        (Phases.CurrentSubPhase as SubPhases.DecloakPlanningSubPhase).TryConfirmDecloakNetwork(shipPosition, decloakHelper, movementTemplatePosition, movementTemplateAngles);
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
        (Phases.CurrentSubPhase as SubPhases.BoostPlanningSubPhase).TryConfirmBoostPositionNetwork(SelectedBoostHelper);
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

    [Command]
    public void CmdSetSwarmManagerManeuver(string maneuverCode)
    {
        RpcSetSwarmManagerManeuver(maneuverCode);
    }

    [ClientRpc]
    public void RpcSetSwarmManagerManeuver(string maneuverCode)
    {
        SwarmManager.SetManeuver(maneuverCode);
    }
}
