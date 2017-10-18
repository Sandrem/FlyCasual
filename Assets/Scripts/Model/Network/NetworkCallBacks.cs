using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class Network
{
    private static Action callBack;
    private static int responcesGot;

    private static int MAX_PLAYERS = 2;

    public static void ExecuteWithCallBack(Action toExecute, Action toCallBack)
    {
        if (IsServer)
        {
            responcesGot = 0;
            callBack = toCallBack;
        }

        toExecute();
    }

    public static void ServerFinishTask()
    {
        if (IsServer) responcesGot++;
        if (responcesGot == MAX_PLAYERS) callBack();
    }

    public static void FinishTask()
    {
        CurrentPlayer.CmdFinishTask();
    }
}
