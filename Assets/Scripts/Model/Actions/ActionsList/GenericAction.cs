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

        public virtual void ActionEffect(System.Action callBack)
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
            * 100 - Free change limited by side if 1
            * 95 - Free rerolls limited by side
            * 90 - Free rerolls
            * 80 - Rerolls TL
            * 70 - Free focus to crit
            * 60 - Free focus to crit, another to hits
            * 55 - Free side to hit
            * 50 - Regular Focus to hits if 1+
            * 40 - Regular Focus to hits if 1
            * 20 - Hits to crits
            */

            /* DEFENCE
            * 95 - Free rerolls limited by side
            * 90 - Free rerolls
            * 80 - Free focus to evades
            * 70 - Regular Evade if 1 uncancelled
            * 50 - Regular Focus to evades if 1+
            * 40 - Regular Focus to evades if 1
            * 20 - Regular Evade if >1 uncancelled
            */

            return result;
        }

        public virtual void ActionTake()
        {

        }

    }

}
