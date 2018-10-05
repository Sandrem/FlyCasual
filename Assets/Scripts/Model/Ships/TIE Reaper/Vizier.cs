using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class Vizier : TIEReaper, ISecondEditionPilot
        {
            public Vizier() : base()
            {
                PilotName = "\"Vizier\"";
                PilotSkill = 3;
                Cost = 23;

                IsUnique = true;

                PilotAbilities.Add(new VizierAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 45;

                PilotAbilities.RemoveAll(ability => ability is VizierAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.VizierAbilitySE());

                SEImageNumber = 115;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VizierAbilitySE : GenericAbility
    {
        private bool RestrictedAbilityIsActivated;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.GrantedBy == "Ailerons")
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToPerformCoordinate);
            }
        }

        private void AskToPerformCoordinate(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("\"Vizier\" can perform a Coordinate action");

            RestrictedAbilityIsActivated = true;
            HostShip.OnActionIsPerformed += CheckActionRestriction;
            HostShip.OnMovementStart += ClearRestrictedAbility;

            HostShip.AskPerformFreeAction(new CoordinateAction() { Host = HostShip }, Triggers.FinishTrigger);
        }

        private void CheckActionRestriction(GenericAction action)
        {
            if (action is CoordinateAction && RestrictedAbilityIsActivated)
            {
                Messages.ShowError("\"Vizier\" skips Perform Action step");
                HostShip.IsSkipsActionSubPhase = true;
            }
        }

        private void ClearRestrictedAbility(GenericShip ship)
        {
            HostShip.OnMovementStart -= ClearRestrictedAbility;
            HostShip.OnActionIsPerformed -= CheckActionRestriction;

            RestrictedAbilityIsActivated = false;
        }
    }
}

namespace Abilities
{
    public class VizierAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckVizierAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckVizierAbility;
        }

        private void CheckVizierAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo 
                && ship.AssignedManeuver.Speed == 1
                && !ship.IsBumped 
                && (HostShip.Tokens.HasToken<FocusToken>() || HostShip.Tokens.HasToken<EvadeToken>())
                && BoardTools.Board.GetRangeOfShips(HostShip, ship) <=1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, (s,e) => AskVizierAbility(ship));
                //Triggers.RegisterTrigger(new Trigger()
                //{
                //    Name = "\"Vizier\" ability",
                //    TriggerType = TriggerTypes.OnShipMovementFinish,
                //    TriggerOwner = HostShip.Owner.PlayerNo,
                //    EventHandler = AskVizierAbility,
                //    Sender = HostUpgrade,
                //});
            }
        }

        private void AskVizierAbility(GenericShip target)
        {
            var newSubPhase = Phases.StartTemporarySubPhaseNew<VizierDecisionSubPhase>("\"Vizier\" decision", Triggers.FinishTrigger);
            newSubPhase.AddDecision("None", (s, e) => DontPassToken());
            if (HostShip.Tokens.HasToken<EvadeToken>())
            {
                newSubPhase.AddDecision("Evade token", (s, e) => PassToken(new EvadeToken(target)));
            }
            if (HostShip.Tokens.HasToken<FocusToken>())
            {
                newSubPhase.AddDecision("Focus token", (s, e) => PassToken(new FocusToken(target)));
            }
            newSubPhase.ShowSkipButton = false;            
            newSubPhase.InfoText = string.Format("{0} may pass a token to {1}", HostShip.PilotName, target.PilotName);
            var aiDecisionIndex = AiDecideToPassToken(target) ? 1 : 0;
            newSubPhase.DefaultDecisionName = newSubPhase.GetDecisions()[aiDecisionIndex].Name;
            newSubPhase.Start();
        }

        private bool AiDecideToPassToken(GenericShip target)
        {
            var myWorth = HostShip.Cost + HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Sum(u => u.Cost);
            var targetWorth = target.Cost + target.UpgradeBar.GetUpgradesOnlyFaceup().Sum(u => u.Cost);
            var targetNeedsToken = !target.Tokens.HasToken<FocusToken>() || !target.Tokens.HasToken<EvadeToken>();            
            return targetWorth > myWorth && targetNeedsToken;
        }

        private void DontPassToken()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void PassToken(GenericToken tokenToPass)
        {
            var target = tokenToPass.Host;            
            var tokenToRemove = HostShip.Tokens.GetToken(tokenToPass.GetType());
            HostShip.Tokens.RemoveToken(tokenToRemove, () => target.Tokens.AssignToken(tokenToPass, DecisionSubPhase.ConfirmDecision));
        }

    }
}

namespace SubPhases
{

    public class VizierDecisionSubPhase : DecisionSubPhase
    { }
}