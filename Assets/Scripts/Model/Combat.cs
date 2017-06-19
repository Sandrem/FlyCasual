using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public static Upgrade.GenericSecondaryWeapon SecondaryWeapon;

    // Use this for initialization
    static Combat() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void DeclareTarget()
    {
        Game.UI.HideContextMenu();

        bool inArc = Actions.InArcCheck(Selection.ThisShip, Selection.AnotherShip);
        int distance = Actions.GetFiringRange(Selection.ThisShip, Selection.AnotherShip);
        int attackTypesAreAvailable = Selection.ThisShip.GetAttackTypes(distance, inArc);

        if (attackTypesAreAvailable > 1)
        {
            Phases.StartTemporarySubPhase("Choose weapon for attack", typeof(SubPhases.WeaponSelectionDecisionSubPhase));
        }
        else
        {
            SelectWeapon();
            TryPerformAttack();
        }
    }

    public static void TryPerformAttack()
    {
        Game.UI.HideContextMenu();
        MovementTemplates.ReturnRangeRuler();

        //TODO: CheckShot is needed before
        if (Actions.TargetIsLegal())
        {
            Board.LaunchObstacleChecker(Selection.ThisShip, Selection.AnotherShip);
            //Call later "Combat.PerformAttack(Selection.ThisShip, Selection.AnotherShip);"
        }
        else
        {
            if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
            {
                Roster.HighlightShipsFiltered(Roster.AnotherPlayer(Phases.CurrentPhasePlayer));
                Game.UI.HighlightNextButton();
            }
            else
            {
                Debug.Log("TARGET IS ILLEGAL!");
                Selection.ThisShip.IsAttackPerformed = true;
                Phases.FinishSubPhase(typeof(SubPhases.CombatSubPhase));
            }
        }
    }

    public static void PerformAttack(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        Debug.Log("Attack is started: " + attacker + " vs " + defender);
        Attacker = attacker;
        Defender = defender;
        InitializeAttack();

        AttackDiceRoll();
    }

    private static void InitializeAttack()
    {
        Roster.AllShipsHighlightOff();

        AttackStep = CombatStep.Attack;
        CallAttackStartEvents();
        Selection.ActiveShip = Attacker;
        if (SecondaryWeapon != null) SecondaryWeapon.PayAttackCost();
    }

    private static void AttackDiceRoll()
    {
        Selection.ActiveShip = Selection.ThisShip;
        Phases.StartTemporarySubPhase("Attack dice roll", typeof(SubPhases.AttackDiceRollCombatSubPhase));
    }

    public static void ShowAttackAnimationAndSound()
    {
        if (SecondaryWeapon != null)
        {
            Sounds.PlayShots("Proton-Torpedoes", 1);
            //Selection.ThisShip.AnimateTorpedoes();
        }
        else
        {
            Sounds.PlayShots(Selection.ActiveShip.SoundShotsPath, Selection.ActiveShip.ShotsCount);
            Selection.ThisShip.AnimatePrimaryWeapon();
        }
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
        Selection.ActiveShip = Defender;
    }

    private static void DefenceDiceRoll()
    {
        Selection.ActiveShip = Selection.AnotherShip;
        Phases.StartTemporarySubPhase("Defence dice roll", typeof(SubPhases.DefenceDiceRollCombatSubPhase));
    }

    public static IEnumerator CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (DiceRollAttack.Successes != 0)
        {
            yield return defender.SufferDamage(DiceRollAttack);
        }
        CallCombatEndEvents();
    }

    public static void CallAttackStartEvents()
    {
        Attacker.CallAttackStart();
        //BUG: NullReferenceException: Object reference not set to an instance of an object
        Defender.CallAttackStart();
    }

    public static void CallDefenceStartEvents()
    {
        Attacker.CallDefenceStart();
        Defender.CallDefenceStart();
    }

    private static void CallCombatEndEvents()
    {
        Attacker.CallCombatEnd();
        Defender.CallCombatEnd();
    }

    public static void SelectWeapon(Upgrade.GenericSecondaryWeapon secondaryWeapon = null)
    {
        SecondaryWeapon = secondaryWeapon;
    }

    public static void ConfirmDiceResults()
    {
        Debug.Log("ConfirmDiceResults is called");
        switch (AttackStep)
        {
            case CombatStep.Attack:
                ConfirmAttackDiceResults();
                break;
            case CombatStep.Defence:
                ConfirmDefenceDiceResults();
                break;
        }
    }

    public static void ConfirmAttackDiceResults()
    {
        Debug.Log("Combat Attack Dice Roll - confirmed");

        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.AttackDiceRollCombatSubPhase));

        PerformDefence(Selection.ThisShip, Selection.AnotherShip);
    }

    public static void ConfirmDefenceDiceResults()
    {
        Debug.Log("Combat Defence Dice Roll - confirmed");

        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.DefenceDiceRollCombatSubPhase));

        MovementTemplates.ReturnRangeRuler();

        Phases.StartTemporarySubPhase("Compare results", typeof(SubPhases.CompareResultsSubPhase));
    }

}

