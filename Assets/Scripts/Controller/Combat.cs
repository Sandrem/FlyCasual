using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatStep
{
    None,
    Attack,
    Defence
}

public static  class Combat
{

    private static GameManagerScript Game;

    public static  DiceRoll DiceRollAttack;
    public static  DiceRoll DiceRollDefence;
    public static  DiceRoll CurentDiceRoll;

    public static  CombatStep AttackStep = CombatStep.None;

    public static  Ship.GenericShip Attacker;
    public static  Ship.GenericShip Defender;

    // Use this for initialization
    static Combat() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
    public static  void PerformAttack(Ship.GenericShip attacker, Ship.GenericShip defender)
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

    public static  void PerformDefence(Ship.GenericShip attacker, Ship.GenericShip defender)
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

    public static  void CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (DiceRollAttack.Successes != 0)
        {
            defender.SufferDamage(DiceRollAttack);
        }
    }

    public static  void AttackStart()
    {
        Attacker.AttackStart();
        Defender.AttackStart();
    }

    public static  void DefenceStart()
    {
        Attacker.DefenceStart();
        Defender.DefenceStart();
    }

}
