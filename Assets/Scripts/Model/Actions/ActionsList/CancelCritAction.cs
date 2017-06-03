using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class CancelCritAction : ActionsList.GenericAction
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
            host = Selection.ThisShip;
            if (CritCard.CancelDiceResults.Count == 0)
            {
                CritCard.DiscardEffect(host);
            }
            else
            {
                //Throw dice for results
            }
        }

    }

}
