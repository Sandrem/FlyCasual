using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Board;
using GameModes;
using Ship;
using SubPhases;

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

    public static GenericShip Attacker;
    public static GenericShip Defender;

    public static IShipWeapon ChosenWeapon;

    public static CriticalHitCard.GenericCriticalHit CurrentCriticalHitCard;

    private static int attacksCounter;
    private static int hitsCounter;

    public static ShipShotDistanceInformation ShotInfo;

    public static Func<GenericShip, IShipWeapon, bool> ExtraAttackFilter;

    private static bool IsAttackAlreadyCalled;

    public static void Initialize()
    {
        CleanupCombatData();
    }

    // DECLARE INTENT TO ATTACK

    public static void DeclareIntentToAttack(int attackerId, int defenderID)
    {
        if (!IsAttackAlreadyCalled)
        {
            IsAttackAlreadyCalled = true;

            Selection.ChangeActiveShip("ShipId:" + attackerId);
            Selection.ChangeAnotherShip("ShipId:" + defenderID);

            ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
            ShotInfo = new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);

            UI.HideContextMenu();

            SelectWeapon();
        }
        else
        {
            Debug.Log("Attack was called when attack is already called - ignore");
        }
    }

    // CHECK AVAILABLE WEAPONS TO ATTACK THIS TARGET

    private static void SelectWeapon()
    {
        int anotherAttacksTypesCount = Selection.ThisShip.GetAnotherAttackTypesCount();

        if (anotherAttacksTypesCount > 0)
        {
            Phases.StartTemporarySubPhaseOld(
                "Choose weapon for attack",
                typeof(WeaponSelectionDecisionSubPhase),
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
            IsAttackAlreadyCalled = false;
            Roster.GetPlayer(Phases.CurrentPhasePlayer).OnTargetNotLegalForAttack();
        }
    }

    private static bool IsTargetLegalForAttack()
    {
        bool result = false;

        if (Rules.TargetIsLegalForShot.IsLegal(true) && ChosenWeapon.IsShotAvailable(Selection.AnotherShip))
        {
            if (ExtraAttackFilter == null || ExtraAttackFilter(Selection.AnotherShip, ChosenWeapon))
            {
                result = true;
            }
        }

        return result;
    }

    private static void CheckFireLineCollisions()
    {
        ShotInfo = (ChosenWeapon.GetType() == typeof(PrimaryWeaponClass)) ? ShotInfo : new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
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
            typeof(AttackDiceRollCombatSubPhase)
        );
    }

    public static void ConfirmAttackDiceResults()
    {
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(AttackDiceRollCombatSubPhase));

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
            typeof(DefenceDiceRollCombatSubPhase)
        );
    }

    // COMPARE RESULTS

    public static void ConfirmDefenceDiceResults()
    {
        DiceCompareHelper.currentDiceCompareHelper.Close();
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(DefenceDiceRollCombatSubPhase));

        MovementTemplates.ReturnRangeRuler();

        Phases.StartTemporarySubPhaseOld("Compare results", typeof(CompareResultsSubPhase));
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
        Phases.FinishSubPhase(typeof(CompareResultsSubPhase));
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
            CheckSecondAttack(Phases.CurrentSubPhase.CallBack);
        }
        else
        {
            CheckExtraAttacks();
        }
    }

    private static void CheckExtraAttacks()
    {
        Triggers.ResolveTriggers(TriggerTypes.OnExtraAttack, Phases.CurrentSubPhase.CallBack);
    }

    private static void CleanupCombatData()
    {
        AttackStep = CombatStep.None;
        Attacker = null;
        Defender = null;
        ChosenWeapon = null;
        ShotInfo = null;
        hitsCounter = 0;
        IsObstructed = false;
        ExtraAttackFilter = null;
        IsAttackAlreadyCalled = false;
    }

    private static void CheckSecondAttack(Action callBack)
    {
        Selection.ThisShip.CallCheckSecondAttack(callBack);
    }

    public static void CheckFinishCombatSubPhase()
    {
        if (Roster.NoSamePlayerAndPilotSkillNotAttacked(Selection.ThisShip))
        {
            Phases.FinishSubPhase(typeof(CombatSubPhase));
        }
    }

    // Extra Attacks

    public static void StartAdditionalAttack(GenericShip ship, Action callback, Func<GenericShip, IShipWeapon, bool> extraAttackFilter = null)
    {
        Selection.ChangeActiveShip("ShipId:" + ship.ShipId);

        Phases.StartTemporarySubPhaseOld(
            "Second attack",
            typeof(SelectTargetForSecondAttackSubPhase),
            delegate { ExtraAttackTargetSelected(callback, extraAttackFilter); }
        );
    }

    private static void ExtraAttackTargetSelected(Action callback, Func<GenericShip, IShipWeapon, bool> extraAttackFilter)
    {
        Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
        Selection.ThisShip.IsAttackPerformed = false;
        Phases.StartTemporarySubPhaseNew(
            "Extra Attack",
            typeof(ExtraAttackSubPhase),
            delegate {
                Phases.FinishSubPhase(typeof(ExtraAttackSubPhase));
                callback();
            }
        );
        ExtraAttackFilter = extraAttackFilter;
        Combat.DeclareIntentToAttack(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
    }

}

namespace SubPhases
{
    public class WeaponSelectionDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            List<IShipWeapon> allWeapons = Selection.ThisShip.GetAllWeapons();

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

        public void PerformAttackWithWeapon(IShipWeapon weapon)
        {
            Tooltips.EndTooltip();

            Combat.ChosenWeapon = weapon;

            Messages.ShowInfo("Attack with " + weapon.Name);

            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));
            CallBack();
        }

    }

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

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }
    }

    public class ExtraAttackSubPhase : GenericSubPhase
    {
        public override void Start()
        {
            Name = "Extra Attack";
            UpdateHelpInfo();

            UI.ShowSkipButton();
        }

        public override void SkipButton()
        {
            CallBack();
        }

        public void RevertSubphase()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
        }
    }

}

