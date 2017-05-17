using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;
using System;

public partial class ShipRoster {

    private GameManagerScript Game;

    // Use this for initialization
    public ShipRoster()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //TODO: ???
        //Game.PhaseManager.OnCombatPhaseStart += HideAssignedDials;
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

        Ship.GenericShip newShip;
        ShipFactory ShipFactory = new ShipFactory();

        //Temporary
        Game.ShipFactory = ShipFactory;

        newShip = ShipFactory.SpawnShip("Ship.XWing.LukeSkywalker", Player1);
        newShip.InstallUpgrade("Upgrade.R2D2");
        newShip.InstallUpgrade("Upgrade.Marksmanship");
        AddShip(newShip, 1);

        newShip = ShipFactory.SpawnShip("Ship.TIEFighter.MaulerMithel", Player2);
        newShip.InstallUpgrade("Upgrade.Determination");
        AddShip(newShip, 2);
        //TODO: Error: Pilots with same skill
        //newShip = ShipFactory.SpawnShip("Ship.TIEFighter.NightBeast", Player2, 4, new Vector3(1, 0, 2.5f));
        //AddShip(newShip, 2);*/
    }

    private void AddPlayer(PlayerType type)
    {
        //Todo: Generate by string-name

        GenericPlayer player = new GenericPlayer(0);

        switch (type)
        {
            case PlayerType.Human:
                player = new HumanPlayer(Players.Count + 1);
                break;
            case PlayerType.Ai:
                player = new AiPlayer(Players.Count + 1);
                break;
            default:
                break;
        }

        Players.Add(player);
    }

    public Ship.GenericShip GetShipById(string id)
    {
        return AllShips[id];
    }

    private void AddShip(Ship.GenericShip newShip, int playerNo)
    {
        AllShips.Add(newShip.Model.GetTag(), newShip);
        if (playerNo == 1)
        {
            team1.Add(newShip.Model.GetTag());
        }
        else if (playerNo == 2)
        {
            team2.Add(newShip.Model.GetTag());
        }
    }

    public Ship.GenericShip GetShipByTag(string tag)
    {
        return AllShips[tag];
    }

    public void DestroyShip(string tag)
    {
        GetShipByTag(tag).Model.SetActive(false);
        GetShipByTag(tag).InfoPanel.SetActive(false);

        //todo: rework
        if (GetShipByTag(tag).Owner.PlayerNo == PlayerNo.Player1)
        {
            if (team1.Contains(tag)) team1.Remove(tag);    
        }
        if (GetShipByTag(tag).Owner.PlayerNo == PlayerNo.Player2)
        {
            if (team1.Contains(tag)) team2.Remove(tag);
        }
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

        if (PlayerFromInt(PilotSkillSubPhasePlayer) == Game.PhaseManager.PlayerWithInitiative)
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
        if (team1.Count == 0)
        {
            return 1;
        }
        if (team2.Count == 0)
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

    public List<string> GetTeam(int playerNo)
    {
        return (playerNo == 1) ? Game.Roster.team1 : Game.Roster.team2;
    }

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

}
