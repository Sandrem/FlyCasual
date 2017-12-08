using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Board;
using GameModes;

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
    CriticalHitCard,
    BombDetonation,
    CardAbility
}

public static partial class Combat
{

    public static DiceRoll DiceRollAttack;
    public static DiceRoll DiceRollDefence;
    public static DiceRoll CurrentDiceRoll;

    public static bool IsObstructed;

    public static CombatStep AttackStep = CombatStep.None;

    public static Ship.GenericShip Attacker;
    public static Ship.GenericShip Defender;

    public static Ship.IShipWeapon ChosenWeapon;

    public static CriticalHitCard.GenericCriticalHit CurrentCriticalHitCard;

    private static int attacksCounter;
    private static int hitsCounter;

    public static ShipShotDistanceInformation ShotInfo;

    // DECLARE INTENT TO ATTACK

    public static void DeclareIntentToAttack(int attackerId, int defenderID)
    {
        Selection.ChangeActiveShip("ShipId:" + attackerId);
        Selection.ChangeAnotherShip("ShipId:" + defenderID);

        ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
        ShotInfo = new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);

        UI.HideContextMenu();

        SelectWeapon();
    }

    // CHECK AVAILABLE WEAPONS TO ATTACK THIS TARGET

    private static void SelectWeapon()
    {
        int anotherAttacksTypesCount = Selection.ThisShip.GetAnotherAttackTypesCount();

        if (anotherAttacksTypesCount > 0)
        {
            Phases.StartTemporarySubPhaseOld(
                "Choose weapon for attack",
                typeof(SubPhases.WeaponSelectionDecisionSubPhase),
                TryPerformAttack
            );
        }
        else
        {
            TryPerformAttack();
        }
    }

    // CHECK LEGALITY OF ATTACK

    public static void TryPerformAttack()
    {
        UI.HideContextMenu();
        MovementTemplates.ReturnRangeRuler();

        if (IsTargetLegalForAttack())
        {
            UI.HideSkipButton();
            Roster.AllShipsHighlightOff();

            DeclareAttackerAndDefender();

            CheckFireLineCollisions();
        }
        else
        {
            Roster.GetPlayer(Phases.CurrentPhasePlayer).OnTargetNotLegalForAttack();
        }
    }

    private static bool IsTargetLegalForAttack()
    {
        bool result = false;

        if (Rules.TargetIsLegalForShot.IsLegal(true) && ChosenWeapon.IsShotAvailable(Selection.AnotherShip))
        {
            result = true;
        }

        return result;
    }

    private static void CheckFireLineCollisions()
    {
        ShotInfo = (ChosenWeapon.GetType() == typeof(Ship.PrimaryWeaponClass)) ? ShotInfo : new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
        ShotInfo.CheckFirelineCollisions(PayAttackCost);
    }

    // PAY ATTACK COST

    private static void PayAttackCost()
    {
        ChosenWeapon.PayAttackCost(StartAttack);
    }

    // DECLARE REAL ATTACK

    public static void StartAttack()
    {
        AttackStep = CombatStep.Attack;
        Selection.ActiveShip = Attacker;

        CallAttackStart();
    }

    private static void DeclareAttackerAndDefender()
    {
        if (DebugManager.DebugPhases) Debug.Log("Attack is started: " + Selection.ThisShip + " vs " + Selection.AnotherShip);

        Attacker = Selection.ThisShip;
        Defender = Selection.AnotherShip;        
    }

    public static void CallAttackStart()
    {
        Attacker.CallAttackStart();
        Defender.CallAttackStart();

        Triggers.ResolveTriggers(TriggerTypes.OnAttackStart, ShotStart);
    }

    // SHOT OF ATTACK (TWICE FOR TWIN ATTACK)

    private static void ShotStart()
    {
        Attacker.CallShotStart();
        Defender.CallShotStart();

        Triggers.ResolveTriggers(TriggerTypes.OnShotStart, AttackDiceRoll);
    }

    private static void AttackDiceRoll()
    {
        Selection.ActiveShip = Selection.ThisShip;
        Phases.StartTemporarySubPhaseOld(
            "Attack dice roll",
            typeof(SubPhases.AttackDiceRollCombatSubPhase)
        );
    }

    public static void ConfirmAttackDiceResults()
    {
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.AttackDiceRollCombatSubPhase));

        PerformDefence();
    }

    // DEFENCE

    public static void PerformDefence()
    {
        AttackStep = CombatStep.Defence;

        CallDefenceStartEvents();
        Selection.ActiveShip = Defender;

        DefenceDiceRoll();
    }

    public static void CallDefenceStartEvents()
    {
        Attacker.CallDefenceStart();
        Defender.CallDefenceStart();
    }

    private static void DefenceDiceRoll()
    {
        Selection.ActiveShip = Selection.AnotherShip;
        Phases.StartTemporarySubPhaseOld(
            "Defence dice roll",
            typeof(SubPhases.DefenceDiceRollCombatSubPhase)
        );
    }

    // COMPARE RESULTS

    public static void ConfirmDefenceDiceResults()
    {
        DiceCompareHelper.currentDiceCompareHelper.Close();
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(SubPhases.DefenceDiceRollCombatSubPhase));

        MovementTemplates.ReturnRangeRuler();

        Phases.StartTemporarySubPhaseOld("Compare results", typeof(SubPhases.CompareResultsSubPhase));
    }

    public static void CancelHitsByDefenceDice()
    {
        int crits = DiceRollAttack.CriticalSuccesses;
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (crits > DiceRollAttack.CriticalSuccesses)
        {
            Attacker.CallOnAtLeastOneCritWasCancelledByDefender();
            Triggers.ResolveTriggers(
                TriggerTypes.OnAtLeastOneCritWasCancelledByDefender,
                delegate
                {
                    CalculateAttackResults();
                });
        }
        else
        {
            CalculateAttackResults();
        }
    }

    private static void CalculateAttackResults()
    {
        DiceRollAttack.RemoveAllFailures();

        if (DiceRollAttack.Successes > 0)
        {
            AttackHit();
        }
        else
        {
            AfterShotIsPerformed();
        }
    }

    private static void AttackHit()
    {
        hitsCounter++;

        Attacker.CallShotHitAsAttacker();
        Defender.CallShotHitAsDefender();

        Triggers.ResolveTriggers(TriggerTypes.OnShotHit, TryDamagePrevention);
    }

    private static void TryDamagePrevention()
    {
        Defender.CallTryDamagePrevention(ResolveCombatDamage);
    }

    private static void ResolveCombatDamage()
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

        SufferDamage();
    }

    private static void SufferDamage()
    {
        Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, AfterShotIsPerformed);
    }

    private static void AfterShotIsPerformed()
    {
        Phases.FinishSubPhase(typeof(SubPhases.CompareResultsSubPhase));
        CheckTwinAttack();
    }

    // TWIN ATTACK

    private static void CheckTwinAttack()
    {
        Upgrade.GenericSecondaryWeapon chosenSecondaryWeapon = ChosenWeapon as Upgrade.GenericSecondaryWeapon;
        if (chosenSecondaryWeapon != null && chosenSecondaryWeapon.IsTwinAttack)
        {
            if (attacksCounter == 0)
            {
                attacksCounter++;

                //Here start attack
                AttackStep = CombatStep.Attack;
                Selection.ActiveShip = Attacker;
                ShotStart();
            }
            else
            {
                attacksCounter = 0;
                CheckMissedAttack();
            }
        }
        else
        {
            CheckMissedAttack();
        }
    }

    private static void CheckMissedAttack()
    {
        if (hitsCounter > 0)
        {
            Attacker.CallOnAttackHitAsAttacker();
            Defender.CallOnAttackHitAsDefender();

            Triggers.ResolveTriggers(TriggerTypes.OnAttackHit, FinishAttack);
        }
        else
        {
            Attacker.CallOnAttackMissedAsAttacker();
            Defender.CallOnAttackMissedAsDefender();

            FinishAttack();
        }
    }

    private static void FinishAttack()
    {
        Selection.ThisShip = Attacker;

        Attacker.CallAttackFinish();
        Defender.CallAttackFinish();

        Triggers.ResolveTriggers(TriggerTypes.OnAttackFinish, CombatEnd);
    }

    private static void CombatEnd()
    {
        CleanupCombatData();

        if (!Selection.ThisShip.IsCannotAttackSecondTime)
        {
            CheckSecondAttack(CheckFinishCombatSubPhase);
        }
        else
        {
            CheckFinishCombatSubPhase();
        }
    }

    private static void CleanupCombatData()
    {
        AttackStep = CombatStep.None;
        Attacker = null;
        Defender = null;
        ChosenWeapon = null;
        ShotInfo = null;
        hitsCounter = 0;
    }

    private static void CheckSecondAttack(Action callBack)
    {
        Selection.ThisShip.CallCheckSecondAttack(callBack);
    }

    private static void CheckFinishCombatSubPhase()
    {
        if (Roster.NoSamePlayerAndPilotSkillNotAttacked(Selection.ThisShip))
        {
            Phases.FinishSubPhase(typeof(SubPhases.CombatSubPhase));
        }
    }

}

