using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Upgrade;
using System;
using SubPhases;

public enum DiceModificationTimingType
{
    AfterRolled,
    Opposite,
    Normal,
    CompareResults
}

namespace ActionsList
{
    public class GenericAction
    {
        public string Name;
        public string DiceModificationName;
        public string ImageUrl;

        public bool IsRed;

        public bool IsCritCancelAction;

        public List<System.Type> TokensSpend = new List<System.Type>();

        public bool IsReroll;

        public bool IsTurnsOneFocusIntoSuccess;
        public bool IsTurnsAllFocusIntoSuccess;

        public bool CanBeUsedFewTimes;

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
        public GenericShip HostShip
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

        public GenericAction AsRedAction
        {
            get
            {
                var redAction = (GenericAction)MemberwiseClone();
                redAction.IsRed = true;
                return redAction;
            }
        }

        public bool IsInActionBar;

        public Action DoAction = delegate { };
        public Action<Action> DoDiceModification = delegate { };
        public Func<bool> CheckDiceModificationAvailable = delegate { return true; };
        public Func<int> GenerateDiceModificationAiPriority = delegate { return 0; };

        public virtual void ActionEffect(Action callBack)
        {
            DoDiceModification(callBack);
        }

        public virtual bool IsDiceModificationAvailable()
        {
            return CheckDiceModificationAvailable();
        }

        public virtual int GetDiceModificationPriority()
        {
            int result = GenerateDiceModificationAiPriority();

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
            * 53 - Spend die with eye result
            * 50 - Regular Focus to hits if 1+
            * 45 - Force to change eye to evade if 1
            * 41 - Calculate
            * 40 - Regular Focus to hits if 1
            * 35 - Force to change eye to evade if 1+
            * 33 - Eye to hit by dealing damage to teammate
            * 30 - Reroll of focus is focus token is available
            * 20 - Hits to crits
            */

            /* DEFENCE
            * 110 - Free add dice with value
            * 100 - Free change limited by side if 1
            * 95 - Free rerolls limited by side
            * 90 - Free rerolls
            * 85 - Not-Free rerolls
            * 80 - Free focus to evades
            * 70 - Regular Evade if 1 uncancelled
            * 65 - Not free Evade if 1 uncancelled
            * 50 - Regular Focus to evades if 1+
            * 45 - Force to change eye to evade if 1
            * 40 - Regular Focus to evades if 1
            * 35 - Force to change eye to evade if 1+
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
            * 100 - Rotate arc to get a shot in no enemies in arc
            * 90 - Cancel crit
            * 50 - Focus action if has target
            * 40 - Evade action
            * 25 - Reinforce action if there are no enemies
            * 20 - Focus action if no target
            * 10 - Focus action if Expertise is installed
            */

            return result;
        }

        public void DoOnlyEffect(Action callback)
        {
            HostShip = Selection.ThisShip;
            Phases.StartTemporarySubPhaseNew<ActionEffectSubPhase>(
                "Action effect",
                delegate
                {
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    callback();
                }
            );
            ActionTake();
        }

        public virtual void RevertActionOnFail()
        {
            HostShip.RemoveAlreadyExecutedAction(this.GetType());
        }

    }

}

namespace SubPhases
{
    public class ActionEffectSubPhase : GenericSubPhase
    {
        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
        }
    }
}
