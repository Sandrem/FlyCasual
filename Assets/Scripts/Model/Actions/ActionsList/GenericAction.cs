using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Upgrade;
using System;

namespace ActionsList
{
    public enum DiceModificationTimingType
    {
        Normal,
        Opposite,
        CompareResults
    }

    public class GenericAction
    {
        public string Name;
        public string EffectName;
        public string ImageUrl;

        public bool IsCritCancelAction;

        public List<System.Type> TokensSpend = new List<System.Type>();

        public bool IsReroll;

        public bool IsTurnsOneFocusIntoSuccess;
        public bool IsTurnsAllFocusIntoSuccess;

        public DiceModificationTimingType DiceModificationTiming = DiceModificationTimingType.Normal;

        public bool CanBePerformedWhileStressed;
        public virtual bool CanBePerformedAsAFreeAction
        {
            get
            {
                return true;
            }
        }

        private GenericShip host;
        public GenericShip Host
        {
            get
            {
                if (host == null) Console.Write(Name + " tries to get Host value, but it was not set", LogTypes.Errors, true, "red");
                return host;
            }
            set { host = value; }
        }

        private GenericUpgrade source;
        public GenericUpgrade Source
        {
            get
            {
                if (source == null) Console.Write(Name + " tries to get Source value, but it was not set", LogTypes.Errors, true, "red");
                return source;
            }
            set { source = value; }
        }

        public Action DoAction = delegate { };
        public Action<Action> DoActionEffect = delegate { };

        public virtual void ActionEffect(Action callBack)
        {
            DoActionEffect(callBack);
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
            * 65 - Not free Evade if 1 uncancelled
            * 50 - Regular Focus to evades if 1+
            * 40 - Regular Focus to evades if 1
            * 20 - Regular Evade if >1 uncancelled
            * 15 - Not free Evade if >1 uncancelled
            */

            return result;
        }

        public virtual void ActionTake()
        {
            DoAction();
        }

        public virtual bool IsActionAvailable()
        {
            return true;
        }

        public virtual int GetActionPriority()
        {
            int result = 0;

            /*
            * 90 - Cancel crit
            * 50 - Focus action if has target
            * 40 - Evade action
            * 25 - Reinforce action if there are no enemies
            * 20 - Focus action if no target
            * 10 - Focus action if Expertise is installed
            */

            return result;
        }

    }

}
