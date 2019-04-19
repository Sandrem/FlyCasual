using Actions;
using ActionsList;
using BoardTools;
using Editions;
using RulesList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace ActionsList
{

    public class TargetLockAction : GenericAction
    {
        public TargetLockAction()
        {
            Name = DiceModificationName = "Target Lock";

            TokensSpend.Add(typeof(Tokens.BlueTargetLockToken));
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);

                if (Combat.Attacker.Tokens.GetToken(typeof(Tokens.BlueTargetLockToken), letters.First()).CanBeUsed)
                {
                    DiceRerollManager diceRerollManager = new DiceRerollManager()
                    {
                        CallBack = callBack
                    };

                    Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), diceRerollManager.Start, letters.First());
                }
                else
                {
                    Messages.ShowErrorToHuman("The attacker cannot use its current Target Lock on the defender.");
                    callBack();
                }
            }
            else
            {
                Messages.ShowErrorToHuman("The attacker has no Target Lock on the defender.");
                callBack();
            }
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
                {
                    result = !Combat.DiceRollAttack.IsEmpty;
                }
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.FocusesNotRerolled;
                int attackBlanks = Combat.CurrentDiceRoll.BlanksNotRerolled;
                int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));


                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (numFocusTokens > 0)
                {
                    if (attackBlanks > 0) result = 80;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 80;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            SelectTargetLockActionSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectTargetLockActionSubPhase>(
                "Select target for Target Lock",
                Phases.CurrentSubPhase.CallBack
            );
            subphase.HostAction = this;
            subphase.Start();
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                UI.ShowSkipButton();
                UI.HighlightSkipButton();
            }
            else
            {
                Phases.GoBack();
            }
        }
        public override int GetActionPriority()
        {
            int result = 0;

            result = (ActionsHolder.HasTarget(Selection.ThisShip)) ? 40 : 0;

            int maxOrdinanceRange = -1;
            int minOrdinanceRange = 99;
            int minShipTargetRange = 1;
            int curOrdinanceMax = -1;
            int curOrdinanceMin = -1;
            int numTargetLockTargets = 0;
            bool validTargetLockedAlready = false;
            if (Selection.ThisShip.ShipInfo.ShipName == "E-wing")
            {
                minShipTargetRange = 2;
            }

            // Find the combined maximum and minimum range of all of our ordinance that currently has charges.
            foreach (GenericUpgrade currentUpgrade in Selection.ThisShip.UpgradeBar.GetUpgradesOnlyFaceup())
            {
                if (currentUpgrade.HasType(UpgradeType.Missile) || currentUpgrade.HasType(UpgradeType.Torpedo) && currentUpgrade.State.Charges > 0)
                {
                    if (currentUpgrade.UpgradeInfo.WeaponInfo.RequiresToken == typeof(BlueTargetLockToken))
                    {
                        curOrdinanceMax = currentUpgrade.UpgradeInfo.WeaponInfo.MaxRange;
                        curOrdinanceMin = currentUpgrade.UpgradeInfo.WeaponInfo.MinRange;

                        if (curOrdinanceMin < minOrdinanceRange && curOrdinanceMin >= minShipTargetRange)
                        {
                            minOrdinanceRange = curOrdinanceMin;
                        }
                        if (curOrdinanceMax > maxOrdinanceRange)
                        {
                            maxOrdinanceRange = curOrdinanceMax;
                        }
                    }
                }
            }
            // If our minimum range is less than 99, we have ordinance that is loaded and have set our min and max ranges.
            // Check all enemy ships to see if they are in range of our ordinance.
            if (minOrdinanceRange < 99)
            {
                foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(Selection.ThisShip.Owner.PlayerNo)).Ships)
                {
                    ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, anotherShip.Value, Selection.ThisShip.PrimaryWeapons);
                    if ((shotInfo.Range <= maxOrdinanceRange) && (shotInfo.Range >= minOrdinanceRange) && (shotInfo.IsShotAvailable))
                    {
                        if (!ActionsHolder.HasTargetLockOn(Selection.ThisShip, anotherShip.Value))
                        {
                            // We have a target in range that doesn't have a target lock on it from us.
                            numTargetLockTargets++;
                        }
                        else
                        {
                            // We already have a target in range that has our target lock on it.
                            validTargetLockedAlready = true;
                        }
                    }
                }
                if (validTargetLockedAlready == false && numTargetLockTargets > 0)
                {
                    // We have ordinance, we have targets for that ordinance, and none of them have our target lock on them.
                    result += 15;
                }
            }


            if (Selection.ThisShip.State.Force > 1 && result == 40)
            {
                // We have at least 2 Force and we haven't already decided to possibly perform a target lock action.
                validTargetLockedAlready = false;
                numTargetLockTargets = 0;
                // Jedi with 2 or more Force should target lock more often than they focus.
                foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(Selection.ThisShip.Owner.PlayerNo)).Ships)
                {
                    ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, anotherShip.Value, Selection.ThisShip.PrimaryWeapons);
                    if ((shotInfo.Range < 4) && (shotInfo.IsShotAvailable))
                    {
                        if (!ActionsHolder.HasTargetLockOn(Selection.ThisShip, anotherShip.Value))
                        {
                            // We have a target in range that doesn't have a target lock on it from us.
                            numTargetLockTargets++;
                        }
                        else
                        {
                            // We already have a target in range that has our target lock on it.
                            validTargetLockedAlready = true;
                        }
                    }
                }
                if (validTargetLockedAlready == false && numTargetLockTargets > 0)
                {
                    // We don't already have a target that is in range and locked, and we have targets available.
                    result += 15;
                }
            }

            return result;
        }
    }

}

