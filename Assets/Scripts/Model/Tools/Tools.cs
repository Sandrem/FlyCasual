using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;
using Ship;
using System;
using SubPhases;

public static partial class Tools
{

    public static int PlayerToInt(PlayerNo playerNo)
    {
        int result = 0;
        if (playerNo == PlayerNo.Player1)
        {
            result = 1;
        }
        else
        {
            result = 2;
        }
        return result;
    }

    public static PlayerNo IntToPlayer(int playerNo)
    {
        PlayerNo result = PlayerNo.Player1;
        if (playerNo == 1)
        {
            result = PlayerNo.Player1;
        }
        else
        {
            result = PlayerNo.Player2;
        }
        return result;
    }

    public static string Canonicalize(string originalString)
    {
        string result = originalString.ToLower();

        if (result.Contains("("))
        {
            result = result.Substring(0, result.IndexOf("("));
        }

        string[] signsToReplace = new string[] { " ", "\"", "/", "'", "-" };
        foreach (var sign in signsToReplace)
        {
            result = result.Replace(sign, "");
        }

        if (Editions.Edition.Current is Editions.FirstEdition)
        {
            result = result.Replace("adv.", "adv");
            result = result.Replace("advanced", "adv");
        }

        result = result.Replace("é", "e");

        return result;
    }

    public static IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }

    public static bool IsSameTeam(GenericShip ship1, GenericShip ship2)
    {
        return ship1.Owner.PlayerNo == ship2.Owner.PlayerNo;
    }

    public static bool IsAnotherTeam(GenericShip ship1, GenericShip ship2)
    {
        return ship1.Owner.PlayerNo != ship2.Owner.PlayerNo;
    }

    public static bool IsSameShip(GenericShip ship1, GenericShip ship2)
    {
        return ship1.ShipId == ship2.ShipId;
    }

    public static bool IsAnotherFriendly(GenericShip ship1, GenericShip ship2)
    {
        return IsSameTeam(ship1, ship2) && !IsSameShip(ship1, ship2);
    }

    public static bool CheckShipsTeam(GenericShip ship1, GenericShip ship2, TargetTypes targetTypes)
    {
        switch (targetTypes)
        {
            case TargetTypes.This:
                return (IsSameShip(ship1, ship2));
            case TargetTypes.OtherFriendly:
                return (IsAnotherFriendly(ship1, ship2));
            case TargetTypes.Enemy:
                return (IsAnotherTeam(ship1, ship2));
            case TargetTypes.Any:
                return true;
            default:
                return true;
        }
    }
}
