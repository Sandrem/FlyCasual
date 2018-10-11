using BoardTools;
using RuleSets;
using RulesList;
using Ship;
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
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                List<char> letters = Actions.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);

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
                if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
                {
                    result = true;
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
            Phases.StartTemporarySubPhaseOld(
                "Select target for Target Lock",
                typeof(SubPhases.SelectTargetLockActionSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class SelectTargetLockActionSubPhase : AcquireTargetLockSubPhase
    {
        public override void RevertSubPhase()
        {
            RuleSet.Instance.ActionIsFailed(TheShip, typeof(ActionsList.TargetLockAction));
            UpdateHelpInfo();
        }

        public override void SkipButton()
        {
            RevertSubPhase();
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
            CanMeasureRangeBeforeSelection = false;

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

            ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, ship, Selection.ThisShip.PrimaryWeapon);
            if (shotInfo.IsShotAvailable) result += 1000;
            if (!ship.ShipsBumped.Contains(Selection.ThisShip)) result += 500;
            if (shotInfo.Range <= 3) result += 250;

            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

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
                Actions.AcquireTargetLock(
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
