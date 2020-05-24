using GameCommands;
using Mirror;
using Players;
using SquadBuilderNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Network
{
    public static NetworkIdentity CurrentPlayer { get; set; }
    public static NetworkConnectionAttemptHandler ConnectionAttempt { get; set; }
    public static bool ConnectionIsEstablished { get; set; }

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
        ConnectionIsEstablished = false;
        NetworkManager.singleton.StartHost();
    }

    public static void BrowseMatches()
    {
        // Messages.ShowInfo("Browse Matches");
    }

    public static void JoinRoom(string password)
    {
        ConnectionIsEstablished = false;
        ConnectionAttempt = GameObject.FindObjectOfType<NetworkConnectionAttemptHandler>();

        Uri uri = new Uri(Network.ServerUri);
        NetworkManager.singleton.StartClient(uri);

        ConnectionAttempt.StartAttempt();
    }

    public static void SendClientInfoToServer()
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

    public static void SyncDecks(PlayerNo playerNo, int seed)
    {
        if (IsServer) CurrentPlayer.CmdSendCommand
        (
            DamageDecks.GenerateDeckShuffleCommand(playerNo, seed).ToString()
        );

        GameController.CheckExistingCommands();
    }

    // Misc

    public static void SendChatMessage(string message)
    {
        CurrentPlayer.CmdSendChatMessage(message);
    }

    public static void FinishTask()
    {
        CurrentPlayer.CmdFinishTask();
    }

    public static void ReturnToMainMenu()
    {
        if (CurrentPlayer != null)
        {
            CurrentPlayer.CmdPlayerIsDisconnected(IsServer);
        }
        else
        {
            Global.ReturnToMainMenu();
        }
    }

    public static void QuitToDesktop()
    {
        if (CurrentPlayer != null)
        {
            CurrentPlayer.CmdPlayerIsDisconnected(IsServer);
        }
        else
        {
            Application.Quit();
        }
    }
}
