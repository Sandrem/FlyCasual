using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BoardTools;
using Ship;
using SubPhases;
using GameCommands;
using Upgrade;
using Arcs;

public enum CombatStep
{
    None,
    Attack,
    Defence,
    CompareResults
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
    CardAbility,
    Rules,
    Console
}

public static partial class Combat
{

    public static DiceRoll DiceRollAttack;
    public static DiceRoll DiceRollDefence;
    public static DiceRoll CurrentDiceRoll;

    public static CombatStep AttackStep = CombatStep.None;

    public static GenericShip Attacker;
    public static GenericShip Defender;

    public static IShipWeapon ChosenWeapon;

    public static GenericDamageCard CurrentCriticalHitCard;

    private static int attacksCounter;
    private static int hitsCounter;

    public static ShotInfo ShotInfo;

    public static Func<GenericShip, IShipWeapon, bool, bool> ExtraAttackFilter;

    public static bool IsAttackAlreadyCalled;

    public static GenericArc ArcForShot;

    public static void Initialize()
    {
        CleanupCombatData();
    }

    // DECLARE INTENT TO ATTACK

    public static GameCommand GenerateIntentToAttackCommand(int attackerId, int defenderId, bool weaponIsAlreadySelected = false, IShipWeapon chosenWeapon = null)
    {
        if (!IsAttackAlreadyCalled)
        {
            IsAttackAlreadyCalled = true;

            JSONObject parameters = new JSONObject();
            parameters.AddField("id", attackerId.ToString());
            parameters.AddField("target", defenderId.ToString());
            parameters.AddField("weaponIsAlreadySelected", weaponIsAlreadySelected.ToString());
            parameters.AddField("weapon", chosenWeapon.Name);

            return GameController.GenerateGameCommand(
                GameCommandTypes.DeclareAttack,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }
        else
        {
            Debug.Log("Attack was called when attack is already called - ignore");
            return null;
        }
    }

    public static void DeclareIntentToAttack(int attackerId, int defenderId, bool weaponIsAlreadySelected = false)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        UI.HideContextMenu();
        UI.HideSkipButton();

        Selection.ChangeActiveShip("ShipId:" + attackerId);
        Selection.ChangeAnotherShip("ShipId:" + defenderId);

        if (!weaponIsAlreadySelected)
        {
            SelectWeapon();
        }
        else
        {
            ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
            TryPerformAttack(isSilent: true);
        }
    }

    // CHECK AVAILABLE WEAPONS TO ATTACK THIS TARGET

