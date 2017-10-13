using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Network
{
    public static NetworkPlayer CurrentPlayer;

    // TESTS

    public static void Test()
    {
        CurrentPlayer.CmdTest();
    }

    public static void RosterTest()
    {
        CurrentPlayer.CmdRosterTest();
    }

    // NETWORK

    public static void ShowMessage(string text)
    {
        CurrentPlayer.CmdShowMessage(text);
    }
}
