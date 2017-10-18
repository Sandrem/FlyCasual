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

    public static void GenerateRandom(Vector2 range, int count, Action<int[]> store, Action callBack)
    {
        storeGeneratedRandomValues = store;
        generateRandomValuesCallBack = callBack;

        if (IsServer) CurrentPlayer.CmdGenerateRandomValues(range, count);
    }

    public static void StoreGeneratedRandomValues(int[] randomHolders)
    {
        storeGeneratedRandomValues(randomHolders);
        Network.FinishTask();
    }

    public static void GenerateRandomValuesCallBack()
    {
        generateRandomValuesCallBack();
    }

}

public partial class NetworkPlayerController : NetworkBehaviour
{
    [Command]
    public void CmdGenerateRandomValues(Vector2 range, int count)
    {
        int[] randomHolder = new int[count];
        for (int i = 0; i < count; i++)
        {
            randomHolder[i] = UnityEngine.Random.Range((int)range.x, (int)range.y+1);
        }

        new NetworkExecuteWithCallback(
            delegate { CmdStoreGeneratedRandomValues(randomHolder); },
            CmdGenerateRandomValuesCallBack
        );
    }

    [Command]
    private void CmdStoreGeneratedRandomValues(int[] randomHolder)
    {
        RpcStoreGeneratedRandomValues(randomHolder);
    }

    [ClientRpc]
    private void RpcStoreGeneratedRandomValues(int[] randomHolder)
    {
        Network.StoreGeneratedRandomValues(randomHolder);
    }

    [Command]
    private void CmdGenerateRandomValuesCallBack()
    {
        RpcGenerateRandomValuesCallBack();
    }

    [ClientRpc]
    private void RpcGenerateRandomValuesCallBack()
    {
        Network.GenerateRandomValuesCallBack();
    }
}
