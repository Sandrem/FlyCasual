using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static partial class Network
{
    private static Action<int[]> storeGeneratedRandomValues;
    private static Action generateRandomValuesCallBack;

    private static int[] storedRandomResults;

    private static bool isReady = false;

    public static void GenerateRandom(Vector2 range, int count, Action<int[]> store, Action callBack)
    {
        storeGeneratedRandomValues = store;
        generateRandomValuesCallBack = callBack;

        if (IsServer) CurrentPlayer.CmdGenerateRandomValues(range, count);

        if (isReady)
        {
            InvokeStoreGeneratedValue();
            InvokeCallback();
        }
    }

    public static void StoreGeneratedRandomValues(int[] randomHolders)
    {
        storedRandomResults = randomHolders;
        if (storeGeneratedRandomValues != null)
        {
            InvokeStoreGeneratedValue();
        }
        else
        {
            isReady = true;
        }
        
        Network.FinishTask();
    }

    public static void GenerateRandomValuesCallBack()
    {
        if (generateRandomValuesCallBack != null) InvokeCallback();
    }

    private static void InvokeStoreGeneratedValue()
    {
        storeGeneratedRandomValues(storedRandomResults);
        storeGeneratedRandomValues = null;
    }

    private static void InvokeCallback()
    {
        Action callback = generateRandomValuesCallBack;
        generateRandomValuesCallBack = null;
        isReady = false;

        callback();
    }

}

public partial class NetworkPlayerController : NetworkBehaviour
{
    [Command]
    public void CmdGenerateRandomValues(Vector2 range, int count)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdGenerateRandomValues");
        int[] randomHolder = new int[count];
        for (int i = 0; i < count; i++)
        {
            randomHolder[i] = UnityEngine.Random.Range((int)range.x, (int)range.y+1);
        }

        new NetworkExecuteWithCallback(
            "Wait sync random numbers then callback",
            delegate { CmdStoreGeneratedRandomValues(randomHolder); },
            CmdGenerateRandomValuesCallBack
        );
    }

    [Command]
    private void CmdStoreGeneratedRandomValues(int[] randomHolder)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdStoreGeneratedRandomValues");
        RpcStoreGeneratedRandomValues(randomHolder);
    }

    [ClientRpc]
    private void RpcStoreGeneratedRandomValues(int[] randomHolder)
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcStoreGeneratedRandomValues");
        if (DebugManager.DebugNetwork)
        {
            string str = "RNG: ";
            for (int i = 0; i < randomHolder.Length; i++)
            {
                str += randomHolder[i] + "; ";
            }
            UI.AddTestLogEntry(str);
        }

        Network.StoreGeneratedRandomValues(randomHolder);
    }

    [Command]
    private void CmdGenerateRandomValuesCallBack()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("S: CmdGenerateRandomValuesCallBack");
        RpcGenerateRandomValuesCallBack();
    }

    [ClientRpc]
    private void RpcGenerateRandomValuesCallBack()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("C: RpcGenerateRandomValuesCallBack");
        Network.GenerateRandomValuesCallBack();
    }
}
