using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static partial class Network
{
    public static NetworkPlayerController CurrentPlayer;

    public static NetworkExecuteWithCallback LastNetworkCallback;

    public static string AllShipNames;

    public static JSONObject SquadJsons;

    public static bool IsNetworkGame
    {
        get { return CurrentPlayer != null; }
    }

    public static bool IsServer
    {
        get { return CurrentPlayer.IsServer; }
    }

    // SQUAD LISTS

    public static void ImportSquad(string squadList, bool isServer)
    {
        string squadName = (isServer) ? "Server" : "Client";
        JSONObject squadListJson = new JSONObject(squadList);
        SquadJsons.AddField(squadName, squadListJson);
    }

    public static void StoreSquadList(string localSquadList, bool isServer)
    {
        CurrentPlayer.CmdStoreSquadList(localSquadList.ToString(), isServer);
    }

    // TESTS

    public static void Test()
    {
        CurrentPlayer.CmdTest();
    }

    public static void UpdateAllShipNames(string text)
    {
        CurrentPlayer.CmdUpdateAllShipNames(text);
    }

    public static void CallBacksTest()
    {
        CurrentPlayer.CmdCallBacksTest();
    }

    // CALLBACKS

    public static void FinishTask()
    {
        string logEntryPostfix = (IsServer) ? "" : "\n";
        Console.Write("Client finished task" + logEntryPostfix, LogTypes.Network);
        CurrentPlayer.CmdFinishTask();
    }

    public static void ServerFinishTask()
    {
        LastNetworkCallback.ServerFinishTask();
    }

    // SELECT SHIP

    public static void RevertSubPhase()
    {
        if (IsServer) CurrentPlayer.CmdRevertSubPhase();
    }

    // TOOLS

    public static void ShowMessage(string text)
    {
        CurrentPlayer.CmdShowMessage(text);
    }

    // BATTLE START

    public static void StartNetworkGame()
    {
        CurrentPlayer.CmdStartNetworkGame();
    }

    // DECISIONS

    public static void TakeDecision(string decisionName)
    {
        CurrentPlayer.CmdTakeDecision(decisionName);
    }

    // SETUP

    public static void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
    {
        CurrentPlayer.CmdConfirmShipSetup(shipId, position, angles);
    }

    // ASSING MANEUVER

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        CurrentPlayer.CmdAssignManeuver(shipId, maneuverCode);
    }

    // NEXT BUTTON

    public static void NextButtonEffect()
    {
        CurrentPlayer.CmdNextButtonEffect();
    }

    // SKIP BUTTON

    public static void SkipButtonEffect()
    {
        CurrentPlayer.CmdSkipButtonEffect();
    }

    // PERFORM MANEUVER

    public static void PerformStoredManeuver(int shipId)
    {
        CurrentPlayer.CmdPerformStoredManeuver(shipId);
    }

    // PERFORM BARREL ROLL

    public static void PerformBarrelRoll()
    {
        if (IsServer) CurrentPlayer.CmdPerformBarrelRoll();
    }

    public static void CancelBarrelRoll()
    {
        if (IsServer) CurrentPlayer.CmdCancelBarrelRoll();
    }

    // PERFORM BOOST

    public static void PerformBoost()
    {
        if (IsServer) CurrentPlayer.CmdPerformBoost();
    }

    public static void CancelBoost()
    {
        if (IsServer) CurrentPlayer.CmdCancelBoost();
    }

    // DECLARE COMBAT TARGET

    public static void DeclareTarget(int attackerId, int defenderId)
    {
        CurrentPlayer.CmdDeclareTarget(attackerId, defenderId);
    }

    // SELECT TARGET SHIP

    public static void SelectTargetShip(int targetId)
    {
        CurrentPlayer.CmdSelectTargetShip(targetId);
    }

    // CONFIRM DICE RESULTS MODIFICATION

    public static void ConfirmDiceResults()
    {
        CurrentPlayer.CmdConfirmDiceResults();
    }

    // CONFIRM DICE ROLL CHECK

    public static void ConfirmDiceRollCheckResults()
    {
        if (IsServer) CurrentPlayer.CmdConfirmDiceRollCheckResults();
    }

    // CONFIRM INFORM CRIT

    public static void CallInformCritWindow()
    {
        if (IsServer) CurrentPlayer.CmdCallInformCritWindow();
    }

    // SYNC DICE ROLL

    public static void SyncDiceResults()
    {
        if (IsServer) CurrentPlayer.CmdSyncDiceResults();
    }

    public static void SyncDiceRerollResults()
    {
        if (IsServer) CurrentPlayer.CmdSyncDiceRerollResults();
    }

    public static void CompareDiceSidesAgainstServer(DieSide[] dieSides)
    {
        if (!IsServer)
        {
            DiceRoll clientDiceRoll = DiceRoll.CurrentDiceRoll;

            bool syncIsNeeded = false;
            for (int i = 0; i < clientDiceRoll.DiceList.Count; i++)
            {
                if (clientDiceRoll.DiceList[i].GetModelFace() != dieSides[i])
                {
                    syncIsNeeded = true;
                    clientDiceRoll.DiceList[i].SetSide(dieSides[i]);
                    clientDiceRoll.DiceList[i].SetModelSide(dieSides[i]);
                }
            }

            if (syncIsNeeded)
            {
                clientDiceRoll.OrganizeDicePositions();
                Messages.ShowInfo("Dice results are synchronized with server");
            }
            /*else
            {
                Messages.ShowInfo("NO PROBLEMS");
            }*/
        }

        Network.FinishTask();
    }

    // DICE MODIFICATIONS

    public static void UseDiceModification(string diceModificationName)
    {
        CurrentPlayer.CmdUseDiceModification(diceModificationName);
    }

    // BARREL ROLL PLANNING

    public static void TryConfirmBarrelRoll(Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        CurrentPlayer.CmdTryConfirmBarrelRoll(shipPosition, movementTemplatePosition);
    }

    // BOOST PLANNING

    public static void TryConfirmBoostPosition(string SelectedBoostHelper)
    {
        CurrentPlayer.CmdTryConfirmBoostPosition(SelectedBoostHelper);
    }

    // DICE SELECTION SYNC

    public static void SyncSelectedDiceAndReroll()
    {
        if (IsServer) CurrentPlayer.CmdSyncSelectedDiceAndReroll();
    }

    // UI

    public static void EnableNetwork()
    {
        NetworkManagerHUD netUI = GameObject.Find("NetworkManager").GetComponentInChildren<NetworkManagerHUD>();
        netUI.showGUI = !netUI.showGUI;
    }

}
