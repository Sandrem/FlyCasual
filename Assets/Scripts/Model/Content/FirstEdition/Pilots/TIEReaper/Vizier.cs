using Ship;
using SubPhases;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace FirstEdition.TIEReaper
    {
        public class Vizier : TIEReaper
        {
            public Vizier() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Vizier\"",
                    3,
                    23,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.VizierAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
                && BoardTools.Board.GetRangeOfShips(HostShip, ship) <= 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, (s, e) => AskVizierAbility(ship));
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
            var myWorth = HostShip.PilotInfo.Cost + HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Sum(u => u.UpgradeInfo.Cost);
            var targetWorth = target.PilotInfo.Cost + target.UpgradeBar.GetUpgradesOnlyFaceup().Sum(u => u.UpgradeInfo.Cost);
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
    public class VizierDecisionSubPhase : DecisionSubPhase { }
}