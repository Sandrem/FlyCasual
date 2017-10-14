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

    public static void LoadBattleScene()
    {
        CurrentPlayer.CmdLoadBattleScene();
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
}
