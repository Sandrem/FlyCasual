using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

public enum CriticalCardType
{
    Ship,
    Pilot
}

namespace DamageDeckCard
{

    public class GenericDamageCard
    {
        public GenericShip Host;

        public string Name;
        public CriticalCardType Type;
        public bool IsFaceUp;

        public List<DieSide> CancelDiceResults = new List<DieSide>();

        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl ?? ImageUrls.GetImageUrl(this); }
            set { imageUrl = value; }
        }

        public void AssignCrit(GenericShip host)
        {
            Host = host;

            Triggers.RegisterTrigger(new Trigger() {
                Name = "Apply critical hit card effect",
                TriggerType = TriggerTypes.OnFaceupCritCardIsDealt,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = ApplyEffect
            });

            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardIsDealt, Triggers.FinishTrigger);
        }

        public virtual void ApplyEffect(object sender, EventArgs e)
        {

        }

        public virtual void DiscardEffect(GenericShip host)
        {

        }

        protected void AddCancelCritAction(GenericShip ship)
        {
            ActionsList.CancelCritAction cancelCritAction = new ActionsList.CancelCritAction();
            cancelCritAction.Initilize(this);
            ship.AddAvailableAction(cancelCritAction);
        }

    }

}

