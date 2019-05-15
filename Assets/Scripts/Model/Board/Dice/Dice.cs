using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateDiceroll(DiceRoll diceRoll);

public static class DiceManager {

    public static List<List<Vector3>> DicePositions = new List<List<Vector3>>();

    public static GameObject DiceAttack;
    public static GameObject DiceDefence;

    public static EventHandler OnDiceRolled;
    public static EventHandler OnDiceResult;

    // Use this for initialization
    static DiceManager()
    {
        DiceAttack = (GameObject)Resources.Load("Prefabs/Dice/DiceAttack", typeof(GameObject));
        DiceDefence = (GameObject)Resources.Load("Prefabs/Dice/DiceDefence", typeof(GameObject));

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
            new Vector3( 0.08f, 0,  0.14f),
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

        // Add 10+ dice ???

    }

    public static void CallDiceRolled(object sender, EventArgs e)
    {
        OnDiceRolled?.Invoke(sender, e);
    }

    internal static void CallDiceResult(object sender, EventArgs e)
    {
        OnDiceResult?.Invoke(sender, e);
    }
}
