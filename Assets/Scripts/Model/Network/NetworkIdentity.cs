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
    public void CmdSyncSquads()
    {
        Messages.ShowInfo("CmdSyncSquads");
        RpcSyncSquads();
    }

    [ClientRpc]
    private void RpcSyncSquads()
    {
        Messages.ShowInfo("RpcSyncSquads");
        SquadBuilder.CreateDummySquads();
    }

    [Command]
    public void CmdSendSquadToServer(string squadString)
    {
        RpcSendFinalSquadsToClients
        (
            SquadBuilder.GetSquadInJson(PlayerNo.Player1).ToString(),
            squadString
        );

        RpcStartNetworkGame();
    }

    [ClientRpc]
    private void RpcSendFinalSquadsToClients(string squad1String, string squad2String)
    {
        SquadBuilder.LoadBothSquadsFromJson(squad1String, squad2String);
    }
}
