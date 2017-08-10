using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class GenericCriticalHit
    {
        protected GameManagerScript Game;

        public Ship.GenericShip Host;

        public string Name;
        public CriticalCardType Type;
        public List<DiceSide> CancelDiceResults = new List<DiceSide>();
        public string ImageUrl;

        public GenericCriticalHit()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

        public void AssignCrit(Ship.GenericShip host)
        {
            this.Host = host;
            Inform();

            Triggers.RegisterTrigger(new Trigger() {
                Name = "Apply critical hit card effect",
                TriggerType = TriggerTypes.OnFaceupCritCardIsDealt,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = ApplyEffect
            });

            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardIsDealt, Triggers.FinishTrigger);
        }

        private void Inform()
        {
            Messages.ShowInfo("Crit: " + Name);
            Game.UI.AddTestLogEntry("Crit: " + Name);
        }

        public virtual void ApplyEffect(object sender, EventArgs e)
        {

        }

        public virtual void DiscardEffect(Ship.GenericShip host)
        {

        }

        protected void AddCancelCritAction(Ship.GenericShip ship)
        {
            ActionsList.CancelCritAction cancelCritAction = new ActionsList.CancelCritAction();
            cancelCritAction.Initilize(this);
            ship.AddAvailableAction(cancelCritAction);
        }

    }

}