namespace SubPhases
{
    public class WeaponSelectionDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            int distance = Actions.GetFiringRange(Selection.ThisShip, Selection.AnotherShip);
            infoText = "Choose weapon for attack (Distance " + distance + ")";

            decisions.Add("Primary", PerformPrimaryAttack);
            decisions.Add("Proton Torpedoes", PerformTorpedoesAttack);

            //Temporary
            tooltips.Add("Proton Torpedoes", "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/e/eb/Proton-torpedoes.png");

            defaultDecision = "Proton Torpedoes";
        }

        private void PerformPrimaryAttack(object sender, EventArgs e)
        {
            Combat.SelectWeapon();
            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));
            Combat.TryPerformAttack();
        }

        public void PerformTorpedoesAttack(object sender, EventArgs e)
        {
            Tooltips.EndTooltip();

            //TODO: Get upgrade correctly
            Upgrade.GenericSecondaryWeapon secondaryWeapon = null;
            foreach (var upgrade in Selection.ThisShip.InstalledUpgrades)
            {
                if (upgrade.Key == Upgrade.UpgradeSlot.Torpedoes) secondaryWeapon = upgrade.Value as Upgrade.GenericSecondaryWeapon;
            }
            Combat.SelectWeapon(secondaryWeapon);

            Messages.ShowInfo("Attack with " + secondaryWeapon.Name);

            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));

            Combat.TryPerformAttack();
        }

    }
}

namespace SubPhases
{

    public class AttackDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {

        public override void Prepare()
        {
            dicesType = "attack";
            dicesCount = Combat.Attacker.GetNumberOfAttackDices(Combat.Defender);

            checkResults = CheckResults;
            finishAction = Combat.ConfirmAttackDiceResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            Combat.CurentDiceRoll = diceRoll;
            Combat.DiceRollAttack = diceRoll;

            base.CheckResults(diceRoll);
        }

    }

    public class DefenceDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {

        public override void Prepare()
        {
            dicesType = "defence";
            dicesCount = Combat.Defender.GetNumberOfDefenceDices(Combat.Attacker);

            checkResults = CheckResults;
            finishAction = Combat.ConfirmDefenceDiceResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            Combat.CurentDiceRoll = diceRoll;
            Combat.DiceRollDefence = diceRoll;

            base.CheckResults(diceRoll);
        }

    }

    public class CompareResultsSubPhase : GenericSubPhase
    {
        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Compare results";
            UpdateHelpInfo();

            Initialize();
        }

        public override void Initialize()
        {
            Game.StartCoroutine(DealDamage());
        }

        private IEnumerator DealDamage()
        {
            Debug.Log("Deal Damage!");
            yield return Combat.CalculateAttackResults(Selection.ThisShip, Selection.AnotherShip);

            Phases.FinishSubPhase(this.GetType());

            if (Roster.NoSamePlayerAndPilotSkillNotAttacked(Selection.ThisShip))
            {
                Phases.FinishSubPhase(typeof(CombatSubPhase));
            }
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }
    }

}

