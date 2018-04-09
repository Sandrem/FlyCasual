using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetworkExecuteWithCallback
{
    private Action callBack;
    private int responcesGot;
    public string TaskName;

    private NetworkExecuteWithCallback PreviousNetworkCallback;

    private int MAX_PLAYERS = 2;

    public NetworkExecuteWithCallback(string name, Action toExecute, Action toCallBack)
    {
        TaskName = name;
        Console.Write(name, LogTypes.Network, true, "yellow");

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
            Console.Write("Server confirmed finished task: " + responcesGot + "/" + MAX_PLAYERS, LogTypes.Network);

            if (responcesGot == MAX_PLAYERS)
            {
                Console.Write("Server allows to continue\n", LogTypes.Network, true, "yellow");
                UnregisterNetworkCallback();
                callBack();
            }
        }
    }
}
