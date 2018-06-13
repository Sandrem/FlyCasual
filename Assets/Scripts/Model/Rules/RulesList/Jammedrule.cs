using Ship;
using Tokens;
using System.Collections.Generic;
using System.Linq;
using RuleSets;

namespace RulesList
{
    public class JammedRule
    {

        public JammedRule()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckJam;
        }

        private void CheckJam(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(JamToken))
            {
                if (ship.Tokens.HasToken(typeof(FocusToken)) || ship.Tokens.HasToken(typeof(EvadeToken)) || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*'))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
            else
            {
                if (ship.Tokens.HasToken(typeof(JamToken)))
                {
                    List<System.Type> tokensTypesToDiscard = new List<System.Type> { typeof(FocusToken), typeof(EvadeToken), typeof(BlueTargetLockToken) };
                    if (tokensTypesToDiscard.Contains(tokenType)) RegisterJammedDecisionTrigger(ship);
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
                if (RuleSet.Instance.IsTokenCanBeDiscardedByJam(token))
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