namespace SubPhases
{

    public class SelectTargetLockActionSubPhase : AcquireTargetLockSubPhase
    {
        public GenericAction HostAction { get; set; }

        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
        }

        protected override void SuccessfulCallback()
        {
            Phases.FinishSubPhase(typeof(SelectTargetLockActionSubPhase));
            base.SuccessfulCallback();
        }
    }

    public class AcquireTargetLockSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            CanMeasureRangeBeforeSelection = (Edition.Current is Editions.SecondEdition);

            if (AbilityName == null) AbilityName = "Target Lock";
            if (Description == null) Description = "Choose a ship to acquire a target lock on it";

            PrepareByParameters(
                TrySelectTargetLock,
                FilterTargetLockTargets,
                GetTargetLockAiPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                AbilityName,
                Description,
                ImageSource
            );
        }

        private bool FilterTargetLockTargets(GenericShip ship)
        {
            // Don't include targets that are owned by the target locking player, ships that can't get target locks, or ships that already have a target lock from this ship.
            // Without the last test, Redline can target lock the same target twice.
            return (ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, ship) && !ActionsHolder.HasTargetLockOn(Selection.ThisShip, ship));
        }

        private int GetTargetLockAiPriority(GenericShip ship)
        {
            int result = 0;

            ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, ship, Selection.ThisShip.PrimaryWeapons);
            if (shotInfo.IsShotAvailable) result += 1000;
            if (!ship.ShipsBumped.Contains(Selection.ThisShip)) result += 500;
            if (shotInfo.Range <= 3) result += 250;

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        protected virtual void SuccessfulCallback()
        {
            UI.HideSkipButton();
            CallBack();
        }

        protected virtual void TrySelectTargetLock()
        {
            if (Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, TargetShip))
            {
                ActionsHolder.AcquireTargetLock(
                    Selection.ThisShip,
                    TargetShip,
                    SuccessfulCallback,
                    RevertSubPhase
                );
            }
            else
            {
                RevertSubPhase();
            }
        }

        public override void RevertSubPhase()
        {

        }

        public override void SkipButton()
        {
            CallBack();
        }

    }

}
