using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateDiceroll(DiceRoll diceRoll);

public static class DicesManager {

    private static GameManagerScript Game;

    private static readonly float WAIT_FOR_DICE_SECONDS = 1.5f;

    public static List<List<Vector3>> DicePositions = new List<List<Vector3>>();

    public static GameObject DiceAttack;
    public static GameObject DiceDefence;
    public static Transform DiceSpawningPoint;
    public static Transform DiceField;

    // Use this for initialization
    static DicesManager() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        DiceAttack = Game.PrefabsList.DiceAttack;
        DiceDefence = Game.PrefabsList.DiceDefence;
        DiceSpawningPoint = Game.PrefabsList.DiceSpawningPoint;
        DiceField = Game.PrefabsList.DiceField;

        GenerateDicePositions();
    }

    private static void GenerateDicePositions()
    {
        List<Vector3> DicePositions1 = new List<Vector3>
        {
            Vector3.zero
        };
        DicePositions.Add(DicePositions1);

        List<Vector3> DicePositions2 = new List<Vector3>
        {
            new Vector3(-0.1f, 0, 0),
            new Vector3( 0.1f, 0, 0)
        };
        DicePositions.Add(DicePositions2);

        List<Vector3> DicePositions3 = new List<Vector3>
        {
            new Vector3(-0.1f, 0,  0.06f),
            new Vector3( 0.1f, 0,  0.06f),
            new Vector3(   0f, 0, -0.1f)
        };
        DicePositions.Add(DicePositions3);

        List<Vector3> DicePositions4 = new List<Vector3>
        {
            new Vector3(-0.1f, 0, -0.1f),
            new Vector3( 0.1f, 0, -0.1f),
            new Vector3(-0.1f, 0,  0.1f),
            new Vector3( 0.1f, 0,  0.1f)
        };
        DicePositions.Add(DicePositions4);

        List<Vector3> DicePositions5 = new List<Vector3>
        {
            new Vector3(    0f, 0,  0.16f),
            new Vector3(-0.16f, 0,  0.04f),
            new Vector3( 0.16f, 0,  0.04f),
            new Vector3(-0.08f, 0, -0.14f),
            new Vector3( 0.08f, 0, -0.14f)
        };
        DicePositions.Add(DicePositions5);

        List<Vector3> DicePositions6 = new List<Vector3>
        {
            new Vector3(-0.08f, 0,  0.14f),
            new Vector3( 0.08f, 0,  0.04f),
            new Vector3(-0.16f, 0,     0f),
            new Vector3( 0.16f, 0,     0f),
            new Vector3(-0.08f, 0, -0.14f),
            new Vector3( 0.08f, 0, -0.14f)
        };
        DicePositions.Add(DicePositions6);

        List<Vector3> DicePositions7 = new List<Vector3>
        {
            new Vector3(-0.08f, 0,  0.14f),
            new Vector3( 0.08f, 0,  0.04f),
            new Vector3(-0.16f, 0,     0f),
            Vector3.zero,
            new Vector3( 0.16f, 0,     0f),
            new Vector3(-0.08f, 0, -0.14f),
            new Vector3( 0.08f, 0, -0.14f)
        };
        DicePositions.Add(DicePositions7);

        List<Vector3> DicePositions8 = new List<Vector3>
        {
            new Vector3(-0.08f, 0,   0.2f),
            new Vector3( 0.08f, 0,   0.2f),
            new Vector3( -0.2f, 0,  0.08f),
            new Vector3(  0.2f, 0,  0.08f),
            new Vector3( -0.2f, 0, -0.08f),
            new Vector3(  0.2f, 0, -0.08f),
            new Vector3(-0.08f, 0,  -0.2f),
            new Vector3( 0.08f, 0,  -0.2f)
        };
        DicePositions.Add(DicePositions8);

        List<Vector3> DicePositions9 = new List<Vector3>
        {
            new Vector3(-0.08f, 0,   0.2f),
            new Vector3( 0.08f, 0,   0.2f),
            new Vector3( -0.2f, 0,  0.08f),
            new Vector3(  0.2f, 0,  0.08f),
            Vector3.zero,
            new Vector3( -0.2f, 0, -0.08f),
            new Vector3(  0.2f, 0, -0.08f),
            new Vector3(-0.08f, 0,  -0.2f),
            new Vector3( 0.08f, 0,  -0.2f)
        };
        DicePositions.Add(DicePositions9);

        // Add 10+ dices ???

    }
	
    private static void OrganizeDicePositions(DiceRoll diceRoll)
    {
        diceRoll.OrganizeDicePositions();
    }

    public static void RerollDices(DiceRoll diceRoll, string results, DelegateDiceroll callback)
    {
        diceRoll.Reroll(results);
        diceRoll.CalculateResults(callback);
    }

    public static void RerollOne(DiceRoll diceRoll, DelegateDiceroll callback)
    {
        diceRoll.RerollOne();
        diceRoll.CalculateResults(callback);
    }

    public static void ApplyFocus(DiceRoll diceRoll)
    {
        diceRoll.ApplyFocus();
        OrganizeDicePositions(diceRoll);
    }

    public static void ApplyEvade(DiceRoll diceRoll)
    {
        diceRoll.ApplyEvade();
        OrganizeDicePositions(diceRoll);
    }
}
