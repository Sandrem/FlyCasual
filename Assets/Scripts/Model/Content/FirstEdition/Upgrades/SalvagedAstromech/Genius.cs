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
        
        protected virtual string AbilityDescription => "Do you want to discard one of your equipped Bomb Upgrade cards without the \"Action:\" header to drop the corresponding bomb token?";

        protected virtual UpgradeSubType BombTypeRestriction => UpgradeSubType.None;

        private void CheckGeniusAbility(GenericShip ship)
        {
            if (HostShip.IsBumped) return;
            if (HostShip.IsBombAlreadyDropped) return;
            if (!BombsManager.HasBombsToDrop(ship, BombTypeRestriction)) return;
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskUseGeniusAbility);
        }

        private void AskUseGeniusAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseGeniusAbility,
                descriptionLong: AbilityDescription,
                imageHolder: HostUpgrade
            );
        }

        private void UseGeniusAbility(object sender, EventArgs e)
        {
            List<GenericUpgrade> timedBombsInstalled = BombsManager.GetBombsToDrop(HostShip, BombTypeRestriction);
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (timedBombsInstalled.Count == 1)
            {
                BombsManager.CurrentDevice = timedBombsInstalled[0] as GenericBomb;
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

            foreach (var timedBombInstalled in BombsManager.GetBombsToDrop(HostShip, BombTypeRestriction))
            {
                selectBombToDrop.AddDecision(
                    timedBombInstalled.UpgradeInfo.Name,
                    delegate { SelectBomb(timedBombInstalled); }
                );
            }

            selectBombToDrop.DescriptionShort = "\"Genius\"";
            selectBombToDrop.DescriptionLong = "Select a device to drop";
            selectBombToDrop.ImageSource = HostUpgrade;

            selectBombToDrop.DefaultDecisionName = BombsManager.GetBombsToDrop(HostShip, BombTypeRestriction).First().UpgradeInfo.Name;

            selectBombToDrop.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private void SelectBomb(GenericUpgrade timedBombUpgrade)
        {
            BombsManager.CurrentDevice = timedBombUpgrade as GenericTimedBomb;
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
            if (BombsManager.CurrentDevice == null || (BombsManager.CurrentDevice as GenericBomb).IsDiscardedAfterDropped)
            {
                Triggers.FinishTrigger();
            }
            else
            {
                BombsManager.CurrentDevice.TryDiscard(Triggers.FinishTrigger);
            }
        }
    }
}