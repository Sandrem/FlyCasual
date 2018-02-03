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
        public bool IsFaceup;
        public int DamageValue { get; protected set; }

        public List<DieSide> CancelDiceResults = new List<DieSide>();

        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl ?? ImageUrls.GetImageUrl(this); }
            set { imageUrl = value; }
        }

        public GenericDamageCard()
        {
            DamageValue = 1;
        }

        public void Assign(GenericShip host)
        {
            Host = host;

            if (IsFaceup)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Apply critical hit card effect",
                    TriggerType = TriggerTypes.OnFaceupCritCardIsDealt,
                    TriggerOwner = host.Owner.PlayerNo,
                    EventHandler = ApplyEffect
                });

                Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardIsDealt, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public virtual void ApplyEffect(object sender, EventArgs e)
        {

        }

        public virtual void DiscardEffect()
        {

        }

        protected void CallAddCancelCritAction(GenericShip ship)
        {
            AddCancelCritAction();
        }

        protected void AddCancelCritAction()
        {
            ActionsList.CancelCritAction cancelCritAction = new ActionsList.CancelCritAction();
            cancelCritAction.Initilize(this);
            Host.AddAvailableAction(cancelCritAction);
        }

    }

}

