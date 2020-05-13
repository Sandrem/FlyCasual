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

public enum PlayerRole
{
    Attacker,
    Defender
}

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

public static class Combat
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

    public static Action<Action> PayExtraAttackCost;

    public static Func<GenericShip, IShipWeapon, bool, bool> ExtraAttackFilter;

    public static bool IsAttackAlreadyCalled;

    public static GenericArc ArcForShot;

    public static bool SkipAttackDiceRollsAndHit;

    public static DiceModificationsManager DiceModifications;

    public static void Initialize()
    {
        CleanupCombatData();
    }

    // DECLARE INTENT TO ATTACK

    public static GameCommand GenerateIntentToAttackCommand(int attackerId, int defenderId, bool weaponIsAlreadySelected = false, IShipWeapon chosenWeapon = null)
    {
        if (!IsAttackAlreadyCalled)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;

            IsAttackAlreadyCalled = true;

            JSONObject parameters = new JSONObject();
            parameters.AddField("id", attackerId.ToString());
            parameters.AddField("target", defenderId.ToString());
            parameters.AddField("weaponIsAlreadySelected", weaponIsAlreadySelected.ToString());
            parameters.AddField("weapon", (chosenWeapon != null) ? chosenWeapon.Name : null);

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

        Action callback = Phases.CurrentSubPhase.CallBack;
        var subphase = Phases.StartTemporarySubPhaseNew(
            "Extra Attack",
            typeof(AttackExecutionSubphase),
            delegate {
                Phases.FinishSubPhase(typeof(AttackExecutionSubphase));
                callback();
            }
        );
        subphase.Start();

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
        List<IShipWeapon> weapons = GetAvailbleAttackTypes(Selection.ThisShip, Selection.AnotherShip);

        if (weapons.Count > 1)
        {
            Phases.StartTemporarySubPhaseOld(
                "Choose a weapon for this attack.",
                typeof(WeaponSelectionDecisionSubPhase),
                delegate { TryPerformAttack(isSilent: false); }
            );
        }
        else if (weapons.Count == 1)
        {
            Combat.ChosenWeapon = weapons.First();
            Messages.ShowInfo("Attacking with " + Combat.ChosenWeapon.Name);

            Combat.ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, Combat.ChosenWeapon);

            TryPerformAttack(isSilent: false);
        }
        else
        {
            // Messages.ShowError("Error: No weapon to use");
            TryPerformAttack(isSilent: false);
        }
    }

    // COUNT ATTACK TYPES

    private static List<IShipWeapon> GetAvailbleAttackTypes(GenericShip thisShip, GenericShip anotherShip)
    {
        List<IShipWeapon> availableWeapons = new List<IShipWeapon>();

        foreach (IShipWeapon shipWeapon in thisShip.GetAllWeapons())
        {
            if (shipWeapon.IsShotAvailable(anotherShip)) availableWeapons.Add(shipWeapon);
        }

        return availableWeapons;
    }

    // CHECK LEGALITY OF ATTACK

    public static void TryPerformAttack(bool isSilent)
    {
        UI.HideContextMenu();

        MovementTemplates.ReturnRangeRuler();

        if (ChosenWeapon != null && Rules.TargetIsLegalForShot.IsLegal(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon, isSilent))
        {
            if (!DebugManager.NoCinematicCamera)
            {
                CommandsList.ShotCamera.ShowShotCamera(Selection.ThisShip, Selection.AnotherShip);
                GameManagerScript.Wait(3, StartLegalAttack);
            }
            else
            {
                StartLegalAttack();
            }
        }
        else
        {
            IsAttackAlreadyCalled = false;
            Roster.GetPlayer(Phases.CurrentPhasePlayer).OnTargetNotLegalForAttack();
        }
    }

    private static void StartLegalAttack()
    {
        UI.HideSkipButton();
        Roster.AllShipsHighlightOff();

        SetArcAsUsedForAttack();
        DeclareAttackerAndDefender();
        ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
        PayAttackCost();
    }

    private static void SetArcAsUsedForAttack()
    {
        Combat.ArcForShot = Combat.ShotInfo.ShotAvailableFromArcs.FirstOrDefault();
        if (ArcForShot != null) ArcForShot.WasUsedForAttackThisRound = true;
    }

    // PAY ATTACK COST

    private static void PayAttackCost()
    {
        if (PayExtraAttackCost == null) PayExtraAttackCost = callback => callback();
        ChosenWeapon.PayAttackCost(() => PayExtraAttackCost(StartAttack));
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

        Attacker.ShowAttackAnimationAndSound();

        DiceModifications = new DiceModificationsManager();
        Triggers.ResolveTriggers(TriggerTypes.OnShotStart, DiceModifications.StartAttack);
    }

    // DEFENCE

    public static void PerformDefence(Action callback)
    {
        AttackStep = CombatStep.Defence;

        Selection.ActiveShip = Defender;

        CallDefenceStartEvents(callback);
    }

    public static void CallDefenceStartEvents(Action callback)
    {
        Attacker.CallDefenceStartAsAttacker();
        Defender.CallDefenceStartAsDefender();

        Triggers.ResolveTriggers(TriggerTypes.OnDefenseStart, callback);
    }

    public static void CancelHitsByDefenceDice()
    {
        if (SkipAttackDiceRollsAndHit) return;

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
                Messages.ShowInfo("This attack is always considered a Hit");
                AttackHit();
            }
            else
            {
                AfterShotIsPerformed();
            }
        }
    }

    public static void AttackHit()
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
    }

    public static void AfterAllDiceModificationsAreDone()
    {
        CheckTwinAttack();
    }

    // TWIN ATTACK

    private static void CheckTwinAttack()
    {
        GenericSpecialWeapon chosenSecondaryWeapon = ChosenWeapon as GenericSpecialWeapon;
        if (chosenSecondaryWeapon != null && chosenSecondaryWeapon.WeaponInfo.TwinAttack && !Defender.IsDestroyed)
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
        PayExtraAttackCost = null;
        SkipAttackDiceRollsAndHit = false;
    }

    public static void FinishCombatSubPhase()
    {
        Phases.FinishSubPhase(typeof(CombatSubPhase));
    }

    // Extra Attacks

    public static void StartSelectAttackTarget
    (
        GenericShip ship,
        Action callback,
        Func<GenericShip, IShipWeapon, bool, bool> extraAttackFilter = null,
        string abilityName = null,
        string description = null,
        IImageHolder imageSource = null,
        bool showSkipButton = true,
        Action<Action> payAttackCost = null
    )
    {
        Selection.ChangeActiveShip("ShipId:" + ship.ShipId);
        Phases.CurrentSubPhase.RequiredPlayer = ship.Owner.PlayerNo;

        ExtraAttackFilter = extraAttackFilter;
        PayExtraAttackCost = payAttackCost;

        SelectTargetForAttackSubPhase newAttackSubphase = (SelectTargetForAttackSubPhase) Phases.StartTemporarySubPhaseNew(
            "Second attack",
            typeof(SelectTargetForAttackSubPhase),
            delegate
            {
                ship.CallAfterAttackWindow();
                Phases.FinishSubPhase(typeof(SelectTargetForAttackSubPhase));
                CameraScript.RestoreCamera();
                callback();
            }
        );
        newAttackSubphase.DescriptionShort = abilityName;
        newAttackSubphase.DescriptionLong = description;
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

            DescriptionShort = "Choose weapon for attack";

            foreach (var weapon in allWeapons)
            {
                if (weapon.IsShotAvailable(Selection.AnotherShip))
                {
                    AddDecision(weapon.Name, delegate { PerformAttackWithWeapon(weapon); });
                    AddTooltip(weapon.Name, (weapon as GenericSpecialWeapon != null) ? (weapon as GenericSpecialWeapon).ImageUrl : null );
                }
            }

            RemoveExtraDecisions();

            DefaultDecisionName = GetDecisions().Last().Name;

            callBack();
        }

        private void RemoveExtraDecisions()
        {
            if (decisions.Any(n => n.Name == "Primary Weapon (Front)") && decisions.Any(n => n.Name == "Primary Weapon (360)"))
            {
                decisions.RemoveAll(n => n.Name == "Primary Weapon (Front)");
            }

            if (decisions.Any(n => n.Name == "Primary Weapon (Front)") && decisions.Any(n => n.Name == "Primary Weapon (Full Front)"))
            {
                decisions.RemoveAll(n => n.Name == "Primary Weapon (Front)");
            }
        }

        public void PerformAttackWithWeapon(IShipWeapon weapon)
        {
            Tooltips.EndTooltip();
            Messages.ShowInfo("Attacking with " + weapon.Name);

            Combat.ChosenWeapon = weapon;
            Combat.ShotInfo = new ShotInfo(Selection.ThisShip, Selection.AnotherShip, Combat.ChosenWeapon);

            Phases.FinishSubPhase(typeof(WeaponSelectionDecisionSubPhase));
            CallBack();
        }

    }

    public class AttackExecutionSubphase : GenericSubPhase
    {
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

