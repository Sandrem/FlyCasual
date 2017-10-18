using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class Network
{
    public static NetworkPlayerController CurrentPlayer;

    public static string AllShipNames;

    public static bool IsNetworkGame
    {
        get { return CurrentPlayer != null; }
    }

    public static bool IsServer
    {
        get { return CurrentPlayer.IsServer; }
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

    // DECLARE COMBAT TARGET

    public static void DeclareTarget(int attackerId, int defenderId)
    {
        CurrentPlayer.CmdDeclareTarget(attackerId, defenderId);
    }

    // CONFIRM DICE RESULTS MODIFICATION

    public static void ConfirmDiceResults()
    {
        CurrentPlayer.CmdConfirmDiceResults();
    }

    // CONFIRM DICE ROLL CHECK

    public static void ConfirmDiceRollCheckResults()
    {
        CurrentPlayer.CmdConfirmDiceRollCheckResults();
    }

}
