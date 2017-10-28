using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetworkExecuteWithCallback
{
    private Action callBack;
    private int responcesGot;

    private NetworkExecuteWithCallback PreviousNetworkCallback;

    private int MAX_PLAYERS = 2;

    public NetworkExecuteWithCallback(Action toExecute, Action toCallBack)
    {
        if (Network.IsServer)
        {
            responcesGot = 0;
            callBack = toCallBack;
        }

        RegisterNetworkCallback();

        toExecute();
    }

    private void RegisterNetworkCallback()
    {
        if (Network.LastNetworkCallback != null) PreviousNetworkCallback = Network.LastNetworkCallback;

        Network.LastNetworkCallback = this;
    }

    private void UnregisterNetworkCallback()
    {
        Network.LastNetworkCallback = PreviousNetworkCallback;
    }

    public void ServerFinishTask()
    {
        if (Network.IsServer)
        {
            responcesGot++;

            if (responcesGot == MAX_PLAYERS)
            {
                UnregisterNetworkCallback();
                callBack();
            }
        }
    }
}
