using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public static partial class Network
{
    public static NetworkPlayerController CurrentPlayer;

    public static NetworkExecuteWithCallback LastNetworkCallback;

    public static string AllShipNames;

    public static JSONObject SquadJsons;

    public static MatchInfoSnapshot SelectedMatch;

    public static bool IsNetworkGame
    {
        get { return CurrentPlayer != null; }
    }

    public static bool IsServer
    {
        get { return CurrentPlayer.IsServer; }
    }

    // SQUAD LISTS

    public static void ImportSquad(string squadList, bool isServer)
    {
        string squadName = (isServer) ? "Server" : "Client";
        JSONObject squadListJson = new JSONObject(squadList);
        SquadJsons.AddField(squadName, squadListJson);
    }

    public static void StoreSquadList(string localSquadList, bool isServer)
    {
        CurrentPlayer.CmdStoreSquadList(localSquadList.ToString(), isServer);
    }

    // TESTS

    public static void Test()
    {
        CurrentPlayer.CmdTest();
    }

    public static void UpdateAllShipNames(string text)
    {
        CurrentPlayer.CmdUpdateAllShipNames(text);
    }

    public static void CallBacksTest()
    {
        CurrentPlayer.CmdCallBacksTest();
    }

    // CALLBACKS

    public static void FinishTask()
    {
        string logEntryPostfix = (IsServer) ? "" : "\n";
        Console.Write("Client finished task" + logEntryPostfix, LogTypes.Network);
        CurrentPlayer.CmdFinishTask();
    }

    public static void ServerFinishTask()
    {
        LastNetworkCallback.ServerFinishTask();
    }

    // SELECT SHIP

    public static void RevertSubPhase()
    {
        if (IsServer) CurrentPlayer.CmdRevertSubPhase();
    }

    // TOOLS

    public static void ShowMessage(string text)
    {
        CurrentPlayer.CmdShowMessage(text);
    }

    // BATTLE START

    public static void StartNetworkGame()
    {
        CurrentPlayer.CmdStartNetworkGame();
    }

    // DECISIONS

    public static void TakeDecision(string decisionName)
    {
        CurrentPlayer.CmdTakeDecision(decisionName);
    }

    // SETUP

    public static void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
    {
        CurrentPlayer.CmdConfirmShipSetup(shipId, position, angles);
    }

    // ASSING MANEUVER

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        CurrentPlayer.CmdAssignManeuver(shipId, maneuverCode);
    }

    // NEXT BUTTON

    public static void NextButtonEffect()
    {
        CurrentPlayer.CmdNextButtonEffect();
    }

    // SKIP BUTTON

    public static void SkipButtonEffect()
    {
        CurrentPlayer.CmdSkipButtonEffect();
    }

    // PERFORM MANEUVER

    public static void PerformStoredManeuver(int shipId)
    {
        CurrentPlayer.CmdPerformStoredManeuver(shipId);
    }

    // PERFORM BARREL ROLL

    public static void PerformBarrelRoll()
    {
        if (IsServer) CurrentPlayer.CmdPerformBarrelRoll();
    }

    public static void CancelBarrelRoll()
    {
        if (IsServer) CurrentPlayer.CmdCancelBarrelRoll();
    }

    // PERFORM DECLOAK

    public static void PerformDecloak()
    {
        if (IsServer) CurrentPlayer.CmdPerformDecloak();
    }

    public static void CancelDecloak()
    {
        if (IsServer) CurrentPlayer.CmdCancelDecloak();
    }

    // PERFORM BOOST

    public static void PerformBoost()
    {
        if (IsServer) CurrentPlayer.CmdPerformBoost();
    }

    public static void CancelBoost()
    {
        if (IsServer) CurrentPlayer.CmdCancelBoost();
    }

    // DECLARE COMBAT TARGET

    public static void DeclareTarget(int attackerId, int defenderId)
    {
        CurrentPlayer.CmdDeclareTarget(attackerId, defenderId);
    }

    // SELECT TARGET SHIP

    public static void SelectTargetShip(int targetId)
    {
        CurrentPlayer.CmdSelectTargetShip(targetId);
    }

    // CONFIRM DICE RESULTS MODIFICATION

    public static void ConfirmDiceResults()
    {
        CurrentPlayer.CmdConfirmDiceResults();
    }

    // CONFIRM DICE ROLL CHECK

    public static void ConfirmDiceRollCheckResults()
    {
        if (IsServer) CurrentPlayer.CmdConfirmDiceRollCheckResults();
    }

    // CONFIRM INFORM CRIT

    public static void CallInformCritWindow()
    {
        if (IsServer) CurrentPlayer.CmdCallInformCritWindow();
    }

    // SYNC DICE ROLL

    public static void SyncDiceResults()
    {
        if (IsServer) CurrentPlayer.CmdSyncDiceResults();
    }

    public static void SyncDiceRerollResults()
    {
        if (IsServer) CurrentPlayer.CmdSyncDiceRerollResults();
    }

    public static void CompareDiceSidesAgainstServer(DieSide[] dieSides)
    {
        if (!IsServer)
        {
            DiceRoll clientDiceRoll = DiceRoll.CurrentDiceRoll;

            bool syncIsNeeded = false;
            for (int i = 0; i < clientDiceRoll.DiceList.Count; i++)
            {
                if (clientDiceRoll.DiceList[i].GetModelFace() != dieSides[i])
                {
                    syncIsNeeded = true;
                    clientDiceRoll.DiceList[i].SetSide(dieSides[i]);
                    clientDiceRoll.DiceList[i].SetModelSide(dieSides[i]);
                }
            }

            if (syncIsNeeded)
            {
                clientDiceRoll.OrganizeDicePositions();
                Messages.ShowInfo("Dice results are synchronized with server");
            }
            /*else
            {
                Messages.ShowInfo("NO PROBLEMS");
            }*/
        }

        Network.FinishTask();
    }

    // DICE MODIFICATIONS

    public static void UseDiceModification(string diceModificationName)
    {
        CurrentPlayer.CmdUseDiceModification(diceModificationName);
    }

    // BARREL ROLL PLANNING

    public static void TryConfirmBarrelRoll(Vector3 shipPosition, Vector3 movementTemplatePosition)
    {
        CurrentPlayer.CmdTryConfirmBarrelRoll(shipPosition, movementTemplatePosition);
    }

    // DECLOAK PLANNING

    public static void TryConfirmDecloak(Vector3 shipPosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
    {
        CurrentPlayer.CmdTryConfirmDecloak(shipPosition, decloakHelper, movementTemplatePosition, movementTemplateAngles);
    }

    // BOOST PLANNING

    public static void TryConfirmBoostPosition(string SelectedBoostHelper)
    {
        CurrentPlayer.CmdTryConfirmBoostPosition(SelectedBoostHelper);
    }

    // DICE SELECTION SYNC

    public static void SyncSelectedDiceAndReroll()
    {
        if (IsServer) CurrentPlayer.CmdSyncSelectedDiceAndReroll();
    }

    // UI

    public static void EnableNetwork()
    {
        NetworkManagerHUD netUI = GameObject.Find("NetworkManager").GetComponentInChildren<NetworkManagerHUD>();
        netUI.showGUI = !netUI.showGUI;
    }

    // SWARM MANAGER

    public static void SetSwarmManagerManeuver(string maneuverCode)
    {
        CurrentPlayer.CmdSetSwarmManagerManeuver(maneuverCode);
    }

    // UI

    public static void CreateMatch(string roomName, string password)
    {
        GameObject createRoomButton = GameObject.Find("UI/Panels/CreateMatchPanel/ContolsPanel/CreateRoomButton");
        createRoomButton.SetActive(false);

        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.CreateMatch(roomName, 2, true, password, "", "", 0, 0, OnInternetMatchCreate);
    }

    private static void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        GameObject createRoomButton = GameObject.Find("UI/Panels/CreateMatchPanel/ContolsPanel/CreateRoomButton");
        createRoomButton.SetActive(true);

        if (success)
        {
            string roomName = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Name").GetComponentInChildren<InputField>().text;

            GameObject WaitingForOpponentPanelGO = GameObject.Find("UI/Panels").transform.Find("WaitingForOpponentsPanel").gameObject;
            WaitingForOpponentPanelGO.transform.Find("Panel").Find("NameText").GetComponent<Text>().text = roomName;
            MainMenu.CurrentMainMenu.ChangePanel(WaitingForOpponentPanelGO);

            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);
            NetworkManager.singleton.StartHost(hostInfo);
        }
        else
        {
            Messages.ShowError("Create match failed");
        }
    }

    public static void BrowseMatches()
    {
        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.ListMatches(0, int.MaxValue, "", false, 0, 0, OnInternetMatchList);
    }

    private static void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                Messages.ShowInfo("A list of matches was returned");
                ShowListOfRooms(matches);
            }
            else
            {
                Messages.ShowError("No matches in requested room!");
            }
        }
        else
        {
            Messages.ShowError("Couldn't connect to match maker");
        }
    }

    private static void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Messages.ShowInfo("Successfully joined match");

            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            if (SelectedMatch.isPrivate)
            {
                Messages.ShowError("Cannot join match - check password");
            }
            else
            {
                Messages.ShowError("Cannot join match");
            }
        }
    }

    public static void ShowListOfRooms(List<MatchInfoSnapshot> matchesList)
    {
        float FREE_SPACE = 10f;

        ClearRoomsList();

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/MatchPanel", typeof(GameObject));
        GameObject MatchsPanel = GameObject.Find("UI/Panels").transform.Find("JoinMatchPanel").Find("Scroll View/Viewport/Content").gameObject;

        RectTransform matchsPanelRectTransform = MatchsPanel.GetComponent<RectTransform>();
        Vector3 currentPosition = new Vector3(matchsPanelRectTransform.sizeDelta.x / 2 + FREE_SPACE, -FREE_SPACE, MatchsPanel.transform.localPosition.z);

        foreach (var match in matchesList)
        {
            Transform existingModeRecord = MatchsPanel.transform.Find(match.name);
            GameObject MatchRecord;

            MatchRecord = MonoBehaviour.Instantiate(prefab, MatchsPanel.transform);
            MatchRecord.transform.localPosition = currentPosition;
            MatchRecord.name = match.networkId.ToString();

            MatchRecord.transform.Find("Name").GetComponent<Text>().text = match.name;
            MatchRecord.transform.Find("Lock").gameObject.SetActive(match.isPrivate);
            MatchRecord.transform.Find("Join").gameObject.SetActive(match.currentSize == 1);

            MatchRecord.transform.Find("Join").GetComponent<Button>().onClick.AddListener(delegate { ClickJoinRoom(match); });

            currentPosition = new Vector3(currentPosition.x, currentPosition.y - 90 - FREE_SPACE, currentPosition.z);
            matchsPanelRectTransform.sizeDelta = new Vector2(matchsPanelRectTransform.sizeDelta.x, matchsPanelRectTransform.sizeDelta.y + 90 + FREE_SPACE);
        }
    }

    public static void ClearRoomsList()
    {
        GameObject MatchsPanel = GameObject.Find("UI/Panels").transform.Find("JoinMatchPanel").Find("Scroll View/Viewport/Content").gameObject;
        foreach (Transform matchRecord in MatchsPanel.transform)
        {
            GameObject.Destroy(matchRecord.gameObject);            
        }
    }

    public static void ClickJoinRoom(MatchInfoSnapshot match)
    {
        Messages.ShowInfo("Joining room...");
        SelectedMatch = match;

        if (!match.isPrivate)
        {
            JoinCurrentRoomByParameters();
        }
        else
        {
            GameObject JoinPrivateMatchPanelGO = GameObject.Find("UI/Panels").transform.Find("JoinPrivateMatchPanel").gameObject;
            JoinPrivateMatchPanelGO.transform.Find("Panel").Find("Name").Find("InputField").GetComponent<InputField>().text = match.name;
            MainMenu.CurrentMainMenu.ChangePanel(JoinPrivateMatchPanelGO);
        }
    }

    public static void JoinCurrentRoomByParameters(string password = "")
    {
        NetworkManager.singleton.matchMaker.JoinMatch(SelectedMatch.networkId, password, "", "", 0, 0, OnJoinInternetMatch);
    }

    public static void CancelWaitingForOpponent()
    {
        NetworkServer.Shutdown();
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StopMatchMaker();
    }

}
