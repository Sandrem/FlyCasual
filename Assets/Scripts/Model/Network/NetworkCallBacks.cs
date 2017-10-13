using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class Network
{
    private static Action callBack;
    private static int responcesGot;

    public static void ExecuteWithCallBack(Action toExecute, Action toCallBack)
    {
        responcesGot = 0;
        callBack = toCallBack;
        toExecute();
    }

    public static void ServerFinishTask()
    {
        responcesGot++;
        if (responcesGot == 2) callBack();
    }

    public static void FinishTask()
    {
        CurrentPlayer.CmdFinishTask();
    }
}
