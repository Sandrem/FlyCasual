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

}
