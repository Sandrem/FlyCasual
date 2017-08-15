using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class GenericAction
    {
        protected GameManagerScript Game;

        public string Name;
        public string EffectName;
        public string ImageUrl;

        public bool IsCritCancelAction = false;
        public bool IsSpendFocus = false;
        public bool IsReroll = false;

        public GenericAction() {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

        public virtual void ActionEffect()
        {

        }

        public virtual bool IsActionEffectAvailable()
        {
            return true;
        }

        public virtual int GetActionEffectPriority()
        {
            int result = 0;

            /* ATTACK
             * 90 - Free rerolls
             * 80 - Rerolls TL
             * 
             * 70 - Focus to crits
             * 50 - Focus to hits if 1+
             * 40 - Focus to hits if 1
             */

            /* DEFENCE
             * 90 - Free rerolls
             * 
             * 70 - Evade if 1 uncancelled
             * 50 - Focus to evades if 1+
             * 40 - Focus to evades if 1
             * 30 - Evade if >1 uncancelled
             */

            return result;
        }

        public virtual void ActionTake()
        {

        }

    }

}
