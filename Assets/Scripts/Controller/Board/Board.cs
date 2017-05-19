using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Board { 

    public static GameManagerScript Game;

    public static void SetShips()
    {
        int i = 1;
        foreach (var ship in Roster.ShipsPlayer1)
        {
            SetShip(ship.Value, i);
            i++;
        }

        i = 1;
        foreach (var ship in Roster.ShipsPlayer2)
        {
            SetShip(ship.Value, i);
            i++;
        }
    }

    public static float CalculateDistance(int countShips)
    {
        float width = 10;
        float distance = width / (countShips + 1);
        return WorldIntoBoard(distance);
    }

    //SCALING TOOLS

    public static float BoardIntoWorld(float length)
    {
        float scale = 10 / SIZE_X;
        return length * scale;
    }

    public static float WorldIntoBoard(float length)
    {
        float scale = SIZE_X / 10;
        return length * scale;
    }
    
}
