using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class TargetLockAction : GenericAction
    {
        public TargetLockAction() {
            Name = EffectName = "Target Lock";
            IsReroll = true;
        }

        public override void ActionEffect()
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                char letter = ' ';
                letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

                Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
                Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

                //TODO: 2 Kinds of reroll
                //TODO: Block buttons
                Dices.RerollDices(Combat.CurentDiceRoll, "failures", Unblock);
            }
        }

        private void Unblock(DiceRoll diceRoll)
        {
            //Todo: Unblock buttons
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

        public override void ActionTake()
        {
            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhase("Select target for Target Lock", typeof(SubPhases.SelectTargetLockSubPhase));
        }

    }

}

namespace SubPhases
{

    public class SelectTargetLockSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isEnemyAllowed = true;
            finishAction = TrySelectTargetLock;
        }

        private void TrySelectTargetLock()
        {
            if (!Actions.AssignTargetLockToPair(Selection.ThisShip, TargetShip))
            {
                RevertSubPhase();
            }
        }

        protected override void RevertSubPhase()
        {
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.TargetLockAction));
            base.RevertSubPhase();
            Actions.ShowActionsPanel();
        }

    }

}
