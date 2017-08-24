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
        public bool CanBeUsed = true;
        public int Count = 1;
        public string Tooltip;

        public GenericToken() {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

        public virtual ActionsList.GenericAction GetAvailableEffects()
        {
            ActionsList.GenericAction result = null;
            if ((Action != null) && (CanBeUsed))
            {
                result = Action;
            }
            return result;
        }

    }

}
