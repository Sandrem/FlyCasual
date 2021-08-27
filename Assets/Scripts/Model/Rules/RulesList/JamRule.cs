using Ship;
using Tokens;
using System.Collections.Generic;
using System.Linq;
using Editions;
using System;
using Players;

namespace RulesList
{
    public enum JamIsNotAllowedReasons
    {
        NotInRange,
        FriendlyShip
    }

    public class JamRule
    {
        static bool RuleIsInitialized = false;

        public delegate void EventHandlerListJamIsNotAllowedReasons2Ships(ref List<JamIsNotAllowedReasons> blockReasons, GenericShip jamSource, GenericShip jamTarget);
        public static event EventHandlerListJamIsNotAllowedReasons2Ships OnCheckJamIsAllowed;
        public static event EventHandlerListJamIsNotAllowedReasons2Ships OnCheckJamIsDisallowed;

        public JamRule()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTokenIsAssignedGlobal += CheckJam;
                RuleIsInitialized = true;
            }
        }

        public bool JamIsAllowed(GenericShip jamSource, GenericShip jamTarget)
        {
            List<JamIsNotAllowedReasons> blockReasons = new List<JamIsNotAllowedReasons>();

            if (Tools.IsSameTeam(jamSource, jamTarget))
            {
                blockReasons.Add(JamIsNotAllowedReasons.FriendlyShip);
            }

            int rangeBetween = jamTarget.GetRangeToShip(jamSource);
            bool isInBullseyeArc = jamSource.SectorsInfo.IsShipInSector(jamTarget, Arcs.ArcType.Bullseye);
            if (rangeBetween == 1 ||
                (rangeBetween == 2 && isInBullseyeArc))
            {
                // November 2020 rule changes
                // Target is allowed
            }
            else
            {
                blockReasons.Add(JamIsNotAllowedReasons.NotInRange);
            }
            
            if (blockReasons.Count > 0) OnCheckJamIsAllowed?.Invoke(ref blockReasons, jamSource, jamTarget);
            if (blockReasons.Count == 0) OnCheckJamIsDisallowed?.Invoke(ref blockReasons, jamSource, jamTarget);

            return blockReasons.Count == 0;
        }

        private bool IsJammableToken(Type tokenType)
        {
            // Blue target locks are jammable
            if (tokenType == typeof(BlueTargetLockToken))
            {
                return true;
            }
            //Tractor beam tokens are not jammable, and they have no constructor with only one paramter so the call to Activator.CreateInstance() below crashes
            if (tokenType == typeof(TractorBeamToken))
            {
                return false;
            }
            // For other tokens, create a token of the type and check with the edition rules instead of checking for a fixed list of types, for future-proofing.
            var token = (GenericToken)Activator.CreateInstance(tokenType, new object[] { null });
            return Edition.Current.IsTokenCanBeDiscardedByJam(token);
        }

        private void CheckJam(GenericShip ship, GenericToken token)
        {
            if (token is JamToken)
            {
                if (ship.Tokens.HasGreenTokens || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*'))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
            else
            {
                if (ship.Tokens.HasToken(typeof(JamToken)) && IsJammableToken(token.GetType()))
                {
                    RegisterJammedDecisionTrigger(ship);
                }
            }
        }

        private void RegisterJammedDecisionTrigger(GenericShip ship)
        {
            JamToken jamToken = (JamToken)ship.Tokens.GetToken(typeof(JamToken));

            SubPhases.JammedDecisionSubPhase newPhase = (SubPhases.JammedDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Jammed",
                typeof(SubPhases.JammedDecisionSubPhase),
                Triggers.FinishTrigger
            );
            newPhase.Assigner = jamToken.Assigner;

            Triggers.RegisterTrigger(new Trigger() {
                Name = "Jammed!",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = (Edition.Current is SecondEdition) ? jamToken.Assigner.PlayerNo: ship.Owner.PlayerNo,
                EventHandler = delegate {
                    Selection.ActiveShip = ship;
                    newPhase.Start();
                }
            });
        }
    }
}

namespace SubPhases
{

    public class JammedDecisionSubPhase : DecisionSubPhase
    {
        public GenericPlayer Assigner;

        public override void PrepareDecision(System.Action callBack)
        {
            DescriptionShort = "Jammed: Select a token to remove";

            DecisionOwner = (Edition.Current is Editions.SecondEdition) ? Assigner : Selection.ActiveShip.Owner;

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
