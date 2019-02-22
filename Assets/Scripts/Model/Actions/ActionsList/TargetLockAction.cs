﻿using Actions;
using ActionsList;
using BoardTools;
using Editions;
using RulesList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                    Messages.ShowErrorToHuman("Cannot use current Target Lock on defender");
                    callBack();
                }
            }
            else
            {
                Messages.ShowErrorToHuman("No Target Lock on defender");
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
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
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
            return ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, ship);
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
