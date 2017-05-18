using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour {

    private GameManagerScript Game;

    public const float SIZE_X           = 91.44f;
    public const float SIZE_Y           = 91.44f;
    public const float SHIP_STAND_SIZE  = 4f;
    public const float DISTANCE_1       = 4f;
    public const float RANGE_1          = 10f;

    public GameObject Board;

    public GameObject RulersHolder;

    public GameObject DiceAttack;
    public GameObject DiceDefence;
    public GameObject DiceSpawningPoint;
    public GameObject DiceField;

    public GameObject StartingZone1;
    public GameObject StartingZone2;

    public void Initialize()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Game.MovementTemplates.Initialize();
        Game.Dices.Initialize();
    }

    public void HighlightStartingZones()
    {
        StartingZone1.SetActive(Game.Phases.CurrentPhasePlayer == Players.PlayerNo.Player1);
        StartingZone2.SetActive(Game.Phases.CurrentPhasePlayer == Players.PlayerNo.Player2);
    }

    public void TurnOffStartingZones()
    {
        StartingZone1.SetActive(false);
        StartingZone2.SetActive(false);
    }

    public void SetShips(Dictionary<string, Ship.GenericShip> shipsPlayer1, Dictionary<string, Ship.GenericShip> shipsPlayer2)
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

    private float CalculateDistance(int countShips)
    {
        float width = 10;
        float distance = width / (countShips + 1);
        return WorldIntoBoard(distance);
    }

    //SCALING TOOLS

    public Vector3 BoardIntoWorld(Vector3 position)
    {
        return Board.transform.TransformPoint(position);
    }

    public float BoardIntoWorld(float length)
    {
        float scale = 10 / SIZE_X;
        return length * scale;
    }

    public float WorldIntoBoard(float length)
    {
        float scale = SIZE_X / 10;
        return length * scale;
    }

}