    private static void SelectWeapon()
    {
        int attackTypesCount = GetAttackTypesCount(Selection.ThisShip, Selection.AnotherShip);

        if (attackTypesCount > 0)
        {
            Phases.StartTemporarySubPhaseOld(
                "Choose weapon for attack",
                typeof(WeaponSelectionDecisionSubPhase),
                delegate { TryPerformAttack(isSilent: false); }
            );
        }
        else
        {
            // TODOREVERT
            ChosenWeapon = Selection.ThisShip.PrimaryWeapons.First();
            ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);

            //TODO: Show range ruler

            TryPerformAttack(isSilent: false);
        }
    }

    // COUNT ATTACK TYPES

    private static int GetAttackTypesCount(GenericShip thisShip, GenericShip anotherShip)
    {
        int result = 0;

        List<IShipWeapon> allWeapons = thisShip.GetAllWeapons();

        foreach (IShipWeapon IShipWeapon in allWeapons)
        {
            if (IShipWeapon.IsShotAvailable(anotherShip)) result++;
        }

        return result;
    }

    // CHECK LEGALITY OF ATTACK

    public static void TryPerformAttack(bool isSilent)
    {
        UI.HideContextMenu();

        MovementTemplates.ReturnRangeRuler();

        if (Rules.TargetIsLegalForShot.IsLegal(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon, isSilent))
        {
            UI.HideSkipButton();
            Roster.AllShipsHighlightOff();

            SetArcAsUsedForAttack();
            DeclareAttackerAndDefender();
            CheckFireLineCollisions();
        }
        else
        {
            IsAttackAlreadyCalled = false;
            Messages.ShowError("AI: Target is not legal for attack");
            Roster.GetPlayer(Phases.CurrentPhasePlayer).OnTargetNotLegalForAttack();
        }
    }

    private static void SetArcAsUsedForAttack()
    {
        Combat.ArcForShot = Combat.ShotInfo.ShotAvailableFromArcs.First();
        ArcForShot.WasUsedForAttackThisRound = true;
    }

    private static void CheckFireLineCollisions()
    {
        ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
        ShotInfo.CheckObstruction(PayAttackCost);
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
        Phases.StartTemporarySubPhaseOld("Attack dice roll", typeof(AttackDiceRollCombatSubPhase));
    }

    public static void ConfirmAttackDiceResults()
    {
        Attacker.ClearAlreadyUsedDiceModifications();
        Defender.ClearAlreadyUsedDiceModifications();
        Attacker.CallAfterAttackDiceModification();
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(AttackDiceRollCombatSubPhase));

        PerformDefence();
    }

    // DEFENCE

    public static void PerformDefence()
    {
        AttackStep = CombatStep.Defence;

        Selection.ActiveShip = Defender;

        CallDefenceStartEvents(DefenceDiceRoll);
    }

    public static void CallDefenceStartEvents(Action callback)
    {
        Attacker.CallDefenceStartAsAttacker();
        Defender.CallDefenceStartAsDefender();

        Triggers.ResolveTriggers(TriggerTypes.OnDefenseStart, callback);
    }

    private static void DefenceDiceRoll()
    {
        Selection.ActiveShip = Selection.AnotherShip;
        Phases.StartTemporarySubPhaseOld("Defence dice roll", typeof(DefenceDiceRollCombatSubPhase));
    }

    // COMPARE RESULTS

    public static void ConfirmDefenceDiceResults()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Attacker.ClearAlreadyUsedDiceModifications();
        Defender.ClearAlreadyUsedDiceModifications();

        AttackStep = CombatStep.CompareResults;

        Combat.Attacker.Owner.UseDiceModifications(DiceModificationTimingType.CompareResults);
    }

    public static void CompareResultsAndDealDamage()
    {
        Attacker.ClearAlreadyUsedDiceModifications();
        Defender.ClearAlreadyUsedDiceModifications();
        DiceCompareHelper.currentDiceCompareHelper.Close();
        HideDiceResultMenu();
        Phases.FinishSubPhase(typeof(DefenceDiceRollCombatSubPhase));

        MovementTemplates.ReturnRangeRuler();

        Phases.StartTemporarySubPhaseOld("Compare results", typeof(CompareResultsSubPhase));
    }

    public static void CancelHitsByDefenceDice()
    {
        int crits = DiceRollAttack.CriticalSuccesses;
        DiceRollAttack.CancelHitsByDefence(DiceRollDefence.Successes);
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
        Combat.Defender.CallCombatCompareResults();

        DiceRollAttack.RemoveAllFailures();

        Combat.Defender.CallAfterNeutralizeResults(CheckAttackHit);
    }

    private static void CheckAttackHit()
    {
        if (DiceRollAttack.Successes > 0)
        {
            AttackHit();
        }
        else
        {
            if (Attacker.AttackIsAlwaysConsideredHit)
            {
                Messages.ShowInfo("Attack is considered a Hit");
                AttackHit();
            }
            else
            {
                AfterShotIsPerformed();
            }
        }
    }

    private static void AttackHit()
    {
        hitsCounter++;

        Attacker.CallShotHitAsAttacker();
        Defender.CallShotHitAsDefender();

        Triggers.ResolveTriggers(TriggerTypes.OnShotHit, ResolveCombatDamage);
    }

    private static void ResolveCombatDamage()
    {
        DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
        {
            Source = Attacker,
            DamageType = DamageTypes.ShipAttack
        };

        Defender.Damage.TryResolveDamage(DiceRollAttack.DiceList, damageArgs, AfterShotIsPerformed);
    }

    private static void AfterShotIsPerformed()
    {
        Phases.FinishSubPhase(typeof(CompareResultsSubPhase));
        CheckTwinAttack();
    }

    // TWIN ATTACK

    private static void CheckTwinAttack()
    {
        GenericSpecialWeapon chosenSecondaryWeapon = ChosenWeapon as GenericSpecialWeapon;
        if (chosenSecondaryWeapon != null && chosenSecondaryWeapon.WeaponInfo.TwinAttack && !(Defender.IsDestroyed || Defender.IsReadyToBeDestroyed))
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

            Triggers.ResolveTriggers(TriggerTypes.OnAttackMissed, FinishAttack);
        }
    }

    private static void FinishAttack()
    {
        Selection.ThisShip = Attacker;

        Attacker.CallAttackFinishAsAttacker();
        Defender.CallAttackFinishAsDefender();

        Attacker.CallAttackFinish();
        Defender.CallAttackFinish();

        Attacker.CallAttackFinishGlobal(); // Only once!

        Triggers.ResolveTriggers(TriggerTypes.OnAttackFinish, FinishCombatActivation);
    }

    private static void FinishCombatActivation()
    {
        Attacker.CallCombatDeactivation(CleanupAndCheckExtraAttacks);
    }

    private static void CleanupAndCheckExtraAttacks()
    {
        CleanupCombatData();

        if (!Selection.ThisShip.IsCannotAttackSecondTime)
        {
            CheckExtraAttacks(FinishCombat);
        }
        else
        {
            FinishCombat();
        }
    }

    private static void FinishCombat()
    {
        Phases.CurrentSubPhase.CallBack();
    }

    private static void CheckExtraAttacks(Action callback)
    {
        Selection.ThisShip.CallCombatCheckExtraAttack(callback);
    }

    private static void CleanupCombatData()
    {
        AttackStep = CombatStep.None;
        Attacker = null;
        Defender = null;
        ChosenWeapon = null;
        ShotInfo = null;
        ArcForShot = null;
        hitsCounter = 0;
        ExtraAttackFilter = null;
        IsAttackAlreadyCalled = false;
    }

    public static void FinishCombatSubPhase()
    {
        Phases.FinishSubPhase(typeof(CombatSubPhase));
    }

    // Extra Attacks

    public static void StartAdditionalAttack(GenericShip ship, Action callback, Func<GenericShip, IShipWeapon, bool, bool> extraAttackFilter = null, 
        string abilityName = null, string description = null, IImageHolder imageSource = null, bool showSkipButton = true)
    {
        Selection.ChangeActiveShip("ShipId:" + ship.ShipId);
        Phases.CurrentSubPhase.RequiredPlayer = ship.Owner.PlayerNo;

        ExtraAttackFilter = extraAttackFilter;

        SelectTargetForSecondAttackSubPhase newAttackSubphase = (SelectTargetForSecondAttackSubPhase) Phases.StartTemporarySubPhaseNew(
            "Second attack",
            typeof(SelectTargetForSecondAttackSubPhase),
            //delegate { ExtraAttackTargetSelected(callback, extraAttackFilter); }
            callback
        );
        newAttackSubphase.AbilityName = abilityName;
        newAttackSubphase.Description = description;
        newAttackSubphase.ImageSource = imageSource;
        newAttackSubphase.ShowSkipButton = showSkipButton;

        newAttackSubphase.Start();
    }

}

