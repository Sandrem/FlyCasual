using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class GenericCriticalHit
    {
        protected GameManagerScript Game;

        protected Ship.GenericShip host;

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
            this.host = host;
            Inform();
            ApplyEffect(host);
        }

        private void Inform()
        {
            Game.UI.ShowInfo("Crit: " + Name);
            Game.UI.AddTestLogEntry("Crit: " + Name);
        }

        public virtual void ApplyEffect(Ship.GenericShip host)
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

