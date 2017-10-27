﻿using RulesList;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionsList
{

    public class TargetLockAction : GenericAction
    {
        public TargetLockAction() {
            Name = EffectName = "Target Lock";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager()
                {
                    CallBack = callBack
                };

                char letter = ' ';
                letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

                Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken), diceRerollManager.Start, letter);
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
            Phases.StartTemporarySubPhase(
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
            var ship = Selection.ThisShip;
            minRange = ship.TargetLockMinRange;
            maxRange = ship.TargetLockMaxRange;

            isEnemyAllowed = true;
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

        protected override void RevertSubPhase()
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
