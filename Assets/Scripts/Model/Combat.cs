using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatStep
{
    None,
    Attack,
    Defence
}

public static partial class Combat
{

    private static GameManagerScript Game;

    public static DiceRoll DiceRollAttack;
    public static DiceRoll DiceRollDefence;
    public static DiceRoll CurentDiceRoll;

    public static CombatStep AttackStep = CombatStep.None;

    public static Ship.GenericShip Attacker;
    public static Ship.GenericShip Defender;

    // Use this for initialization
    static Combat() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
    public static void PerformAttack(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        Attacker = attacker;
        Defender = defender;
        InitializeAttack();
        
        AttackDiceRoll();
    }

    private static void InitializeAttack()
    {
        AttackStep = CombatStep.Attack;
        CallAttackStartEvents();
        Game.Selection.ActiveShip = Attacker;
    }

    private static void AttackDiceRoll()
    {
        ShowDiceResultMenu();
        DiceRoll DiceRollAttack = new DiceRoll("attack", Attacker.GetNumberOfAttackDices(Defender));
        DiceRollAttack.Roll();
        CurentDiceRoll = DiceRollAttack;
        DiceRollAttack.CalculateResults();
    }

    public static void PerformDefence(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        Attacker = attacker;
        Defender = defender;
        InitializeDefence();

        DefenceDiceRoll();
    }

    private static void InitializeDefence()
    {
        AttackStep = CombatStep.Defence;
        CallDefenceStartEvents();
        Game.Selection.ActiveShip = Defender;
    }

    private static void DefenceDiceRoll()
    {
        ShowDiceResultMenu();
        DiceRoll DiceRollDefence = new DiceRoll("defence", Defender.GetNumberOfDefenceDices(Attacker));
        DiceRollDefence.Roll();
        CurentDiceRoll = DiceRollDefence;
        DiceRollDefence.CalculateResults();
    }

    public static void CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (DiceRollAttack.Successes != 0)
        {
            defender.SufferDamage(DiceRollAttack);
        }
    }

    public static void CallAttackStartEvents()
    {
        Attacker.AttackStart();
        Defender.AttackStart();
    }

    public static void CallDefenceStartEvents()
    {
        Attacker.DefenceStart();
        Defender.DefenceStart();
    }

}
