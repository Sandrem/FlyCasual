using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Board;

public enum CombatStep
{
    None,
    Attack,
    Defence
}

public class DamageSourceEventArgs : EventArgs
{
    public object Source { get; set; }
    public DamageTypes DamageType { get; set; }
}

public enum DamageTypes
{
    ShipAttack,
    ObstacleCollision,
    CriticalHitCard
}

public static partial class Combat
{

    private static GameManagerScript Game;

    public static DiceRoll DiceRollAttack;
    public static DiceRoll DiceRollDefence;
    public static DiceRoll CurentDiceRoll;

    public static bool IsObstructed;

    public static CombatStep AttackStep = CombatStep.None;

    public static Ship.GenericShip Attacker;
    public static Ship.GenericShip Defender;

    public static Upgrade.GenericSecondaryWeapon SecondaryWeapon;

    public static CriticalHitCard.GenericCriticalHit CurrentCriticalHitCard;

    private static int attacksCounter;

    // Use this for initialization
    static Combat()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void DeclareTarget()
    {
        Game.UI.HideContextMenu();

        int attackTypesAreAvailable = Selection.ThisShip.GetAttackTypes();

        if (attackTypesAreAvailable > 1)
        {
            Phases.StartTemporarySubPhase(
                "Choose weapon for attack",
                typeof(SubPhases.WeaponSelectionDecisionSubPhase),
                Combat.TryPerformAttack
            );
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

        if (Rules.TargetIsLegalForShot.IsLegal())
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip);
            shotInfo.CheckFirelineCollisions(delegate { PerformAttack(Selection.ThisShip, Selection.AnotherShip); });
        }
        else
        {
            if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
            {
                //TODO: except non-legal targets, bupmed for example, biggs?
                Roster.HighlightShipsFiltered(Roster.AnotherPlayer(Phases.CurrentPhasePlayer));
                Game.UI.HighlightNextButton();
            }
            else
            {
                Selection.ThisShip.IsAttackPerformed = true;
                Phases.FinishSubPhase(typeof(SubPhases.CombatSubPhase));
            }
        }
    }

    public static void PerformAttack(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        if (DebugManager.DebugPhases) Debug.Log("Attack is started: " + attacker + " vs " + defender);
        Attacker = attacker;
        Defender = defender;
        InitializeAttack(AttackDiceRoll);
    }

    private static void InitializeAttack(Action callBack)
    {
        Roster.AllShipsHighlightOff();

        AttackStep = CombatStep.Attack;
        CallAttackStartEvents();
        Selection.ActiveShip = Attacker;
        if (SecondaryWeapon != null)
        {
            SecondaryWeapon.PayAttackCost(callBack);
        }
        else
        {
            callBack();
        }
    }

    private static void AttackDiceRoll()
    {
        Selection.ActiveShip = Selection.ThisShip;
        Phases.StartTemporarySubPhase(
            "Attack dice roll",
            typeof(SubPhases.AttackDiceRollCombatSubPhase)
        );
    }

    public static void ShowAttackAnimationAndSound()
    {
        if (SecondaryWeapon != null)
        {
            if (SecondaryWeapon.Type == Upgrade.UpgradeType.Torpedoes || SecondaryWeapon.Type == Upgrade.UpgradeType.Missiles)
            {
                Sounds.PlayShots("Proton-Torpedoes", 1);
                Selection.ThisShip.AnimateMunitionsShot();
            }
            else if (SecondaryWeapon.Type == Upgrade.UpgradeType.Turret)
            {
                Sounds.PlayShots(Selection.ActiveShip.SoundShotsPath, Selection.ActiveShip.ShotsCount);
                Selection.ThisShip.AnimateTurretWeapon();
            }

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
        Phases.StartTemporarySubPhase(
            "Defence dice roll",
            typeof(SubPhases.DefenceDiceRollCombatSubPhase)
        );
    }

    public static void CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        DiceRollAttack.RemoveAllFailures();

        if (DiceRollAttack.Successes > 0)
        {
            Attacker.CallOnAttackHitAsAttacker();
            Defender.CallOnAttackHitAsDefender();

            Triggers.ResolveTriggers(TriggerTypes.OnAttackHit, delegate { ResolveCombatDamage(SufferDamage); });
        }
        else
        {
            SufferDamage();
        }        
    }

    private static void ResolveCombatDamage(Action callBack)
    {
        Defender.AssignedDamageDiceroll = DiceRollAttack;

        foreach (var dice in DiceRollAttack.DiceList)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Defender.Owner.PlayerNo,
                EventHandler = Defender.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = Attacker,
                    DamageType = DamageTypes.ShipAttack
                },
                Skippable = true
            });
        }

        callBack();
    }

    private static void SufferDamage()
    {
        Triggers.ResolveTriggers(
            TriggerTypes.OnDamageIsDealt,
            delegate {
                Phases.FinishSubPhase(typeof(SubPhases.CompareResultsSubPhase));
                CheckTwinAttack();
            }
        );
    }

    private static void CheckTwinAttack()
    {
        if (SecondaryWeapon != null && SecondaryWeapon.IsTwinAttack)
        {
            if (attacksCounter == 0)
            {
                attacksCounter++;

                AttackStep = CombatStep.Attack;
                Selection.ActiveShip = Attacker;
                AttackDiceRoll();
            }
            else
            {
                attacksCounter = 0;
                CallCombatEndEvents();
            }
        }
        else
        {
            CallCombatEndEvents();
        }
    }

    private static void CallCombatEndEvents()
    {
        Attacker.CallCombatEnd();
        Defender.CallCombatEnd();

        if (Roster.NoSamePlayerAndPilotSkillNotAttacked(Selection.ThisShip))
        {
            Phases.FinishSubPhase(typeof(SubPhases.CombatSubPhase));
        }
    }

    public static void SelectWeapon(Upgrade.GenericSecondaryWeapon secondaryWeapon = null)
    {
        SecondaryWeapon = secondaryWeapon;
    }

    public static void ConfirmDiceResults()
    {
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
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.AttackDiceRollCombatSubPhase));

        PerformDefence(Selection.ThisShip, Selection.AnotherShip);
    }

    public static void ConfirmDefenceDiceResults()
    {
        DiceCompareHelper.currentDiceCompareHelper.Close();
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.DefenceDiceRollCombatSubPhase));

        MovementTemplates.ReturnRangeRuler();

        Phases.StartTemporarySubPhase("Compare results", typeof(SubPhases.CompareResultsSubPhase));
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

}

