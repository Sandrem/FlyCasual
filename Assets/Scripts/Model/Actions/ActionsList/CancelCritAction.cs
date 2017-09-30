﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class CancelCritAction : GenericAction
    {
        private CriticalHitCard.GenericCriticalHit CritCard;

        public CancelCritAction()
        {
            IsCritCancelAction = true;
        }

        public void Initilize(CriticalHitCard.GenericCriticalHit critCard)
        {
            CritCard = critCard;

            string cancelType = (critCard.CancelDiceResults.Count == 0) ? "Discard" : "Try to discard";
            Name = critCard.Name + ": " + cancelType;

            ImageUrl = critCard.ImageUrl;
        }

        public override void ActionTake()
        {
            Selection.ActiveShip = Selection.ThisShip;

            Host = Selection.ThisShip;
            if (CritCard.CancelDiceResults.Count == 0)
            {
                CritCard.DiscardEffect(Host);
                Phases.FinishSubPhase(typeof(SubPhases.CancelCritCheckSubPhase));
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Actions.SelectedCriticalHitCard = CritCard;
                Selection.ActiveShip = Selection.ThisShip;
                Phases.StartTemporarySubPhase(
                    "Trying to flip critical card",
                    typeof(SubPhases.CancelCritCheckSubPhase),
                    delegate {
                        Phases.FinishSubPhase(typeof(SubPhases.CancelCritCheckSubPhase));
                        Phases.CurrentSubPhase.CallBack();
                    });
            }
        }

        public override int GetActionPriority()
        {
            int result = 0;
            result = 90;
            return result;
        }

    }

}

namespace SubPhases
{

    public class CancelCritCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            Selection.ActiveShip = Selection.ThisShip;
            if (Actions.SelectedCriticalHitCard.CancelDiceResults.Contains(CurrentDiceRoll.DiceList[0].Side)) Actions.SelectedCriticalHitCard.DiscardEffect(Actions.SelectedCriticalHitCard.Host);

            CallBack();
        }

    }

}
