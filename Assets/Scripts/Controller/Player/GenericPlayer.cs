using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class GenericPlayer
    {
        protected GameManagerScript Game;

        public GenericPlayer(int id) {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

        public virtual void SetupShip() { }

        public virtual void AssignManeuver() { }

        public virtual void PerformManeuver() { }

        public virtual void PerformAction() { }

        public virtual void PerformFreeAction() { }

        public virtual void PerformAttack() { }

        public virtual void UseDiceModifications() { }
    }

}
