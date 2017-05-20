using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class GenericToken
    {
        protected GameManagerScript Game;

        public string Name;
        public bool Temporary = true;
        public ActionsList.GenericAction Action = null;
        public int Count = 1;

        public GenericToken() {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

        public virtual void GetAvailableEffects(ref List<ActionsList.GenericAction> availableActionEffects)
        {
            if (Action!=null) availableActionEffects.Add(Action);
        }

    }

}
