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
    public static DiceRoll CurentDiceRoll;

    public static bool IsObstructed;

    public static CombatStep AttackStep = CombatStep.None;

    public static Ship.GenericShip Attacker;
    public static Ship.GenericShip Defender;

    public static Ship.IShipWeapon ChosenWeapon;

    public static CriticalHitCard.GenericCriticalHit CurrentCriticalHitCard;

    private static int attacksCounter;

    public static ShipShotDistanceInformation ShotInfo;

    public static void DeclareTarget(int attackerId, int defenderID)
    {
        Selection.ChangeActiveShip("ShipId:" + attackerId);
        Selection.ChangeAnotherShip("ShipId:" + defenderID);

        if (Network.IsNetworkGame)
        {
            ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
            ShotInfo = new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
        }

        UI.HideContextMenu();

        int anotherAttacksTypesCount = Selection.ThisShip.GetAnotherAttackTypesCount();

        if (anotherAttacksTypesCount > 0)
        {
            Phases.StartTemporarySubPhase(
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

    public static void TryPerformAttack()
    {
        UI.HideContextMenu();
        MovementTemplates.ReturnRangeRuler();

        if (Rules.TargetIsLegalForShot.IsLegal() && ChosenWeapon.IsShotAvailable(Selection.AnotherShip))
        {
            UI.HideSkipButton();
            ShotInfo = (ChosenWeapon.GetType() == typeof(Ship.PrimaryWeaponClass)) ? ShotInfo : new ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon);
            ShotInfo.CheckFirelineCollisions(delegate { PerformAttack(Selection.ThisShip, Selection.AnotherShip); });
        }
        else
        {
            Roster.GetPlayer(Phases.CurrentPhasePlayer).OnTargetNotLegalForAttack();
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
        Triggers.ResolveTriggers(TriggerTypes.OnAttackStart, delegate
        {
            ChosenWeapon.PayAttackCost(callBack);
        });
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
        Upgrade.GenericSecondaryWeapon chosenSecondaryWeapon = ChosenWeapon as Upgrade.GenericSecondaryWeapon;
        if (chosenSecondaryWeapon != null && chosenSecondaryWeapon.Type != Upgrade.UpgradeType.Cannon)
        {
            if (chosenSecondaryWeapon.Type == Upgrade.UpgradeType.Torpedo || chosenSecondaryWeapon.Type == Upgrade.UpgradeType.Missile)
            {
                Sounds.PlayShots("Proton-Torpedoes", 1);
                Selection.ThisShip.AnimateMunitionsShot();
            }
            else if (chosenSecondaryWeapon.Type == Upgrade.UpgradeType.Turret)
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

    public static void CancelHitsByDefenceDice(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        int crits = DiceRollAttack.CriticalSuccesses;
        DiceRollAttack.CancelHits(DiceRollDefence.Successes);
        if (crits > DiceRollAttack.CriticalSuccesses)
        {
            attacker.CallOnAtLeastOneCritWasCancelledByDefender();
            Triggers.ResolveTriggers(
                TriggerTypes.OnAtLeastOneCritWasCancelledByDefender,
                delegate
                {
                    CalculateAttackResults(attacker, defender);
                });
        }
        else
        {
            CalculateAttackResults(attacker, defender);
        }
    }

    private static void CalculateAttackResults(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        DiceRollAttack.RemoveAllFailures();

        if (DiceRollAttack.Successes > 0)
        {
            Attacker.CallOnAttackHitAsAttacker();
            Defender.CallOnAttackHitAsDefender();

            Triggers.ResolveTriggers(TriggerTypes.OnAttackHit, delegate { ResolveCombatDamage(SufferDamage); });
        }
        else
        {
            Attacker.CallOnAttackMissedAsAttacker();
            Defender.CallOnAttackMissedAsDefender();

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
            AfterAttackIsPerformed
        );
    }

    private static void AfterAttackIsPerformed()
    {
        Phases.FinishSubPhase(typeof(SubPhases.CompareResultsSubPhase));
        Attacker.CallOnAttackPerformed();
        Triggers.ResolveTriggers(TriggerTypes.OnAttackPerformed, CheckTwinAttack);
    }

    private static void CheckTwinAttack()
    {
        Upgrade.GenericSecondaryWeapon chosenSecondaryWeapon = ChosenWeapon as Upgrade.GenericSecondaryWeapon;
        if (chosenSecondaryWeapon != null && chosenSecondaryWeapon.IsTwinAttack)
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
        Selection.ThisShip = Attacker;

        Attacker.CallCombatEnd();
        Defender.CallCombatEnd();

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

    private static void CleanupCombatData()
    {
        AttackStep = CombatStep.None;
        Attacker = null;
        Defender = null;
        ChosenWeapon = null;
        ShotInfo = null;
    }

    public static void ConfirmDiceResults()
    {
        GameMode.CurrentGameMode.ConfirmDiceResults();
    }

    public static void ConfirmDiceResultsClient()
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

        public override void PrepareDecision(System.Action callBack)
        {
            List<Ship.IShipWeapon> allWeapons = GetAllWeapons();

            //TODO: Range?
            infoText = "Choose weapon for attack";

            foreach (var weapon in allWeapons)
            {
                if (weapon.IsShotAvailable(Selection.AnotherShip))
                {
                    AddDecision(weapon.Name, delegate { PerformAttackWithWeapon(weapon); });
                    AddTooltip(weapon.Name, (weapon as Upgrade.GenericSecondaryWeapon != null) ? (weapon as Upgrade.GenericSecondaryWeapon).ImageUrl : null );
                }
            }

            defaultDecision = GetDecisions().Last().Key;

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

            Combat.CurentDiceRoll = diceRoll;
            Combat.DiceRollAttack = diceRoll;

            base.CheckResults(diceRoll);
        }

        public override void Pause()
        {
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
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

            Combat.CurentDiceRoll = diceRoll;
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
            Combat.CancelHitsByDefenceDice(Selection.ThisShip, Selection.AnotherShip);
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

