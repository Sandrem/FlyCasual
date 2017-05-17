using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatStep
{
    None,
    Attack,
    Defence
}

public class CombatManager {

    private GameManagerScript Game;

    public DiceRoll DiceRollAttack;
    public DiceRoll DiceRollDefence;
    public DiceRoll CurentDiceRoll;

    public CombatStep AttackStep = CombatStep.None;

    public Ship.GenericShip Attacker;
    public Ship.GenericShip Defender;

    // Use this for initialization
    public void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
    public void PerformAttack(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        Attacker = attacker;
        Defender = defender;

        attacker.IsAttackPerformed = true;
        AttackStep = CombatStep.Attack;

        AttackStart();

        Game.Selection.ActiveShip = attacker;
        int currentFirepower = attacker.GetNumberOfAttackDices(defender);

        Game.UI.DiceResults.ShowDiceResultMenu();
        DiceRoll DiceRollAttack = new DiceRoll("attack", currentFirepower);
        DiceRollAttack.Roll();
        CurentDiceRoll = DiceRollAttack;

        DiceRollAttack.CalculateResults();
    }

    public void PerformDefence(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        Attacker = attacker;
        Defender = defender;

        AttackStep = CombatStep.Defence;

        DefenceStart();

        Game.Selection.ActiveShip = defender;
        int currentAgility = defender.GetNumberOfDefenceDices(attacker);

        Game.UI.DiceResults.ShowDiceResultMenu();
        DiceRoll DiceRollDefence = new DiceRoll("defence", currentAgility);
        DiceRollDefence.Roll();
        CurentDiceRoll = DiceRollDefence;

        DiceRollDefence.CalculateResults();
    }

    public void CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (DiceRollAttack.Successes != 0)
        {
            defender.SufferDamage(DiceRollAttack);
        }
    }

    public void AttackStart()
    {
        Attacker.AttackStart();
        Defender.AttackStart();
    }

    public void DefenceStart()
    {
        Attacker.DefenceStart();
        Defender.DefenceStart();
    }

}
