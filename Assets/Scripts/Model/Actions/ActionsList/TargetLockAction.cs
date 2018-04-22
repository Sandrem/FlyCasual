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
            Name = EffectName = "Target Lock";

            TokensSpend.Add(typeof(Tokens.BlueTargetLockToken));
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                char letter = ' ';
                letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

                if (Combat.Attacker.Tokens.GetToken(typeof(Tokens.BlueTargetLockToken), letter).CanBeUsed)
                {
                    DiceRerollManager diceRerollManager = new DiceRerollManager()
                    {
                        CallBack = callBack
                    };

                    Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), diceRerollManager.Start, letter);
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

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender) != ' ')
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
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
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.TargetLockAction));

            Phases.FinishSubPhase(this.GetType());
            Phases.CurrentSubPhase.Resume();
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
                GetAiPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                AbilityName,
                Description,
                ImageUrl
            );
        }

        private bool FilterTargetLockTargets(GenericShip ship)
        {
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship);
            return ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && distanceInfo.Range >= Selection.ThisShip.TargetLockMinRange && distanceInfo.Range <= Selection.ThisShip.TargetLockMaxRange && Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, ship);
        }

        private int GetTargetLockAiPriority(GenericShip ship)
        {
            int result = 0;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, ship);
            if (shotInfo.InShotAngle) result += 1000;
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
