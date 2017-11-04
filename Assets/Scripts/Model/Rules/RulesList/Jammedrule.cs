using Ship;
using Tokens;
using System.Collections.Generic;
using System.Linq;

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
                if (ship.HasToken(typeof(FocusToken)) || ship.HasToken(typeof(EvadeToken)) || ship.HasToken(typeof(BlueTargetLockToken), '*'))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
            else
            {
                if (ship.HasToken(typeof(JamToken)))
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

            List<System.Type> tokensTypesToDiscard = new List<System.Type> { typeof(FocusToken), typeof(EvadeToken), typeof(BlueTargetLockToken) };
            List<System.Type> tokensTypesFound = new List<System.Type>();

            foreach (var token in Selection.ActiveShip.GetAllTokens())
            {
                if (tokensTypesToDiscard.Contains(token.GetType()))
                {
                    if (!tokensTypesFound.Contains(token.GetType()) || token.GetType() == typeof(BlueTargetLockToken))
                    {
                        string name = token.Name;
                        if (token.GetType() == typeof(BlueTargetLockToken)) name += " \"" + (token as BlueTargetLockToken).Letter + "\"";

                        AddDecision(name, delegate { RemoveToken(token); });
                    }
                }
            }

            if (GetDecisions().Count != 0)
            {
                DefaultDecision = GetDecisions().First().Key;
            }

            callBack();
        }

        private void RemoveToken(GenericToken token)
        {
            Selection.ActiveShip.RemoveToken(token.GetType(), (token.GetType() == typeof(BlueTargetLockToken)) ? (token as BlueTargetLockToken).Letter : ' ');
            Selection.ActiveShip.RemoveToken(typeof(JamToken));

            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
