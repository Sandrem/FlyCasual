using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class GenericAction
    {
        public string Name;
        public string EffectName;
        public string ImageUrl;

        public bool IsCritCancelAction;

        public bool IsSpendFocus;
        public bool IsSpendEvade;
        public bool IsSpendTargetLock;

        public bool IsReroll;

        public bool IsTurnsOneFocusIntoSuccess;
        public bool IsTurnsAllFocusIntoSuccess;

        public bool IsOpposite;

        public Ship.GenericShip Host;
        public Upgrade.GenericUpgrade Source;

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
            * 110 - Free add dice with value
            * 100 - Free change limited by side if 1
            * 95 - Free rerolls limited by side
            * 90 - Free rerolls
            * 85 - Reroll of all rerollable dice if bad result
            * 80 - Rerolls TL
            * 70 - Free focus to crit
            * 60 - Free focus to crit, another to hits
            * 55 - Free side to hit
            * 50 - Regular Focus to hits if 1+
            * 40 - Regular Focus to hits if 1
            * 30 - Reroll of focus is focus token is available
            * 20 - Hits to crits
            */

            /* DEFENCE
            * 110 - Free add dice with value
            * 100 - Free change limited by side if 1
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

        public virtual bool IsActionAvailable()
        {
            return true;
        }

        public virtual int GetActionPriority()
        {
            int result = 0;

            /*
            * 
            * 90 - Cancel crit
            * 50 - Focus action if has target
            * 40 - Evade action
            * 20 - Focus action if no target
            * 
            */

            return result;
        }

    }

}