namespace SubPhases
{
    public class WeaponSelectionDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            List<IShipWeapon> allWeapons = Selection.ThisShip.GetAllWeapons();

            InfoText = "Choose weapon for attack";

            foreach (var weapon in allWeapons)
            {
                if (weapon.IsShotAvailable(Selection.AnotherShip))
                {
                    AddDecision(weapon.Name, delegate { PerformAttackWithWeapon(weapon); });
                    AddTooltip(weapon.Name, (weapon as GenericSpecialWeapon != null) ? (weapon as GenericSpecialWeapon).ImageUrl : null );
                }
            }

            DefaultDecisionName = GetDecisions().Last().Name;

            callBack();
        }

        public void PerformAttackWithWeapon(IShipWeapon weapon)
        {
            Tooltips.EndTooltip();
            Messages.ShowInfo("Attack with " + weapon.Name);

            Combat.ChosenWeapon = weapon;
            Combat.ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, Combat.ChosenWeapon);

            Combat.ShotInfo.CheckObstruction(delegate{
                Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));
                CallBack();
            });
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
            Phases.CurrentSubPhase.Resume();
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
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.DeclareAttack, GameCommandTypes.PressSkip }; } }

        public override void Start()
        {
            Name = "Extra Attack";
            UpdateHelpInfo();

            UI.ShowSkipButton();

            IsReadyForCommands = true;
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
        }
    }

}

