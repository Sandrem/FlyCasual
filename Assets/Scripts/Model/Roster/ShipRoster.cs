using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;
using Ship;
using SquadBuilderNS;
using System;
using GameCommands;
using Obstacles;
using Remote;

public static partial class Roster
{
    //Players

    public static List<GenericPlayer> Players;

    public static GenericPlayer Player1 { get { return Players.Find(n => n.PlayerNo == PlayerNo.Player1); } }
    public static GenericPlayer Player2 { get { return Players.Find(n => n.PlayerNo == PlayerNo.Player2); } }

    //Ships

    public static Dictionary<string, GenericShip> AllUnits;
    public static Dictionary<string, GenericShip> AllShips { get { return AllUnits.Where(n => !(n.Value is GenericRemote)).ToDictionary(n => n.Key, m => m.Value); } }
    public static Dictionary<string, GenericShip> AllRemotes { get { return AllUnits.Where(n => n.Value is GenericRemote).ToDictionary(n => n.Key, m => m.Value); } }

    public static Dictionary<string, GenericShip> ShipsPlayer1 { get { return Player1.Ships; } }
    public static Dictionary<string, GenericShip> ShipsPlayer2 {get { return Player2.Ships; } }

    public static List<GenericShip> Reserve;

    public static GenericPlayer GetOpponent()
    {
        PlayerNo playerNo = PlayerNo.Player2;

        if (Network.IsNetworkGame && !Network.IsServer) playerNo = PlayerNo.Player1;

        return GetPlayer(playerNo);
    }

    public static GenericPlayer GetThisPlayer()
    {
        PlayerNo playerNo = PlayerNo.Player1;

        if (Network.IsNetworkGame && !Network.IsServer) playerNo = PlayerNo.Player2;

        return GetPlayer(playerNo);
    }

    // SQUADRONS

    private static IEnumerator PrepareSquadrons()
    {
        GameInitializer.SetState(typeof(SquadsSyncCommand));

        if (ReplaysManager.Mode == ReplaysMode.Write)
        {
            foreach (var squad in SquadBuilder.SquadLists)
            {
                squad.SavedConfiguration["description"].str = squad.SavedConfiguration["description"].str.Replace("\n", "");

                GameController.SendCommand
                (
                    GenerateSyncSquadCommand
                    (
                        squad.PlayerNo.ToString(),
                        squad.PlayerType.ToString(),
                        Options.Title,
                        Options.Avatar,
                        squad.SavedConfiguration.ToString()
                    )
                );
            }

            GameController.CheckExistingCommands();
        }
        else if (ReplaysManager.Mode == ReplaysMode.Read)
        {
            GameController.CheckExistingCommands();
        }

        while (GameInitializer.AcceptsCommandType == typeof(SquadsSyncCommand) && GameInitializer.CommandsReceived < 2)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public static GameCommand GenerateSyncSquadCommand(string playerName, string playerType, string title, string avatar, string squadString)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("player", playerName);
        parameters.AddField("type", playerType);
        parameters.AddField("title", title);
        parameters.AddField("list", JSONObject.Create(squadString));

        return GameController.GenerateGameCommand(
            GameCommandTypes.SquadsSync,
            null,
            parameters.ToString()
        );
    }

    //PLAYERS CREATION

