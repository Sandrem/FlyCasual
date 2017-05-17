using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;

public partial class ShipRoster
{
    //Players

    public List<GenericPlayer> Players = new List<GenericPlayer>();

    private GenericPlayer player1;
    public GenericPlayer Player1 { get { return Players[0]; } }

    private GenericPlayer player2;
    public GenericPlayer Player2 { get { return Players[1]; } }

    //Ships

    public Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();

    private Dictionary<string, Ship.GenericShip> shipsPlayer1;
    public Dictionary<string, Ship.GenericShip> ShipsPlayer1 { get { return Players[0].Ships; } }

    private Dictionary<string, Ship.GenericShip> shipsPlayer2;
    public Dictionary<string, Ship.GenericShip> ShipsPlayer2 {get { return Players[1].Ships; } }

    private GameManagerScript Game;

    // Use this for initialization
    public ShipRoster()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //TODO: ???
        //Game.Phases.OnCombatPhaseStart += HideAssignedDials;
    }

    public void Start()
    {
        CreatePlayers();
        SpawnAllShips();
    }

    //PLAYERS CREATION

    private void CreatePlayers()
    {
        foreach (var playerType in Global.GetPlayerTypes())
        {
            CreatePlayer(playerType);
        }
    }

    private void CreatePlayer(System.Type type)
    {
        System.Activator.CreateInstance(type);
    }

    //SHIP CREATION

    private void SpawnAllShips()
    {
        foreach (var shipConfig in Global.GetShipConfigurations())
        {
            Ship.GenericShip newShip = Game.ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }
    }

    private void AddShipToLists(Ship.GenericShip newShip)
    {
        AllShips.Add(newShip.Model.GetTag(), newShip);
        newShip.Owner.Ships.Add(newShip.Model.GetTag(), newShip);
    }

    //SHIP DESTRUCTION

    public void DestroyShip(string id)
    {
        GetShipById(id).Model.SetActive(false);
        GetShipById(id).InfoPanel.SetActive(false);

        RemoveShipFromLists(id);
    }

    private void RemoveShipFromLists(string id)
    {
        GetShipById(id).Owner.Ships.Remove(id);
        AllShips.Remove(id);
    }

    //TOOLS

    public Ship.GenericShip GetShipById(string id)
    {
        return AllShips[id];
    }

    public GenericPlayer GetPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? Game.Roster.Player1 : Game.Roster.Player2;
    }

    public int AnotherPlayer(int player)
    {
        return (player == 1) ? 2 : 1;
    }

    public PlayerNo AnotherPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? PlayerNo.Player2 : PlayerNo.Player1;
    }


    //FIND SHIPS BY REQUEST

    public Dictionary<int, PlayerNo> NextPilotSkillAndPlayerAfter(int previousPilotSkill, Players.PlayerNo PilotSkillSubPhasePlayer, Sorting sorting)
    {

        //TODO: Check for same skill with another player
        //pilots = ListAnotherPlayerButSamePilotSkill(previousPilotSkill, PilotSkillSubPhasePlayer);

        int pilotSkillMin = (sorting == Sorting.Asc) ? previousPilotSkill : -1;
        int pilotSkillMax = (sorting == Sorting.Asc) ? 100 : previousPilotSkill;

        var results =
            from n in AllShips
            where n.Value.PilotSkill > pilotSkillMin
            where n.Value.PilotSkill < pilotSkillMax
            orderby n.Value.PilotSkill
            select n;

        Dictionary<int, PlayerNo> dict = new Dictionary<int, PlayerNo>();
        if (results.Count() > 0)
        {
            var result = (sorting == Sorting.Asc) ? results.First() : results.Last();
            dict.Add(result.Value.PilotSkill, result.Value.Owner.PlayerNo);
        }

        return dict;
    }

    public Dictionary<string, Ship.GenericShip> ListSamePlayerAndPilotSkill(Ship.GenericShip thisShip)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo    == thisShip.Owner.PlayerNo
            where n.Value.PilotSkill        == thisShip.PilotSkill
            where n.Value.ShipId            == thisShip.ShipId
            select n;

        return results.ToDictionary(t => t.Key, t => t.Value);
    }

    public Dictionary<int, int> ListAnotherPlayerButSamePilotSkill(int previousPilotSkill, int PilotSkillSubPhasePlayer)
    {
        var results =
            from n in AllShips
            where n.Value.PilotSkill    == previousPilotSkill
            where n.Value.Owner.Id      != PilotSkillSubPhasePlayer
            select n;

        return results.ToDictionary(t => previousPilotSkill, t => t.Value.Owner.Id);
    }

    // CHECK ALL SHIPS IN ROSTER

    public bool AllManuersAreAssigned(PlayerNo playerNo)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.AssignedManeuver == null
            select n;

        if (results.Count() > 0) Game.UI.ShowError("Not all ship are assigned their maneuvers");
        return (results.Count() == 0);
    }

    public bool AllManueversArePerformed()
    {
        var results =
            from n in AllShips
            where n.Value.IsManeurPerformed == false
            select n;

        if (results.Count() > 0) Game.UI.ShowError("Not all ship executed their maneuvers");
        return (results.Count() == 0);
    }

    public bool NoSamePlayerAndPilotSkillNotMoved(Ship.GenericShip thisShip)
    {
        var results =
            from n in ListSamePlayerAndPilotSkill(thisShip)
            where n.Value.IsManeurPerformed == false
            select n;

        return (results.Count() == 0);
    }

    public bool NoSamePlayerAndPilotSkillNotAttacked(Ship.GenericShip thisShip)
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

    public int CheckIsAnyTeamIsEliminated()
    {
        int result = 0;
        if (ShipsPlayer1.Count == 0)
        {
            return 1;
        }
        if (ShipsPlayer2.Count == 0)
        {
            return 2;
        }
        return result;
    }

    // TODO: ??? Move to selection

    public void SetRaycastTargets(bool value)
    {
        foreach (var shipHolder in AllShips)
        {
            shipHolder.Value.Model.SetRaycastTarget(value);
        }
    }

}
