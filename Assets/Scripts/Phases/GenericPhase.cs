using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phases
{
    None,
    Setup,
    Planning,
    Activation,
    Combat,
    End
}

public enum SubPhases
{
    None,
    Setup,
    AssignManeuvers,
    PerformManeuver,
    PerformAction,
    PerformAttack
}

public enum Sorting
{
    Asc,
    Desc
}

public class GenericPhase
{
    public Phases Phase;
    public SubPhases SubPhase;

    protected GameManagerScript Game;

    public int RequiredPilotSkill;
    public Player RequiredPlayer = Player.Player1;

    protected const int PILOTSKILL_MIN = 0;
    protected const int PILOTSKILL_MAX = 12;

    public virtual void StartPhase()
    {
        
    }

    public virtual void NextPhase()
    {

    }

    public virtual void NextSubPhase()
    {

    }

    public virtual bool ThisShipCanBeSelected(Ship.GenericShip ship)
    {
        return false;
    }

    protected virtual int GetStartingPilotSkill()
    {
        return PILOTSKILL_MIN - 1;
    }

    protected void UpdateHelpInfo()
    {
        Game.UI.Helper.UpdateHelpInfo(Phase, SubPhase, PlayerToInt(RequiredPlayer), RequiredPilotSkill);
    }

    protected void NextSubPhaseCommon(Sorting sorting)
    {
        Game.Selection.DeselectAllShips();

        Dictionary<int, int> pilots = Game.Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, PlayerToInt(RequiredPlayer), sorting);
        foreach (var pilot in pilots)
        {
            RequiredPilotSkill = pilot.Key;
            RequiredPlayer = PlayerFromInt(pilot.Value);
        }

        UpdateHelpInfo();

        if (RequiredPilotSkill == -1)
        {
            NextPhase();
        }
        else
        {
            //Debug.Log(" - " + CurrentSubPhase + " " + PilotSkillSubPhasePlayer + " Pilots:" + PilotSkillSubPhase);
        }
    }

    //TODO: move
    protected int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }

    //TODO: move
    protected Player PlayerFromInt(int playerNo)
    {
        Player result = Player.None;
        if (playerNo == 1) result = Player.Player1;
        if (playerNo == 2) result = Player.Player2;
        return result;
    }

    //TODO: move
    protected Player AnotherPlayer(Player player)
    {
        return (player == Player.Player1) ? Player.Player2 : Player.Player1;
    }

}
