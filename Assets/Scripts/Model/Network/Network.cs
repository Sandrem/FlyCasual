using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class Network
{
    public static NetworkPlayer CurrentPlayer;

    public static string AllShipNames;

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
}
