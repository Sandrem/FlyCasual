using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    None,
    Player1,
    Player2
}

public class ShipRosterManagerScript : MonoBehaviour {

    private GameManagerScript Game;

    public Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();

    private List<string> team1 = new List<string>();
    private List<string> team2 = new List<string>();

    public ShipFactoryScript ShipFactory;

    // Use this for initialization
    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //ToDo: AutoID, Teams as Enums
    public void SpawnAllShips()
    {
        Ship.GenericShip newShip;
        ShipFactoryScript ShipFactory = this.GetComponent<ShipFactoryScript>();

        newShip = ShipFactory.SpawnShip("Ship.XWing.LukeSkywalker", Player.Player1, 1, new Vector3(0, 0, -2.5f));
        newShip.InstallUpgrade("Upgrade.R2D2");
        newShip.InstallUpgrade("Upgrade.Marksmanship");
        AddShip(newShip, 1);

        newShip = ShipFactory.SpawnShip("Ship.TIEFighter.MaulerMithel", Player.Player2, 2, new Vector3(-1, 0, 2.5f));
        newShip.InstallUpgrade("Upgrade.Determination");
        AddShip(newShip, 2);
        newShip = ShipFactory.SpawnShip("Ship.TIEFighter.DarkCurse", Player.Player2, 3, new Vector3(1, 0, 2.5f));
        AddShip(newShip, 2);
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
        if (GetShipByTag(tag).PlayerNo == Player.Player1)
        {
            if (team1.Contains(tag)) team1.Remove(tag);    
        }
        if (GetShipByTag(tag).PlayerNo == Player.Player2)
        {
            if (team1.Contains(tag)) team2.Remove(tag);
        }
        AllShips.Remove(tag);
    }

    //TODO: Rewrite player skill checks (all 3 functions)

    public Dictionary<int, int> NextPilotSkillAndPlayerAfter(int previousPilotSkill, int PilotSkillSubPhasePlayer, Sorting sorting)
    {

        Dictionary<int, int> pilots = new Dictionary<int, int>();

        //Check for same skill with another player
        pilots = ListAnotherPlayerButSamePilotSkill(previousPilotSkill, PilotSkillSubPhasePlayer);

        //Check for another pilot skill
        int nextPilotSkill = -1;
        int playerNo = -1;

        //rewrite next two blocks?
        if (sorting == Sorting.Asc)
        {
            nextPilotSkill = 100;
            foreach (var ship in AllShips)
            {
                if ((ship.Value.PilotSkill > previousPilotSkill) && (ship.Value.PilotSkill < nextPilotSkill))
                {
                    nextPilotSkill = ship.Value.PilotSkill;
                    playerNo = PlayerToInt(ship.Value.PlayerNo);
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
                    playerNo = PlayerToInt(ship.Value.PlayerNo);
                }
            }
        }

        pilots.Add(nextPilotSkill, playerNo);
        return pilots;
    }

    public bool AllManuersAreAssigned(int playerNo)
    {
        foreach (var item in AllShips)
        {
            if (item.Value.PlayerNo == PlayerFromInt(playerNo))
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
            if (item.Value.PlayerNo == thisShip.PlayerNo)
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
                    if (ship.Value.PlayerNo != PlayerFromInt(PilotSkillSubPhasePlayer))
                    {
                        result.Add(previousPilotSkill, PlayerToInt(ship.Value.PlayerNo));
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

    //TODO: move
    public Player PlayerFromInt(int playerNo)
    {
        Player result = Player.None;
        if (playerNo == 1) result = Player.Player1;
        if (playerNo == 2) result = Player.Player2;
        return result;
    }

    //TODO: move
    public int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }

}
