using GameCommands;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public static void SendChatMessage(string message)
    {
        CurrentPlayer.CmdSendChatMessage(message);
    }

    public static void SendCommand(GameCommand command)
    {
        Messages.ShowInfo("Send Command");
    }

    public static void CreateMatch(string roomName, string password)
    {
        Messages.ShowInfo("Create Match");
        NetworkManager.singleton.StartHost();
    }

    internal static void RevertSubPhase()
    {
        throw new NotImplementedException();
    }

    internal static void AssignManeuver(int shipId, string maneuverCode)
    {
        throw new NotImplementedException();
    }

    public static void BrowseMatches()
    {
        Messages.ShowInfo("Browse Matches");
    }

    internal static void GenerateRandom(Vector2 vector2, int v, Action<int[]> storePlayerWithInitiative, Action finishTrigger)
    {
        throw new NotImplementedException();
    }

    public static void JoinCurrentRoomByParameters(string password)
    {
        Messages.ShowInfo("Join Current Room By Parameters");

        NetworkManager.singleton.StartClient();
    }

    public static void CancelWaitingForOpponent()
    {
        Messages.ShowInfo("Cancel Waiting For Opponent");
        NetworkManager.singleton.StopHost();
    }

    public static void FinishTask()
    {
        Messages.ShowInfo("Finish Task");
    }

    public static void StartNetworkGame()
    {
        Messages.ShowInfo("Start Network Game");
    }

    public static void SelectTargetShip(int shipId)
    {
        Messages.ShowInfo("Select Target Ship");
    }

    public static void TryConfirmBarrelRoll(string templateName, Vector3 shipBasePosition, Vector3 movementTemplatePosition)
    {
        Messages.ShowInfo("Try Confirm Barrel Roll");
    }

    public static void PerformBarrelRoll()
    {
        Messages.ShowInfo("Perform Barrel Roll");
    }

    public static void CancelBarrelRoll()
    {
        Messages.ShowInfo("Cancel Barrel Roll");
    }

    public static void PerformDecloak()
    {
        Messages.ShowInfo("Perform Decloak");
    }

    public static void CancelDecloak()
    {
        Messages.ShowInfo("Cancel Decloak");
    }

    public static void TryConfirmBoostPosition(string selectedBoostHelper)
    {
        Messages.ShowInfo("Try Confirm Boost Position");
    }

    public static void PerformBoost()
    {
        Messages.ShowInfo("Perform Boost");
    }

    public static void CancelBoost()
    {
        Messages.ShowInfo("Cancel Boost");
    }

    public static void SyncDecks(int playerNo, int seed)
    {
        Messages.ShowInfo("Sync Decks");
    }

    public static void ReturnToMainMenu()
    {
        Messages.ShowInfo("Return To Main Menu");
    }

    public static void QuitToDesktop()
    {
        Messages.ShowInfo("Quit To Desktop");
    }
}
