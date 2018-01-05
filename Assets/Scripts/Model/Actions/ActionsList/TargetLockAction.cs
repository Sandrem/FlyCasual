using RulesList;
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
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                char letter = ' ';
                letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

                if (Combat.Attacker.GetToken(typeof(Tokens.BlueTargetLockToken), letter).CanBeUsed)
                {
                    DiceRerollManager diceRerollManager = new DiceRerollManager()
                    {
                        CallBack = callBack
                    };

                    Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken), diceRerollManager.Start, letter);
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
                typeof(SubPhases.SelectTargetLockSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class SelectTargetLockSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            CanMeasureRangeBeforeSelection = false;

            var ship = Selection.ThisShip;
            minRange = ship.TargetLockMinRange;
            maxRange = ship.TargetLockMaxRange;

            targetsAllowed.Add(TargetTypes.Enemy);
            finishAction = TrySelectTargetLock;

            UI.ShowSkipButton();
        }

        private void TrySelectTargetLock()
        {
            if (Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, TargetShip))
            {
                Actions.AssignTargetLockToPair(
                    Selection.ThisShip,
                    TargetShip,
                    delegate
                    {
                        UI.HideSkipButton();
                        Phases.FinishSubPhase(typeof(SelectTargetLockSubPhase));
                        CallBack();
                    },
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
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.TargetLockAction));
            base.RevertSubPhase();
        }

        public override void SkipButton()
        {
            RevertSubPhase();
        }

    }

}
