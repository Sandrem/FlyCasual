using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateDiceroll(DiceRoll diceRoll);

public static class Dices {

    private static GameManagerScript Game;

    private static readonly float WAIT_FOR_DICE_SECONDS = 1.5f;

    public static List<Vector3> diceResultsOffset = new List<Vector3>();

    public static GameObject DiceAttack;
    public static GameObject DiceDefence;
    public static Transform DiceSpawningPoint;
    public static Transform DiceField;

    // Use this for initialization
    static Dices() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        DiceAttack = Game.PrefabsList.DiceAttack;
        DiceDefence = Game.PrefabsList.DiceDefence;
        DiceSpawningPoint = Game.PrefabsList.DiceSpawningPoint;
        DiceField = Game.PrefabsList.DiceField;

        diceResultsOffset.Add(new Vector3(-0.2f, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(0, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(0.2f, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(-0.2f, 0, -0.2f));
        diceResultsOffset.Add(new Vector3(0, 0, -0.2f));
        diceResultsOffset.Add(new Vector3(0.2f, 0, -0.2f));
    }
	
    public static void PlanWaitForResults(DiceRoll diceRoll, DelegateDiceroll callBack)
    {
        Game.StartCoroutine(WaitForResults(diceRoll, callBack));
    }

    static IEnumerator WaitForResults(DiceRoll diceRoll, DelegateDiceroll callBack)
    {
        yield return new WaitForSeconds(WAIT_FOR_DICE_SECONDS);
        //OrganizeDicePositions(diceRoll);
        diceRoll.CalculateWaitedResults();

        callBack(diceRoll);
    }

    private static void OrganizeDicePositions(DiceRoll diceRoll)
    {
        diceRoll.OrganizeDicePositions();
    }

    public static void RerollDices(DiceRoll diceRoll, string results, DelegateDiceroll callback)
    {
        diceRoll.Reroll(results);
        Game.StartCoroutine(WaitForResults(diceRoll, callback));
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
