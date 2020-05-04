using GameCommands;
using Mirror;
using Players;
using Ship;
using SquadBuilderNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Network
{
    public static NetworkIdentity CurrentPlayer { get; set; }

    public static bool IsNetworkGame
    {
        get { return CurrentPlayer != null; }
    }

    public static bool IsServer
    {
        get { return CurrentPlayer.IsServer; }
    }

    public static string ServerUri { get; set; } = "tcp4://127.0.0.1";

    // Match creation

    public static void CreateMatch(string roomName, string password)
    {
        NetworkManager.singleton.StartHost();
    }

    public static void BrowseMatches()
    {
        // Messages.ShowInfo("Browse Matches");
    }

    public static void JoinRoom(string password)
    {
        Global.Instance.StartCoroutine(JoinRoomCoroutine());
    }

    private static IEnumerator JoinRoomCoroutine()
    {
        Uri uri = new Uri(Network.ServerUri);
        NetworkManager.singleton.StartClient(uri);

        yield return new WaitForSeconds(3f);

        // TODO: When connected
        SendClientInfoToServer();
    }

    private static void SendClientInfoToServer()
    {
        CurrentPlayer.CmdSyncAndStartGame
        (
            Options.NickName,
            Options.Title,
            Options.Avatar,
            SquadBuilder.GetSquadInJson(PlayerNo.Player1).ToString()
        );
    }

    public static void CancelWaitingForOpponent()
    {
        NetworkManager.singleton.StopHost();
    }

    // Game Interaction

    public static void SendCommand(GameCommand command)
    {
        CurrentPlayer.CmdSendCommand(command.ToString());
    }

    //Command that is executed only if server sent it - avoids duplication of commands
    public static void SendServerCommand(GameCommand command)
    {
        if (IsServer) CurrentPlayer.CmdSendCommand(command.ToString());
    }

    public static void RevertSubPhase()
    {
        throw new NotImplementedException();
    }

    public static void SyncDecks(PlayerNo playerNo, int seed)
    {
        if (IsServer) CurrentPlayer.CmdSendCommand
        (
            DamageDecks.GenerateDeckShuffleCommand(playerNo, seed).ToString()
        );
    }

    // Misc

    public static void SendChatMessage(string message)
    {
        CurrentPlayer.CmdSendChatMessage(message);
    }

    public static void GenerateRandom(Vector2 vector2, int v, Action<int[]> storePlayerWithInitiative, Action finishTrigger)
    {
        throw new NotImplementedException();
    }

    public static void FinishTask()
    {
        CurrentPlayer.CmdFinishTask();
    }

    public static void ReturnToMainMenu()
    {
        // Messages.ShowInfo("Return To Main Menu");
    }

    public static void QuitToDesktop()
    {
        // Messages.ShowInfo("Quit To Desktop");
    }
}
