using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;

public static partial class Roster
{
    //Players

    public static List<GenericPlayer> Players = new List<GenericPlayer>();

    private static GenericPlayer player1;
    public static GenericPlayer Player1 { get { return Players[0]; } }

    private static GenericPlayer player2;
    public static GenericPlayer Player2 { get { return Players[1]; } }

    //Ships

    public static Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();

    private static Dictionary<string, Ship.GenericShip> shipsPlayer1;
    public static Dictionary<string, Ship.GenericShip> ShipsPlayer1 { get { return Players[0].Ships; } }

    private static Dictionary<string, Ship.GenericShip> shipsPlayer2;
    public static Dictionary<string, Ship.GenericShip> ShipsPlayer2 {get { return Players[1].Ships; } }

    private static GameManagerScript Game;

    public static void Start()
    {
        CreatePlayers();
        SpawnAllShips();
    }

    // Use this for initialization
    static Roster()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //TODO: ???
        //Game.Phases.OnCombatPhaseStart += HideAssignedDials;
    }

    //PLAYERS CREATION

    private static void CreatePlayers()
    {
        foreach (var playerType in Global.PlayerTypes)
        {
            CreatePlayer(playerType);
        }
    }

    private static void CreatePlayer(System.Type type)
    {
        System.Activator.CreateInstance(type);
    }

    //SHIP CREATION

    private static void SpawnAllShips()
    {
        foreach (var shipConfig in Global.ShipConfigurations)
        {
            Ship.GenericShip newShip = ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }
        Board.BoardManager.SetShips();
    }

    private static void AddShipToLists(Ship.GenericShip newShip)
    {
        AllShips.Add(newShip.GetTag(), newShip);
        newShip.Owner.Ships.Add(newShip.GetTag(), newShip);
    }

    //SHIP DESTRUCTION

    public static void DestroyShip(string id)
    {
        GetShipById(id).SetActive(false);
        GetShipById(id).InfoPanel.SetActive(false);
        OrganizeRosterPositions();

        RemoveShipFromLists(id);
    }

    private static void RemoveShipFromLists(string id)
    {
        GetShipById(id).Owner.Ships.Remove(id);
        AllShips.Remove(id);
    }

    //TOOLS

    public static Ship.GenericShip GetShipById(string id)
    {
        return AllShips[id];
    }

    public static GenericPlayer GetPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? Roster.Player1 : Roster.Player2;
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

    public static Dictionary<string, Ship.GenericShip> ListSamePlayerAndPilotSkill(Ship.GenericShip thisShip)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == thisShip.Owner.PlayerNo
            where n.Value.PilotSkill == thisShip.PilotSkill
            where n.Value.ShipId == thisShip.ShipId
            select n;

        return results.ToDictionary(t => t.Key, t => t.Value);
    }

    public static Dictionary<int, int> ListAnotherPlayerButSamePilotSkill(int previousPilotSkill, int PilotSkillSubPhasePlayer)
    {
        var results =
            from n in AllShips
            where n.Value.PilotSkill == previousPilotSkill
            where n.Value.Owner.Id != PilotSkillSubPhasePlayer
            select n;

        return results.ToDictionary(t => previousPilotSkill, t => t.Value.Owner.Id);
    }

    // CHECK ALL SHIPS IN ROSTER

    public static bool AllManuersAreAssigned(PlayerNo playerNo)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.AssignedManeuver == null
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

        if (results.Count() > 0) Game.UI.ShowError("Not all ship executed their maneuvers");
        return (results.Count() == 0);
    }

    public static bool NoSamePlayerAndPilotSkillNotMoved(Ship.GenericShip thisShip)
    {
        var results =
            from n in ListSamePlayerAndPilotSkill(thisShip)
            where n.Value.IsManeuverPerformed == false
            select n;

        return (results.Count() == 0);
    }

    public static bool NoSamePlayerAndPilotSkillNotAttacked(Ship.GenericShip thisShip)
    {
        Dictionary<string, Ship.GenericShip> samePlayerAndPilotSkill = ListSamePlayerAndPilotSkill(thisShip);
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
        if (ShipsPlayer1.Count == 0)
        {
            result += 1;
        }
        if (ShipsPlayer2.Count == 0)
        {
            result += 2;
        }
        return result;
    }

    // TODO: ??? Move to selection

    public static void SetRaycastTargets(bool value)
    {
        foreach (var shipHolder in AllShips)
        {
            shipHolder.Value.SetRaycastTarget(value);
        }
    }

    // NEW

    public static void HighlightShipsFiltered(PlayerNo playerNo, int pilotSkill = -1)
    {
        AllShipsHighlightOff();
        foreach (var ship in GetPlayer(playerNo).Ships)
        {
            if ((pilotSkill == -1) || (ship.Value.PilotSkill == pilotSkill))
            {
                ship.Value.HighlightCanBeSelectedOn();
                RosterPanelHighlightOn(ship.Value);
            }
        }
    }

    public static void AllShipsHighlightOff()
    {
        RosterAllPanelsHighlightOff();
        foreach (var ship in AllShips)
        {
            ship.Value.HighlightCanBeSelectedOff();
        }
    }

    public static void HighlightShipOff(Ship.GenericShip ship)
    {
        ship.HighlightCanBeSelectedOff();
        RosterPanelHighlightOff(ship);
    }

}
