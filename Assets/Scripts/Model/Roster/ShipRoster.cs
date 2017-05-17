using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public partial class ShipRoster
{

    public List<GenericPlayer> Players = new List<GenericPlayer>();

    public Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();

    private GenericPlayer player1;
    public GenericPlayer Player1 { get { return Players[0]; } }

    private Dictionary<string, Ship.GenericShip> shipsPlayer1;
    public Dictionary<string, Ship.GenericShip> ShipsPlayer1 { get { return Players[0].Ships; } }

    private GenericPlayer player2;
    public GenericPlayer Player2 { get { return Players[1]; } }

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
        //Todo: move to constructor
        CreatePlayers();
        SpawnAllShips();
    }

    private void CreatePlayers()
    {
        AddPlayer(PlayerType.Human);
        AddPlayer(PlayerType.Human);
    }

    //ToDo: AutoID, Teams as Enums
    private void SpawnAllShips()
    {
        foreach (var shipConfig in Global.GetShipConfigurations())
        {
            Ship.GenericShip newShip = Game.ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }
    }

    private void AddPlayer(PlayerType type)
    {
        //Todo: Generate by string-name

        switch (type)
        {
            case PlayerType.Human:
                new HumanPlayer();
                break;
            case PlayerType.Ai:
                new AiPlayer();
                break;
            default:
                break;
        }

        
    }

    public Ship.GenericShip GetShipById(string id)
    {
        return AllShips[id];
    }

    private void AddShipToLists(Ship.GenericShip newShip)
    {
        AllShips.Add(newShip.Model.GetTag(), newShip);
        newShip.Owner.Ships.Add(newShip.Model.GetTag(), newShip);
    }

    public Ship.GenericShip GetShipByTag(string tag)
    {
        return AllShips[tag];
    }

    public void DestroyShip(string tag)
    {
        GetShipByTag(tag).Model.SetActive(false);
        GetShipByTag(tag).InfoPanel.SetActive(false);

        GetShipByTag(tag).Owner.Ships.Remove(tag);
        AllShips.Remove(tag);
    }

    //TODO: Rewrite player skill checks (all 3 functions)

    public Dictionary<int, Players.PlayerNo> NextPilotSkillAndPlayerAfter(int previousPilotSkill, Players.PlayerNo PilotSkillSubPhasePlayer, Sorting sorting)
    {

        Dictionary<int, Players.PlayerNo> pilots = new Dictionary<int, Players.PlayerNo>();

        //Check for same skill with another player
        //pilots = ListAnotherPlayerButSamePilotSkill(previousPilotSkill, PilotSkillSubPhasePlayer);

        //Check for another pilot skill
        int nextPilotSkill = -1;
        PlayerNo playerNo = PilotSkillSubPhasePlayer;

        //rewrite next two blocks?
        if (sorting == Sorting.Asc)
        {
            nextPilotSkill = 100;
            foreach (var ship in AllShips)
            {
                if ((ship.Value.PilotSkill > previousPilotSkill) && (ship.Value.PilotSkill < nextPilotSkill))
                {
                    nextPilotSkill = ship.Value.PilotSkill;
                    playerNo = ship.Value.Owner.PlayerNo;
                }
            }
            if (nextPilotSkill == 100)
            {
                nextPilotSkill = -1;
            }

        }

        if (sorting == Sorting.Desc)
        {
            nextPilotSkill = -1;
            foreach (var ship in AllShips)
            {
                if ((ship.Value.PilotSkill < previousPilotSkill) && (ship.Value.PilotSkill > nextPilotSkill))
                {
                    nextPilotSkill = ship.Value.PilotSkill;
                    playerNo = ship.Value.Owner.PlayerNo;
                }
            }
        }
        pilots.Add(nextPilotSkill, playerNo);
        return pilots;
    }

    public bool AllManuersAreAssigned(Players.PlayerNo playerNo)
    {
        foreach (var item in AllShips)
        {
            if (item.Value.Owner.PlayerNo == playerNo)
            {
                if (item.Value.AssignedManeuver == null)
                {
                    Game.UI.ShowError("Not all ship are assigned their maneuvers");
                    return false;
                }
            }
        }
        return true;
    }

    public bool AllManueversArePerformed()
    {
        foreach (var item in AllShips)
        {
            if (item.Value.IsManeurPerformed == false)
            {
                Game.UI.ShowError("Not all ship executed their maneuvers");
                return false;
            }
        }
        return true;
    }

    public void SetRaycastTargets(bool value)
    {
        foreach (var shipHolder in AllShips)
        {
            shipHolder.Value.Model.SetRaycastTarget(value);
        }
    }

    public Dictionary<string, Ship.GenericShip> ListSamePlayerAndPilotSkill(Ship.GenericShip thisShip)
    {
        Dictionary<string, Ship.GenericShip> result = new Dictionary<string, Ship.GenericShip>();
        foreach (var item in AllShips)
        {
            if (item.Value.Owner == thisShip.Owner)
            {
                if (item.Value.PilotSkill == thisShip.PilotSkill)
                {
                    if (item.Value.ShipId != thisShip.ShipId)
                    {
                        result.Add(item.Key, item.Value);
                    }
                }
            }
        }
        return result;
    }

    public Dictionary<int, int> ListAnotherPlayerButSamePilotSkill(int previousPilotSkill, int PilotSkillSubPhasePlayer)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();

        //fix this
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        if (PlayerFromInt(PilotSkillSubPhasePlayer) == Game.Phases.PlayerWithInitiative)
        {
            foreach (var ship in AllShips)
            {
                if (ship.Value.PilotSkill == previousPilotSkill)
                {
                    if (ship.Value.Owner.PlayerNo != PlayerFromInt(PilotSkillSubPhasePlayer))
                    {
                        result.Add(previousPilotSkill, PlayerToInt(ship.Value.Owner.PlayerNo));
                        return result;
                    }
                }
            }
        }
        return result;
    }

    public bool NoSamePlayerAndPilotSkillNotMoved(Ship.GenericShip thisShip)
    {
        Dictionary<string, Ship.GenericShip> samePlayerAndPilotSkill = ListSamePlayerAndPilotSkill(thisShip);
        foreach (var item in samePlayerAndPilotSkill)
        {
            if (item.Value.IsManeurPerformed == false)
            {
                return false;
            }
        }
        return true;
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

    public Players.GenericPlayer GetPlayer(PlayerNo playerNo)
    {
        //fix this
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        return (playerNo == PlayerNo.Player1) ? Game.Roster.Player1 : Game.Roster.Player2;
    }

    /*public Players.GenericPlayer GetPlayer(Players.GenericPlayer playerNo)
    {
        //fix this
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        return (playerNo == Player.Player1) ? Game.Roster.Player1 : Game.Roster.Player2;
    }*/

    //TODO: move
    public PlayerNo PlayerFromInt(int playerNo)
    {
        PlayerNo result = PlayerNo.Player1;
        if (playerNo == 1) result = PlayerNo.Player1;
        if (playerNo == 2) result = PlayerNo.Player2;
        return result;
    }

    //TODO: move
    public int PlayerToInt(PlayerNo playerNo)
    {
        int result = -1;
        if (playerNo == PlayerNo.Player1) result = 1;
        if (playerNo == PlayerNo.Player2) result = 2;
        return result;
    }

    //TODO: move
    public int AnotherPlayer(int player)
    {
        return (player == 1) ? 2 : 1;
    }

    //TODO: move
    public PlayerNo AnotherPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? PlayerNo.Player2 : PlayerNo.Player1;
    }

}
