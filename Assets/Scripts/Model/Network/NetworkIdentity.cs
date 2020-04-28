using Mirror;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    [Command]
    public void CmdStartNetworkGame()
    {
        Messages.ShowInfo("CmdStartNetworkGame");
        RpcStartNetworkGame();
    }

    [ClientRpc]
    private void RpcStartNetworkGame()
    {
        Messages.ShowInfo("RpcStartNetworkGame");

        SquadBuilder.StartNetworkGame();
    }

    [Command]
    public void CmdBattleIsReady()
    {
        Messages.ShowInfo("CmdBattleIsReady");
        RpcBattleIsReady();
    }

    [ClientRpc]
    private void RpcBattleIsReady()
    {
        Messages.ShowInfo("RpcBattleIsReady");
        Global.BattleIsReady();
    }

    [Command]
    public void CmdSendSquadToServer(string playerName, string title, string avatar, string squadString)
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
}
