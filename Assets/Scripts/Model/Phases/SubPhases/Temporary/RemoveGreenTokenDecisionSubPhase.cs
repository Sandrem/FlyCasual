using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
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
                AddDecision(
                    "Discard " + kv.Key.ToLower(),
                    delegate
                    {
                        Messages.ShowInfo(Selection.ThisShip.PilotInfo.PilotName + " discarded " + kv.Key.ToLower());
                        Selection.ThisShip.Tokens.RemoveToken(
                            kv.Value.GetType(),
                            delegate { AfterTokenIsDiscarded(); }
                        );
                    }
                );
            }

            PrepareCustomDecisions();

            DefaultDecisionName = decisions.First().Name;

            callBack();
        }

        protected virtual void AfterTokenIsDiscarded()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        public virtual void PrepareCustomDecisions()
        {
            // implement in your subclass
        }
    }

    public class SpendGreenTokenDecisionSubPhase : DecisionSubPhase
    {
        public GenericShip HostShip;

        public override void PrepareDecision(Action callBack)
        {
            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>();

            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (token.TokenColor != TokenColors.Green)
                    continue;

                if (tokens.ContainsKey(token.Name))
                    continue;

                tokens[token.Name] = token;
            }

            foreach (KeyValuePair<string, GenericToken> kv in tokens)
            {
                AddDecision(
                    "Spend " + kv.Key.ToLower(),
                    delegate {
                        Messages.ShowInfo(HostShip.PilotInfo.PilotName + " spent " + kv.Key.ToLower());
                        HostShip.Tokens.SpendToken(
                            kv.Value.GetType(),
                            DecisionSubPhase.ConfirmDecision
                        );
                    }
                );
            }

            DefaultDecisionName = GetDecisions().First().Name;

            PrepareCustomDecisions();
            callBack();
        }

        public virtual void PrepareCustomDecisions()
        {
            // implement in your subclass
        }
    }

    public class RemoveBadTokenFromDefenderDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>(); 

            foreach(GenericToken token in Combat.Defender.Tokens.GetAllTokens())
            {
                if (token.TokenColor != TokenColors.Orange && token.TokenColor != TokenColors.Red)
                    continue;

                if (tokens.ContainsKey(token.Name))
                    continue;

                string tokenName = token.Name.ToLower() + ((token is RedTargetLockToken) ? $" \"{(token as RedTargetLockToken).Letter}\"" : "");
                tokens[tokenName] = token;
            }

            foreach(KeyValuePair<string, GenericToken> kv in tokens)
            {
                AddDecision(
                    "Remove " + kv.Key,
                    delegate {
                        Messages.ShowInfo($"{kv.Key} is removed from {Combat.Defender.PilotInfo.PilotName}");
                        Combat.Defender.Tokens.RemoveToken(
                            kv.Value,
                            DecisionSubPhase.ConfirmDecision
                        );
                    }
                );
            }

            DefaultDecisionName = decisions.First().Name;

            ShowSkipButton = false;

            callBack();
        }
    }
}
