using System;
using System.Collections.Generic;
using Tokens;

namespace SubPhases
{
    public class RemoveBadTokenDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>(); 

            foreach(GenericToken token in Selection.ThisShip.Tokens.GetAllTokens())
            {
                if (token.TokenColor == TokenColors.Orange || token.TokenColor == TokenColors.Red) tokens[token.Name] = token;
            }

            foreach(KeyValuePair<string, GenericToken> kv in tokens)
            {
                AddDecision(
                    "Discard " + kv.Key.ToLower(),
                    delegate {
                        Selection.ThisShip.Tokens.RemoveToken(
                            kv.Value.GetType(),
                            DecisionSubPhase.ConfirmDecision
                        );
                    }
                );
            }

            PrepareCustomDecisions();
            callBack();
        }

        public virtual void PrepareCustomDecisions()
        {
            // implement in your subclass
        }
    }

    public class RemoveRedTokenDecisionSubPhase : DecisionSubPhase
    {
        public bool RemoveOnlyNonLocks = false;

        public override void PrepareDecision(Action callBack)
        {
            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>();

            foreach (GenericToken token in Selection.ThisShip.Tokens.GetAllTokens())
            {
                if (token.TokenColor == TokenColors.Red && CanRemoveLockTokens(token))
                {
                    string tokenName = token.Name;
                    if (token is RedTargetLockToken) tokenName += " \"" + (token as RedTargetLockToken).Letter + "\"";
                    tokens[tokenName] = token;
                }
            }

            foreach (KeyValuePair<string, GenericToken> kv in tokens)
            {
                AddDecision(
                    "Discard " + kv.Key.ToLower(),
                    delegate {
                        Selection.ThisShip.Tokens.RemoveToken(
                            kv.Value,
                            DoCustomFinishDecision
                        );
                    }
                );
            }

            AddDecision("None", delegate { DecisionSubPhase.ConfirmDecision(); });

            PrepareCustomDecisions();
            callBack();
        }

        private bool CanRemoveLockTokens(GenericToken token)
        {
            return !RemoveOnlyNonLocks || !(token is RedTargetLockToken);
        }

        public virtual void DoCustomFinishDecision()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        public virtual void PrepareCustomDecisions()
        {
            // implement in your subclass
        }
    }
}
