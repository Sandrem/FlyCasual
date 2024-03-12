﻿using Mirror;
using Players;

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
        if (this.isOwned)
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
        Global.StartNetworkGame();
    }

    [ClientRpc]
    private void RpcBattleIsReady()
    {
        Global.BattleIsReady();
    }

    [Command]
    public void CmdSyncAndStartGame(string playerName, string title, string avatar, string squadString, string clientAppVersion)
    {
        if (clientAppVersion == Global.CurrentVersion)
        {
            if (!Network.ConnectionIsEstablished)
            {
                RpcSendPlayerInfoToClients
                (
                    1,
                    Options.NickName,
                    Options.Title,
                    Options.Avatar,
                    Global.SquadBuilder.SquadLists[PlayerNo.Player1].GetSquadInJson().ToString()
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
                RpcDisconnectExtraClient("Sorry, the host have already found an opponent");
            }
        }
        else
        {
            RpcDisconnectExtraClient("Sorry, the host uses another version of the game: " + Global.CurrentVersion);
        }
    }

    [ClientRpc]
    private void RpcSendPlayerInfoToClients(int playerNo, string playerName, string title, string avatar, string squadString)
    {
        Global.PrepareOnlineMatchLists
        (
            playerNo,
            playerName,
            title,
            avatar,
            squadString
        );
    }

    [ClientRpc]
    private void RpcDisconnectExtraClient(string message)
    {
        if (!Network.ConnectionIsEstablished)
        {
            Messages.ShowError(message);
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

        if (this.isOwned)
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
