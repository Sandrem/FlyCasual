using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Board {

    public static Transform BoardTransform;

    public static Transform RulersHolderTransform;

    public static GameObject StartingZone1;
    public static GameObject StartingZone2;

    public static readonly float SIZE_X = 91.44f;
    public static readonly float SIZE_Y = 91.44f;
    public static readonly float SHIP_STAND_SIZE = 4f;
    public static readonly float DISTANCE_1 = 4f;
    public static readonly float RANGE_1 = 10f;

    static Board()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        BoardTransform = Game.PrefabList.BoardTransform;
        RulersHolderTransform = Game.PrefabList.RulersHolderTransform;
        StartingZone1 = Game.PrefabList.StartingZone1;
        StartingZone2 = Game.PrefabList.StartingZone2;
    }

    private static void SetShip(Ship.GenericShip ship, int count)
    {
        float distance = CalculateDistance(ship.Owner.Ships.Count);
        float side = (ship.Owner.PlayerNo == Players.PlayerNo.Player1) ? -1 : 1;
        ship.Model.SetPosition(BoardIntoWorld(new Vector3(-SIZE_X / 2 + count * distance, 0, side * SIZE_Y / 2 + +side * 2 * RANGE_1)));
        ship.Model.SetPosition(BoardIntoWorld(new Vector3(-SIZE_X / 2 + count * distance, 0, side * SIZE_Y / 2 + -side * 2 * RANGE_1)));
    }

    public static void HighlightStartingZones()
    {
        StartingZone1.SetActive(Phases.CurrentPhasePlayer == Players.PlayerNo.Player1);
        StartingZone2.SetActive(Phases.CurrentPhasePlayer == Players.PlayerNo.Player2);
    }

    public static void TurnOffStartingZones()
    {
        StartingZone1.SetActive(false);
        StartingZone2.SetActive(false);
    }

    public static void SetShips(Dictionary<string, Ship.GenericShip> shipsPlayer1, Dictionary<string, Ship.GenericShip> shipsPlayer2)
    {

        int i = 1;
        foreach (var ship in shipsPlayer1)
        {
            float distance = CalculateDistance(shipsPlayer1.Count);
            ship.Value.Model.SetPosition(BoardIntoWorld(new Vector3(- SIZE_X / 2 + i *distance, 0, -SIZE_Y/2 + 2*RANGE_1)));
            i++;
        }

        i = 1;
        foreach (var ship in shipsPlayer2)
        {
            float distance = CalculateDistance(shipsPlayer2.Count);
            ship.Value.Model.SetPosition(BoardIntoWorld(new Vector3(- SIZE_X / 2 + i * distance, 0, SIZE_Y/2 - 2*RANGE_1)));
            i++;
        }
    }

    //SCALING TOOLS

    public static Vector3 BoardIntoWorld(Vector3 position)
    {
        return BoardTransform.TransformPoint(position);
    }

    //GET TRANSFORMS

    public static Transform GetBoard()
    {
        return BoardTransform;
    }

    public static Transform GetRulersHolder()
    {
        return RulersHolderTransform;
    }

    public static Transform GetStartingZone(Players.PlayerNo playerNo)
    {
        return (playerNo == Players.PlayerNo.Player1) ? StartingZone1.transform : StartingZone2.transform;
    }

}