namespace SubPhases
{
    public class WeaponSelectionDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            List<Ship.IShipWeapon> allWeapons = GetAllWeapons();

            //TODO: Range?
            InfoText = "Choose weapon for attack";

            foreach (var weapon in allWeapons)
            {
                if (weapon.IsShotAvailable(Selection.AnotherShip))
                {
                    AddDecision(weapon.Name, delegate { PerformAttackWithWeapon(weapon); });
                    AddTooltip(weapon.Name, (weapon as Upgrade.GenericSecondaryWeapon != null) ? (weapon as Upgrade.GenericSecondaryWeapon).ImageUrl : null );
                }
            }

            DefaultDecision = GetDecisions().Last().Key;

            callBack();
        }

        private static List<Ship.IShipWeapon> GetAllWeapons()
        {
            List<Ship.IShipWeapon> allWeapons = new List<Ship.IShipWeapon>();

            allWeapons.Add(Selection.ThisShip.PrimaryWeapon);

            foreach (var upgrade in Selection.ThisShip.UpgradeBar.GetInstalledUpgrades())
            {
                Ship.IShipWeapon secondaryWeapon = upgrade as Ship.IShipWeapon;
                if (secondaryWeapon != null) allWeapons.Add(secondaryWeapon);
            }

            return allWeapons;
        }

        public void PerformAttackWithWeapon(Ship.IShipWeapon weapon)
        {
            Tooltips.EndTooltip();

            Combat.ChosenWeapon = weapon;

            Messages.ShowInfo("Attack with " + weapon.Name);

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

            diceType = DiceKind.Attack;
            diceCount = Combat.Attacker.GetNumberOfAttackDice(Combat.Defender);

            checkResults = CheckResults;
            CallBack = Combat.ConfirmAttackDiceResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            Combat.CurrentDiceRoll = diceRoll;
            Combat.DiceRollAttack = diceRoll;

            Combat.Attacker.CallCheckCancelCritsFirst();
            Combat.Defender.CallCheckCancelCritsFirst();

            base.CheckResults(diceRoll);
        }

        public override void Pause()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);
        }
    }

    public class DefenceDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {
        public override void Prepare()
        {
            diceType = DiceKind.Defence;
            diceCount = Combat.Defender.GetNumberOfDefenceDice(Combat.Attacker);

            checkResults = CheckResults;
            CallBack = Combat.ConfirmDefenceDiceResults;

            new DiceCompareHelper(Combat.DiceRollAttack);
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.AnotherShip;

            Combat.CurrentDiceRoll = diceRoll;
            Combat.DiceRollDefence = diceRoll;

            base.CheckResults(diceRoll);
        }

    }

    public class CompareResultsSubPhase : GenericSubPhase
    {
        public override void Start()
        {
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
            Combat.CancelHitsByDefenceDice();
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

