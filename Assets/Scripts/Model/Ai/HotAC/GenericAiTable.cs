using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAiTable
{

    protected List<string> FrontManeuversInner = new List<string>();
    protected List<string> FrontManeuversOuter = new List<string>();
    protected List<string> FrontSideManeuversInner = new List<string>();
    protected List<string> FrontSideManeuversOuter = new List<string>();
    protected List<string> SideManeuversInner = new List<string>();
    protected List<string> SideManeuversOuter = new List<string>();
    protected List<string> BackSideManeuversInner = new List<string>();
    protected List<string> BackSideManeuversOuter = new List<string>();
    protected List<string> BackManeuversInner = new List<string>();
    protected List<string> BackManeuversOuter = new List<string>();

    public GenericAiTable()
    {

    }

    public Movement GetManeuver(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Movement result = null;
        float vector = Actions.GetVector(thisShip, anotherShip);
        bool isClosing = Actions.IsClosing(thisShip, anotherShip);
        result = GetManeuverFromTable(vector, isClosing);
        return result;
    }

    public Movement GetManeuverFromTable(float vector, bool isClosing)
    {
        Movement result = null;
        if (isClosing)
        {
            if ((vector > -22.5f) && (vector < 22.5f)) return RandomManeuverFromTable(FrontManeuversInner);
            if (((vector >= 22.5f) && (vector < 67.5f)) || ((vector <= -22.5f) && (vector > -67.5f))) return AdjustDirection(RandomManeuverFromTable(FrontSideManeuversInner), vector);
            if (((vector >= 67.5f) && (vector < 112.5f)) || ((vector <= -67.5f) && (vector > -112.5f))) return AdjustDirection(RandomManeuverFromTable(SideManeuversInner), vector);
            if (((vector >= 112.5f) && (vector < 157.5f)) || ((vector <= -112.5f) && (vector > -157.5f))) return AdjustDirection(RandomManeuverFromTable(BackSideManeuversInner), vector);
            if ((vector >= 157.5f) || (vector <= -157.5f)) return RandomManeuverFromTable(BackManeuversInner);
        }
        else
        {
            if ((vector > -22.5f) && (vector < 22.5f)) return RandomManeuverFromTable(FrontManeuversOuter);
            if (((vector >= 22.5f) && (vector < 67.5f)) || ((vector <= -22.5f) && (vector > -67.5f))) return AdjustDirection(RandomManeuverFromTable(FrontSideManeuversOuter), vector);
            if (((vector >= 67.5f) && (vector < 112.5f)) || ((vector <= -67.5f) && (vector > -112.5f))) return AdjustDirection(RandomManeuverFromTable(SideManeuversOuter), vector);
            if (((vector >= 112.5f) && (vector < 157.5f)) || ((vector <= -112.5f) && (vector > -157.5f))) return AdjustDirection(RandomManeuverFromTable(BackSideManeuversOuter), vector);
            if ((vector >= 157.5f) || (vector <= -157.5f)) return RandomManeuverFromTable(BackManeuversOuter);
        }
        return result;
    }

    private Movement AdjustDirection(Movement movement, float vector)
    {
        Movement result = movement;
        if (movement.Direction != ManeuverDirection.Forward)
        {
            movement.Direction = (vector < 0) ? ManeuverDirection.Left : ManeuverDirection.Right;
        }
        return result;
    }

    public Movement RandomManeuverFromTable(List<string> table)
    {
        string result = "";
        int random = Random.Range(1, 6);
        result = table[random];

        //Temporary
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //Debug.Log(result);

        return Game.Movement.ManeuverFromString(result);
    }

    public static bool IsClosing(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        bool result = false;
        float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralFrontPoint());
        float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralBackPoint());
        result = (distanceToFront < distanceToBack) ? true : false;
        return result;
    }

}
