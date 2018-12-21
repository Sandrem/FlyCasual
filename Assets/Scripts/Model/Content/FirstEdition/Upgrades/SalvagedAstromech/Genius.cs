using System.Collections;
using System.Collections.Generic;
using Upgrade;
using System;
using Ship;
using SubPhases;
using Bombs;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Genius : GenericUpgrade
    {
        public Genius() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Genius\"",
                UpgradeType.SalvagedAstromech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.GeniusAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GeniusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckGeniusAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckGeniusAbility;
        }

        private void CheckGeniusAbility(GenericShip ship)
        {
            if (HostShip.IsBumped) return;
            if (HostShip.IsBombAlreadyDropped) return;
            if (!BombsManager.HasBombsToDrop(ship)) return;
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskUseGeniusAbility);
        }

        private void AskUseGeniusAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseGeniusAbility);
        }

        private void UseGeniusAbility(object sender, EventArgs e)
        {
            List<GenericUpgrade> timedBombsInstalled = BombsManager.GetBombsToDrop(HostShip);
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (timedBombsInstalled.Count == 1)
            {
                BombsManager.CurrentBomb = timedBombsInstalled[0] as GenericBomb;
                StartDropBombSubphase();
            }
            else
            {
                AskToSelectTimedBomb(StartDropBombSubphase);
            }
        }

        private void AskToSelectTimedBomb(Action callback)
        {
            GeniusBombDecisionSubPhase selectBombToDrop = (GeniusBombDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select bomb to drop",
                typeof(GeniusBombDecisionSubPhase),
                callback
            );

            foreach (var timedBombInstalled in BombsManager.GetBombsToDrop(HostShip))
            {
                selectBombToDrop.AddDecision(
                    timedBombInstalled.UpgradeInfo.Name,
                    delegate { SelectBomb(timedBombInstalled); }
                );
            }

            selectBombToDrop.InfoText = "Select bomb to drop";

            selectBombToDrop.DefaultDecisionName = BombsManager.GetBombsToDrop(HostShip).First().UpgradeInfo.Name;

            selectBombToDrop.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private void SelectBomb(GenericUpgrade timedBombUpgrade)
        {
            BombsManager.CurrentBomb = timedBombUpgrade as GenericTimedBomb;
            DecisionSubPhase.ConfirmDecision();
        }

        private class GeniusBombDecisionSubPhase : DecisionSubPhase { }

        protected virtual void StartDropBombSubphase()
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                CheckThatBombIsDiscarded
            );
        }

        private void CheckThatBombIsDiscarded()
        {
            if (BombsManager.CurrentBomb == null || BombsManager.CurrentBomb.IsDiscardedAfterDropped)
            {
                Triggers.FinishTrigger();
            }
            else
            {
                BombsManager.CurrentBomb.TryDiscard(Triggers.FinishTrigger);
            }
        }
    }
}