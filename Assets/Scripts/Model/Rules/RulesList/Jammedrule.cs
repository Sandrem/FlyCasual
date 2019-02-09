using Ship;
using Tokens;
using System.Collections.Generic;
using System.Linq;
using Editions;
using System;

namespace RulesList
{
    public class JammedRule
    {
        static bool RuleIsInitialized = false;

        public JammedRule()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTokenIsAssignedGlobal += CheckJam;
                RuleIsInitialized = true;
            }
        }

        private bool IsJammableToken(Type tokenType)
        {
            // Blue target locks are jammable
            if (tokenType == typeof(BlueTargetLockToken))
            {
                return true;
            }
            // For other tokens, create a token of the type and check with the edition rules instead of checking for a fixed list of types, for future-proofing.
            var token = (GenericToken)Activator.CreateInstance(tokenType, new GenericShip());
            return Edition.Current.IsTokenCanBeDiscardedByJam(token);
        }

        private void CheckJam(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(JamToken))
            {
                if (ship.Tokens.HasGreenTokens() || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*'))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
            else
            {
                if (ship.Tokens.HasToken(typeof(JamToken)) && IsJammableToken(tokenType))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
        }

        private void RegisterJammedDecisionTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Jammed!",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = StartJammedDecisionSubPhase,
                Sender = ship
            });
        }

        private void StartJammedDecisionSubPhase(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = sender as GenericShip;

            Phases.StartTemporarySubPhaseOld(
                "Jammed",
                typeof(SubPhases.JammedDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }

    }
}

namespace SubPhases
{

    public class JammedDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select token to remove";

            DecisionOwner = Selection.ActiveShip.Owner;

            List<System.Type> tokensTypesFound = new List<System.Type>();

            foreach (GenericToken token in Selection.ActiveShip.Tokens.GetAllTokens())
            {
                if (Edition.Current.IsTokenCanBeDiscardedByJam(token))
                {
                    if (!tokensTypesFound.Contains(token.GetType()) || token.GetType() == typeof(BlueTargetLockToken))
                    {
                        string name = token.Name;
                        if (token.GetType() == typeof(BlueTargetLockToken)) name += " \"" + (token as BlueTargetLockToken).Letter + "\"";

                        AddDecision(name, delegate { RemoveJamAndToken(token); });
                    }
                }
            }

            if (GetDecisions().Count != 0)
            {
                DefaultDecisionName = GetDecisions().First().Name;
            }

            callBack();
        }

        private void RemoveJamAndToken(GenericToken token)
        {
            Selection.ActiveShip.Tokens.RemoveToken(
                token,
                RemoveJamToken
            );
        }

        private void RemoveJamToken()
        {
            Selection.ActiveShip.Tokens.RemoveToken(
                typeof(JamToken),
                Finish
            );
        }

        private void Finish()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
