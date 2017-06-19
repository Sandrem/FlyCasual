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

        public override void ActionTake()
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
                Phases.StartTemporarySubPhase("Trying to flip critical card", typeof(SubPhases.CancelCritCheckSubPhase));
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
            dicesType = "attack";
            dicesCount = 1;

            checkResults = CheckResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            if (Actions.SelectedCriticalHitCard.CancelDiceResults.Contains(diceRoll.DiceList[0].Side)) Actions.SelectedCriticalHitCard.DiscardEffect(Actions.SelectedCriticalHitCard.Host);

            base.CheckResults(diceRoll);
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

    }

}
