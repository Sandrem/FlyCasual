using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class CancelCritAction : GenericAction
    {
        private Ship.GenericShip host;
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

        public override void ActionTake(System.Action callBack)
        {
            Selection.ActiveShip = Selection.ThisShip;

            host = Selection.ThisShip;
            if (CritCard.CancelDiceResults.Count == 0)
            {
                CritCard.DiscardEffect(host);
            }
            else
            {
                Actions.SelectedCriticalHitCard = CritCard;
                Selection.ActiveShip = Selection.ThisShip;
                Phases.StartTemporarySubPhase("Trying to flip critical card", typeof(SubPhases.CancelCritCheckSubPhase), callBack);
            }
            Phases.Next();
        }

    }

}

namespace SubPhases
{

    public class CancelCritCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            dicesType = DiceKind.Attack;
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            Selection.ActiveShip = Selection.ThisShip;
            if (Actions.SelectedCriticalHitCard.CancelDiceResults.Contains(CurrentDiceRoll.DiceList[0].Side)) Actions.SelectedCriticalHitCard.DiscardEffect(Actions.SelectedCriticalHitCard.Host);
            base.CheckResults(CurrentDiceRoll);

            Phases.FinishSubPhase(this.GetType());
            callBack();
        }

    }

}
