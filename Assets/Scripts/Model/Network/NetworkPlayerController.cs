using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class NetworkPlayerController : NetworkBehaviour {

    private void Start()
    {
        if (isLocalPlayer) Network.CurrentPlayer = this;
        DontDestroyOnLoad(this.gameObject);
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
        new NetworkExecuteWithCallback(CmdRosterTest, CmdShowVariable);
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
        string text = (isServer) ? "Hello from server" : "Hello from client";
        text += "\nMy first ship is " + RosterBuilder.TestGetNameOfFirstShipInRoster();
        Network.UpdateAllShipNames(RosterBuilder.TestGetNameOfFirstShipInRoster() + "\n");
        Network.ShowMessage(text);

        Network.FinishTask();
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
            delegate { new NetworkExecuteWithCallback(CmdGetSquadList, CmdSendSquadToOpponent); },
            delegate { new NetworkExecuteWithCallback(CmdLoadBattleScene, CmdStartBattle); }
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
        JSONObject localSquadList = RosterBuilder.GetSquadInJson(Players.PlayerNo.Player1);
        Network.StoreSquadList(localSquadList.ToString(), isServer);
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
        string opponentSquadName = (isServer) ? "Client" : "Server";
        if (squadsJson.HasField(opponentSquadName))
        {
            JSONObject squadJson = squadsJson[opponentSquadName];
            if (isServer)
            {
                RosterBuilder.SetPlayerSquadFromImportedJson(squadJson, Players.PlayerNo.Player2, Network.FinishTask);
            }
            else
            {
                RosterBuilder.SwapRosters(delegate { RosterBuilder.SetPlayerSquadFromImportedJson(squadJson, Players.PlayerNo.Player1, Network.FinishTask); });
            }
        }
        else
        {
            Messages.ShowError("No ships");
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
        RosterBuilder.GeneratePlayersShipConfigurations();

        RosterBuilder.HideNetworkManagerHUD();
        RosterBuilder.ShowOpponentSquad();
        RosterBuilder.LoadBattleScene();
    }

    [Command]
    public void CmdStartBattle()
    {
        RpcStartBattle();
    }

    [ClientRpc]
    public void RpcStartBattle()
    {
        Global.StartBattle();
    }

    // DECISIONS

    [Command]
    public void CmdTakeDecision(string decisionName)
    {
        RpcTakeDecision(decisionName);
    }

    [ClientRpc]
    private void RpcTakeDecision(string decisionName)
    {
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
        // Temporary
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Movement.AssignManeuver(shipId, maneuverCode);
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
        new NetworkExecuteWithCallback(
            delegate { CmdLaunchStoredManeuver(shipId); },
            delegate { CmdFinishManeuver(shipId); }
        );
    }

    [Command]
    public void CmdLaunchStoredManeuver(int shipId)
    {
        RpcLaunchStoredManeuver(shipId);
    }

    [ClientRpc]
    private void RpcLaunchStoredManeuver(int shipId)
    {
        // Temporary
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Movement.PerformStoredManeuver(shipId);
    }

    [Command]
    public void CmdFinishManeuver(int shipId)
    {
        RpcFinishManeuver(shipId);
    }

    [ClientRpc]
    private void RpcFinishManeuver(int shipId)
    {
        Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase));
    }

    // BARREL ROLL

    [Command]
    public void CmdPerformBarrelRoll()
    {
        new NetworkExecuteWithCallback(
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
        (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).StartBarrelRollExecution(Selection.ThisShip);
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

    // BOOST

    [Command]
    public void CmdPerformBoost()
    {
        new NetworkExecuteWithCallback(
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
        (Phases.CurrentSubPhase as SubPhases.BoostPlanningSubPhase).StartBoostExecution(Selection.ThisShip);
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

    // DECLARE ATTACK TARGET

    [Command]
    public void CmdDeclareTarget(int attackerId, int defenderId)
    {
        RpcDeclareTarget(attackerId, defenderId);
    }

    [ClientRpc]
    private void RpcDeclareTarget(int attackerId, int defenderId)
    {
        Combat.DeclareTarget(attackerId, defenderId);
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

    // CONFIRM DICE ROLL CHECK

    [Command]
    public void CmdConfirmDiceRollCheckResults()
    {
        new NetworkExecuteWithCallback(CmdShowDiceRollCheckConfirmButton, CmdConfirmDiceRerollCheckResults);
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
        new NetworkExecuteWithCallback(CmdShowInformCritWindow, CmdHideInformCritWindow);
    }

    [Command]
    private void CmdShowInformCritWindow()
    {
        RpcShowInformCritWindow();
    }

    [ClientRpc]
    private void RpcShowInformCritWindow()
    {
        InformCrit.ShowPanelVisible();
    }

    [Command]
    private void CmdHideInformCritWindow()
    {
        RpcHideInformCritWindow();
    }

    [ClientRpc]
    private void RpcHideInformCritWindow()
    {
        Triggers.FinishTrigger();
    }

    // DICE ROLL SYNC

    [Command]
    public void CmdSyncDiceResults()
    {
        new NetworkExecuteWithCallback(
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
    public void CmdTryConfirmBarrelRoll(Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        RpcTryConfirmBarrelRoll(shipPosition, movementTemplatePosition);
    }

    [ClientRpc]
    private void RpcTryConfirmBarrelRoll(Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).TryConfirmBarrelRollNetwork(shipPosition, movementTemplatePosition);
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
}
