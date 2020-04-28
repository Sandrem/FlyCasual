using GameCommands;
using Mirror;
using Players;
using SquadBuilderNS;
using System;
using System.Collections;
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

    // Match creation

    public static void CreateMatch(string roomName, string password)
    {
        Messages.ShowInfo("Create Match");
        NetworkManager.singleton.StartHost();
    }

    public static void BrowseMatches()
    {
        Messages.ShowInfo("Browse Matches");
    }

    public static void JoinCurrentRoomByParameters(string password)
    {
        Messages.ShowInfo("Join Current Room By Parameters");

        Global.Instance.StartCoroutine(JoinRoomCoroutine());
    }

    private static IEnumerator JoinRoomCoroutine()
    {
        NetworkManager.singleton.StartClient();

        yield return new WaitForSeconds(3f);

        SendClientInfoToServer();
    }

    private static void SendClientInfoToServer()
    {
        CurrentPlayer.CmdSendSquadToServer
        (
            Options.NickName,
            Options.Title,
            Options.Avatar,
            SquadBuilder.GetSquadInJson(PlayerNo.Player1).ToString()
        );
    }

    public static void CancelWaitingForOpponent()
    {
        Messages.ShowInfo("Cancel Waiting For Opponent");
        NetworkManager.singleton.StopHost();
    }

    public static void StartNetworkGame()
    {
        Messages.ShowInfo("Start Network Game");

        CurrentPlayer.CmdStartNetworkGame();
    }

    public static void BattleIsReady()
    {
        Messages.ShowInfo("Battle Is Ready");

        CurrentPlayer.CmdBattleIsReady();
    }

    // Game Interaction

    public static void SendCommand(GameCommand command)
    {
        Messages.ShowInfo("Send Command");

        CurrentPlayer.CmdSendCommand(command.ToString());
    }

    public static void RevertSubPhase()
    {
        throw new NotImplementedException();
    }

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        throw new NotImplementedException();
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
        Messages.ShowInfo("Finish Task");
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
