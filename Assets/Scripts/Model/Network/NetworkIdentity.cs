using Mirror;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Players;
using UnityEngine;
using SubPhases;

public class NetworkIdentity : NetworkBehaviour
{
    public bool IsServer
    {
        get { return isServer; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (this.hasAuthority)
        {
            Network.CurrentPlayer = this;
        }
    }

    [Command]
    public void CmdSendCommand(string commandline)
    {
        RpcSendCommand(commandline);
    }

    [ClientRpc]
    public void RpcSendCommand(string commandline)
    {
        GameController.SendCommand(GameController.GenerateGameCommand(commandline, true));
    }

    [Command]
    public void CmdSendChatMessage(string message)
    {
        RpcSendChatMessage(message);
    }

    [ClientRpc]
    public void RpcSendChatMessage(string message)
    {
        Messages.ShowInfo(message);
    }

    [ClientRpc]
    private void RpcStartNetworkGame()
    {
        Network.ConnectionIsEstablished = true;
        SquadBuilder.StartNetworkGame();
    }

    [ClientRpc]
    private void RpcBattleIsReady()
    {
        Global.BattleIsReady();
    }

    [Command]
    public void CmdSyncAndStartGame(string playerName, string title, string avatar, string squadString)
    {
        if (!Network.ConnectionIsEstablished)
        {
            RpcSendPlayerInfoToClients
            (
                1,
                Options.NickName,
                Options.Title,
                Options.Avatar,
                SquadBuilder.GetSquadInJson(PlayerNo.Player1).ToString()
            );

            RpcSendPlayerInfoToClients
            (
                2,
                playerName,
                title,
                avatar,
                squadString
            );

            RpcStartNetworkGame();

            new NetworkTask
            (
                "Load Battle Scene",
                RpcBattleIsReady
            );
        }
        else
        {
            RpcDisconnectExtraClient();
        }
    }

    [ClientRpc]
    private void RpcSendPlayerInfoToClients(int playerNo, string playerName, string title, string avatar, string squadString)
    {
        SquadBuilder.PrepareOnlineMatchLists
        (
            playerNo,
            playerName,
            title,
            avatar,
            squadString
        );
    }

    [ClientRpc]
    private void RpcDisconnectExtraClient()
    {
        if (!Network.ConnectionIsEstablished)
        {
            Messages.ShowError("Sorry, the host have already found an opponent");
            Network.ConnectionAttempt.AbortAttempt();
        }
    }

    [Command]
    public void CmdFinishTask()
    {
        NetworkTask.CurrentTask.FinishOne();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (this.hasAuthority)
        {
            if (Network.ConnectionAttempt != null) Network.ConnectionAttempt.StopAttempt();

            Network.CurrentPlayer = this;

            if (!Network.IsServer)
            {
                Network.SendClientInfoToServer();
            }
        }
    }

    [Command]
    public void CmdPlayerIsDisconnected(bool isServerDisconnected)
    {
        RpcPlayerIsDisconnected(isServerDisconnected);
    }

    [ClientRpc]
    private void RpcPlayerIsDisconnected(bool isServerDisconnected)
    {
        if (Network.IsServer != isServerDisconnected)
        {
            Phases.EndGame();
            Global.ShowAnotherPlayerDisconnected();
        }
        else
        {

            if (Network.IsServer)
            {
                NetworkManager.singleton.StopServer();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }

            Global.ReturnToMainMenu();
        }
    }
}