namespace SubPhases
{
    public class WeaponSelectionDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip);
            infoText = "Choose weapon for attack (Range " + shotInfo.Range + ")";

            if (shotInfo.Range <= 3 && shotInfo.InArc)
            {
                AddDecision("Primary", PerformPrimaryAttack);
            }

            foreach (var upgrade in Selection.ThisShip.InstalledUpgrades)
            {
                Upgrade.GenericSecondaryWeapon secondaryWeapon = upgrade.Value as Upgrade.GenericSecondaryWeapon;
                if (secondaryWeapon != null)
                {
                    if (secondaryWeapon.IsShotAvailable(Selection.AnotherShip))
                    {
                        AddDecision(secondaryWeapon.Name, delegate { PerformSecondaryWeaponAttack(secondaryWeapon, null); });
                        AddTooltip(secondaryWeapon.Name, secondaryWeapon.ImageUrl);
                    }
                }
            }
            defaultDecision = GetDecisions().Last().Key;
        }

        private void PerformPrimaryAttack(object sender, EventArgs e)
        {
            Combat.SelectWeapon();
            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));
            CallBack();
        }

        public void PerformSecondaryWeaponAttack(object sender, EventArgs e)
        {
            Tooltips.EndTooltip();

            Upgrade.GenericSecondaryWeapon secondaryWeapon = null;
            secondaryWeapon = sender as Upgrade.GenericSecondaryWeapon;

            if (secondaryWeapon == null) Debug.Log("Ooops! Secondary weapon is incorrect!");

            Combat.SelectWeapon(secondaryWeapon);

            Messages.ShowInfo("Attack with " + secondaryWeapon.Name);

            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));

            CallBack();
        }

    }
}

namespace SubPhases
{

    public class AttackDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {
        public override void Prepare()
        {
            CanBePaused = true;

            dicesType = DiceKind.Attack;
            dicesCount = Combat.Attacker.GetNumberOfAttackDices(Combat.Defender);

            checkResults = CheckResults;
            CallBack = Combat.ConfirmAttackDiceResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            Combat.CurentDiceRoll = diceRoll;
            Combat.DiceRollAttack = diceRoll;

            base.CheckResults(diceRoll);
        }

        public override void Pause()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(false);
        }

        public override void Resume()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(true);
        }

    }

    public class DefenceDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {
        public override void Prepare()
        {
            dicesType = DiceKind.Defence;
            dicesCount = Combat.Defender.GetNumberOfDefenceDices(Combat.Attacker);

            checkResults = CheckResults;
            CallBack = Combat.ConfirmDefenceDiceResults;

            new DiceCompareHelper(Combat.DiceRollAttack);
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.AnotherShip;

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
            DealDamage();
        }

        private void DealDamage()
        {
            if (DebugManager.DebugPhases) Debug.Log("Deal Damage!");
            Combat.CalculateAttackResults(Selection.ThisShip, Selection.AnotherShip);
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

