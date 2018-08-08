using System;
using System.Collections.Generic;
using Tokens;

namespace SubPhases
{
    public class SpendDiceResultDecisionSubPhase : DecisionSubPhase
    {
        protected void AddSpendDiceResultEffect(DieSide side, string text, EventHandler effect)
        {
            if(Combat.CurrentDiceRoll.HasResult(side))
            {
                AddDecision(text, effect);
            }
        }

        public override void PrepareDecision(Action callBack)
        {
            PrepareDiceResultEffects();
            callBack();
        }

        protected virtual void PrepareDiceResultEffects()
        {
            // implement in your subclass
        }
    }
}
