﻿using System;
using System.Collections.Generic;
using Tokens;

namespace SubPhases
{
    public class RemoveGreenTokenDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>(); 

            foreach(GenericToken token in Selection.ThisShip.Tokens.GetAllTokens())
            {
                if (token.TokenColor != TokenColors.Green)
                    continue;

                if (tokens.ContainsKey(token.Name))
                    continue;

                tokens[token.Name] = token;
            }

            foreach(KeyValuePair<string, GenericToken> kv in tokens)
            {
                AddDecision("Discard " + kv.Key.ToLower() + ".", delegate { Selection.ThisShip.Tokens.RemoveToken(kv.Value.GetType(), DecisionSubPhase.ConfirmDecision); });
            }

            PrepareCustomDecisions();
            callBack();
        }

        public virtual void PrepareCustomDecisions()
        {
            // implement in your subclass
        }
    }
}
