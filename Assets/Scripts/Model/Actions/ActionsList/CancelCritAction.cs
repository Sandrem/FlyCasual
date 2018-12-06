﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class CancelCritAction : GenericAction
    {
        private GenericDamageCard CritCard;

        public CancelCritAction()
        {
            IsCritCancelAction = true;
        }

        public void Initilize(GenericDamageCard critCard)
        {
            CritCard = critCard;

            string cancelType = (critCard.CancelDiceResults.Count == 0) ? "Discard" : "Try to discard";
            Name = critCard.Name + ": " + cancelType;

            ImageUrl = critCard.ImageUrl;
        }

        public override void ActionTake()
        {
            Selection.ActiveShip = Selection.ThisShip;

            HostShip = Selection.ThisShip;
            if (CritCard.CancelDiceResults.Count == 0)
            {
                CritCard.DiscardEffect();
                Phases.FinishSubPhase(typeof(SubPhases.CancelCritCheckSubPhase));
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                ActionsHolder.SelectedCriticalHitCard = CritCard;
                Selection.ActiveShip = Selection.ThisShip;
                Phases.StartTemporarySubPhaseOld(
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
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            Selection.ActiveShip = Selection.ThisShip;
            if (ActionsHolder.SelectedCriticalHitCard.CancelDiceResults.Contains(CurrentDiceRoll.DiceList[0].Side)) ActionsHolder.SelectedCriticalHitCard.DiscardEffect();

            CallBack();
        }

    }

}