    private static void CreatePlayers()
    {
        foreach (var squadList in SquadBuilder.SquadLists)
        {
            Type playerType = squadList.PlayerType;

            bool isHotacAi = (playerType == typeof(HotacAiPlayer));

            if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                playerType = typeof(ReplayPlayer);
            }

            GenericPlayer player = CreatePlayer(playerType, squadList.PlayerNo);
            player.UsesHotacAiRules = isHotacAi;

            Players.Add(player);
        }
    }

    private static GenericPlayer CreatePlayer(System.Type type, PlayerNo playerNo)
    {
        GenericPlayer player = (GenericPlayer) System.Activator.CreateInstance(type);
        player.SetPlayerNo(playerNo);
        return player;
    }

    //SHIP CREATION

    private static void SpawnAllShips()
    {
        ObstaclesManager.Instance.ChosenObstacles = new List<GenericObstacle>();

        foreach (var squadList in SquadBuilder.SquadLists)
        {
            SquadBuilder.SetPlayerSquadFromImportedJson(squadList.Name, squadList.SavedConfiguration, squadList.PlayerNo, delegate { });

            if (Roster.GetPlayer(squadList.PlayerNo).PlayerType != PlayerType.Ai)
            {
                JSONObject playerInfo = squadList.SavedConfiguration.GetField("PlayerInfo");
                Roster.GetPlayer(squadList.PlayerNo).NickName = playerInfo.GetField("NickName").str;
                Roster.GetPlayer(squadList.PlayerNo).Title = playerInfo.GetField("Title").str;
                Roster.GetPlayer(squadList.PlayerNo).Avatar = playerInfo.GetField("Avatar").str;
            }

            Roster.GetPlayer(squadList.PlayerNo).SquadCost = squadList.Points;

            Roster.GetPlayer(squadList.PlayerNo).ChosenObstacles = new List<GenericObstacle>()
            {
                ObstaclesManager.GenerateObstacle(squadList.ChosenObstacles[0].ShortName, squadList.PlayerNo),
                ObstaclesManager.GenerateObstacle(squadList.ChosenObstacles[1].ShortName, squadList.PlayerNo),
                ObstaclesManager.GenerateObstacle(squadList.ChosenObstacles[2].ShortName, squadList.PlayerNo)
            };
        }

        // Keep order, ships must have same ID on both clients
        ShipFactory.Initialize();
        foreach (SquadBuilderShip shipConfig in SquadBuilder.GetSquadList(PlayerNo.Player1).GetShips())
        {
            GenericShip newShip = ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }
        foreach (SquadBuilderShip shipConfig in SquadBuilder.GetSquadList(PlayerNo.Player2).GetShips())
        {
            GenericShip newShip = ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }

        BoardTools.Board.SetShips();
    }

    public static void AddShipToLists(GenericShip newShip)
    {
        AllUnits.Add(newShip.GetTag(), newShip);
        newShip.Owner.Units.Add(newShip.GetTag(), newShip);
    }

    //SHIP DESTRUCTION

    public static void RemoveDestroyedShip(string id)
    {
        HideShip(id);

        OrganizeRosters();
    }

    public static void HideShip(string id)
    {
        var ship = GetShipById(id);

        if (ship != null)
        {
            ship.SetActive(false);
            ship.SetPosition(new Vector3(0, -100, 0));
            ship.InfoPanel.SetActive(false);
            ship.Owner.Units.Remove(id);
            AllUnits.Remove(id);
        }
    }

    public static void DockShip(string id)
    {
        DockShip(GetShipById(id));
    }

    public static void DockShip(GenericShip ship)
    {
        if (ship != null)
        {
            ship.SetActive(false);
            TogglePanelActive(ship, false);
            ship.SetDockedName(true);
            string shipIdKey = ship.Owner.Ships.First(n => n.Value == ship).Key;
            ship.Owner.Units.Remove(shipIdKey);
            AllUnits.Remove(shipIdKey);
        }
    }

    public static void UndockShip(GenericShip ship)
    {
        if (ship != null)
        {
            ship.SetActive(true);
            TogglePanelActive(ship, true);
            ship.SetDockedName(false);
            ship.Owner.Units.Add("ShipId:" + ship.ShipId, ship);
            AllUnits.Add("ShipId:" + ship.ShipId, ship);
        }
    }

    public static void ShowShip(GenericShip ship)
    {
        if (ship != null)
        {
            ship.SetActive(true);
            ship.InfoPanel.SetActive(true);
            ship.Owner.Units.Add("ShipId:" + ship.ShipId, ship);
            AllUnits.Add("ShipId:" + ship.ShipId, ship);
        }
    }

    //TOOLS

    public static GenericShip GetShipById(string id)
    {
		if (AllUnits.Any (x => x.Key == id))
        {
			return AllUnits[id];
		}

		return null;
    }

    public static GenericPlayer GetPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? Roster.Player1 : Roster.Player2;
    }

    public static GenericPlayer GetPlayer(int playerNo)
    {
        return (playerNo == 1) ? Roster.Player1 : Roster.Player2;
    }

    public static int AnotherPlayer(int player)
    {
        return (player == 1) ? 2 : 1;
    }

    public static PlayerNo AnotherPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? PlayerNo.Player2 : PlayerNo.Player1;
    }

    //FIND SHIPS BY REQUEST

    public static Dictionary<string, GenericShip> ListSamePlayerAndPilotSkill(PlayerNo playerNo, int pilotSkill)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.State.Initiative == pilotSkill
            select n;

        return results.ToDictionary(t => t.Key, t => t.Value);
    }

    public static Dictionary<int, int> ListAnotherPlayerButSamePilotSkill(int previousPilotSkill, int PilotSkillSubPhasePlayer)
    {
        var results =
            from n in AllShips
            where n.Value.State.Initiative == previousPilotSkill
            where n.Value.Owner.Id != PilotSkillSubPhasePlayer
            select n;

        return results.ToDictionary(t => previousPilotSkill, t => t.Value.Owner.Id);
    }

    // CHECK ALL SHIPS IN ROSTER

    public static bool AllManuversAreAssigned(PlayerNo playerNo)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.AssignedManeuver == null && !RulesList.IonizationRule.IsIonized(n.Value)
            select n;

        //if (results.Count() > 0) Game.UI.ShowError("Not all ship are assigned their maneuvers");
        return (results.Count() == 0);
    }

    public static bool AllManueversArePerformed()
    {
        var results =
            from n in AllShips
            where n.Value.IsManeuverPerformed == false
            select n;

        if (results.Count() > 0) Messages.ShowErrorToHuman("Not all ships have executed their maneuvers");
        return (results.Count() == 0);
    }

    public static bool NoSamePlayerAndPilotSkillNotAttacked()
    {
        Dictionary<string, GenericShip> samePlayerAndPilotSkill = ListSamePlayerAndPilotSkill(Phases.CurrentSubPhase.RequiredPlayer, Phases.CurrentSubPhase.RequiredInitiative);
        foreach (var item in samePlayerAndPilotSkill)
        {
            if (item.Value.IsAttackPerformed == false)
            {
                return false;
            }
        }
        return true;
    }

    //TODO: Rework

    public static int CheckIsAnyTeamIsEliminated()
    {
        int result = 0;
        if (Roster.GetPlayer(PlayerNo.Player1).Ships.Count == 0 && !Roster.Reserve.Any(n => n.Owner.PlayerNo == PlayerNo.Player1))
        {
            result += 1;
        }
        if (Roster.GetPlayer(PlayerNo.Player2).Ships.Count == 0 && !Roster.Reserve.Any(n => n.Owner.PlayerNo == PlayerNo.Player2))
        {
            result += 2;
        }
        return result;
    }

    // TODO: ??? Move to selection

    public static void SetRaycastTargets(bool value)
    {
        foreach (var shipHolder in AllUnits)
        {
            shipHolder.Value.SetRaycastTarget(value);
        }
    }

    // NEW

    public static void HighlightShipsFiltered(Func<GenericShip, bool> filter)
    {
        AllShipsHighlightOff();

        if (GetPlayer(Phases.CurrentSubPhase.RequiredPlayer) is HumanPlayer)
        {
            foreach (GenericShip ship in Roster.AllUnits.Values)
            {
                if (filter(ship))
                {
                    RosterPanelHighlightOn(ship);
                }
            }
        }
    }

    public static void AllShipsHighlightOff()
    {
        RosterAllPanelsHighlightOff();
        foreach (var ship in AllUnits)
        {
            ship.Value.HighlightCanBeSelectedOff();
        }
    }

    public static void HighlightShipOff(GenericShip ship)
    {
        ship.HighlightCanBeSelectedOff();
        RosterPanelHighlightOff(ship);
    }

    // RESERVE

    public static void MoveToReserve(GenericShip ship)
    {
        ship.SetActive(false);
        TogglePanelActive(ship, false);
        ship.SetInReserveName(true);

        AllUnits.Remove("ShipId:" + ship.ShipId);
        ship.Owner.Units.Remove("ShipId:" + ship.ShipId);

        Reserve.Add(ship);
    }

    public static void ReturnFromReserve(GenericShip ship)
    {
        ship.SetActive(true);
        TogglePanelActive(ship, true);
        ship.SetInReserveName(false);

        AllUnits.Add("ShipId:" + ship.ShipId, ship);
        ship.Owner.Units.Add("ShipId:" + ship.ShipId, ship);

        Reserve.Remove(ship);
    }

    public static void ToggleCalculatingStatus(PlayerNo playerNo, bool isActive)
    {
        Roster.GetPlayer(playerNo).PlayerInfoPanel.transform.Find("StatusPanel").gameObject.SetActive(isActive);
    }

}
