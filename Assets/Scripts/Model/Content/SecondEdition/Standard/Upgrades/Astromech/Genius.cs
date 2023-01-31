using Upgrade;
using SubPhases;
using Bombs;
using System.Collections.Generic;
using Ship;
using System;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Genius : GenericUpgrade
    {
        public Genius() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Genius\"",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GeniusAbility),
                restriction: new FactionRestriction(Faction.Scum),                
                seImageNumber: 143
            );
        }
    }
}

namespace Abilities.SecondEdition
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
            if (!BombsManager.HasBombsToDrop(ship, UpgradeSubType.Bomb)) return;
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskUseGeniusAbility);
        }

        private void AskUseGeniusAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseGeniusAbility,
                descriptionLong: "Do you want to drop a bomb?",
                imageHolder: HostUpgrade
            );
        }

        private void UseGeniusAbility(object sender, EventArgs e)
        {
            List<GenericUpgrade> timedBombsInstalled = BombsManager.GetBombsToDrop(HostShip, UpgradeSubType.Bomb);
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

            foreach (var timedBombInstalled in BombsManager.GetBombsToDrop(HostShip, UpgradeSubType.Bomb))
            {
                selectBombToDrop.AddDecision(
                    timedBombInstalled.UpgradeInfo.Name,
                    delegate { SelectBomb(timedBombInstalled); }
                );
            }

            selectBombToDrop.DescriptionShort = "\"Genius\"";
            selectBombToDrop.DescriptionLong = "Select a device to drop";
            selectBombToDrop.ImageSource = HostUpgrade;

            selectBombToDrop.DefaultDecisionName = BombsManager.GetBombsToDrop(HostShip, UpgradeSubType.Bomb).First().UpgradeInfo.Name;

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
                Triggers.FinishTrigger
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