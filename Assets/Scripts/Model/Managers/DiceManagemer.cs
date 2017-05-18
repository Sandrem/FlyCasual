using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceSide
{
    Unknown,
    Blank,
    Focus,
    Success,
    Crit
}

public class DiceManager {

    private GameManagerScript Game;

    private const float WAIT_FOR_DICE_SECONDS = 1.5f;

    public List<Vector3> diceResultsOffset = new List<Vector3>();

    // Use this for initialization
    public void Initialize() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        diceResultsOffset.Add(new Vector3(-0.2f, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(0, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(0.2f, 0, 0.2f));
        diceResultsOffset.Add(new Vector3(-0.2f, 0, -0.2f));
        diceResultsOffset.Add(new Vector3(0, 0, -0.2f));
        diceResultsOffset.Add(new Vector3(0.2f, 0, -0.2f));
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void PlanWaitForResults(DiceRoll diceRoll)
    {
        Game.StartCoroutine(WaitForResults(diceRoll));
    }

    IEnumerator WaitForResults(DiceRoll diceRoll)
    {
        yield return new WaitForSeconds(WAIT_FOR_DICE_SECONDS);
        //OrganizeDicePositions(diceRoll);
        diceRoll.CalculateWaitedResults();
        if (Game.Combat.AttackStep == CombatStep.Attack) Game.Combat.DiceRollAttack = diceRoll;
        if (Game.Combat.AttackStep == CombatStep.Defence) Game.Combat.DiceRollDefence = diceRoll;
        Game.UI.DiceResults.ShowDiceModificationButtons();
    }

    private void OrganizeDicePositions(DiceRoll diceRoll)
    {
        diceRoll.OrganizeDicePositions();
    }

    public void RerollDices(DiceRoll diceRoll, string results)
    {
        diceRoll.Reroll(results);
        Game.StartCoroutine(WaitForResults(diceRoll));
    }

    public void ApplyFocus(DiceRoll diceRoll)
    {
        diceRoll.ApplyFocus();
        OrganizeDicePositions(diceRoll);
    }

    public void ApplyEvade(DiceRoll diceRoll)
    {
        diceRoll.ApplyEvade();
        OrganizeDicePositions(diceRoll);
    }
}
