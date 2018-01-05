using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

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

        string[] signsToReplace = new string[] { " ", "\"", "/", "'", "-" };
        foreach (var sign in signsToReplace)
        {
            result = result.Replace(sign, "");
        }

        result = result.Replace("adv.", "adv");
        result = result.Replace("advanced", "adv");

        return result;
    }

}
