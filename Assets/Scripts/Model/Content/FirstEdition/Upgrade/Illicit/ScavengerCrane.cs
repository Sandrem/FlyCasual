using Ship;
using Upgrade;
using System;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class ScavengerCrane : GenericUpgrade
    {
        public ScavengerCrane() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Scavenger Crane",
                UpgradeType.Illicit,
                2,
                abilityType: typeof(Abilities.FirstEdition.ScavengerCraneAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ScavengerCraneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDestroyedGlobal += RegisterScavengerCraneAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDestroyedGlobal -= RegisterScavengerCraneAbility;
        }

        private void RegisterScavengerCraneAbility(GenericShip destroyedShip, bool isFled)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, destroyedShip);
            if (distanceInfo.Range <= 2)
            {
                var recoverableUpgrades = GetRecoverableUpgrades();
                if (recoverableUpgrades.Length > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskUseScavengerCrane);
                }
            }
        }

        protected void AskUseScavengerCrane(object sender, EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<SubPhases.ScavengerCraneDecisionSubPhase>("Select upgrade to recover", Triggers.FinishTrigger);
            phase.hostUpgrade = HostUpgrade;
            phase.hostAbility = this;
            phase.Start();
        }

        public GenericUpgrade[] GetRecoverableUpgrades()
        {
            var allowedTypes = new[] { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Bomb, UpgradeType.Cannon, UpgradeType.Turret, UpgradeType.Modification };
            var discardedUpgrades = HostShip.UpgradeBar.GetUpgradesOnlyDiscarded()
                .Where(upgrade => allowedTypes.Any(type => upgrade.HasType(type)))
                .ToArray();
            return discardedUpgrades;
        }
    }
}


namespace SubPhases
{

    public class ScavengerCraneDecisionSubPhase : DecisionSubPhase
    {
        public GenericUpgrade hostUpgrade;
        public Abilities.FirstEdition.ScavengerCraneAbility hostAbility;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select upgrade to recover";
            var discardedUpgrades = hostAbility.GetRecoverableUpgrades();

            foreach (var upgrade in discardedUpgrades)
            {
                var thisUpgrade = upgrade;
                AddDecision(upgrade.UpgradeInfo.Name, (s, e) => RecoverUpgrade(thisUpgrade));
                AddTooltip(upgrade.UpgradeInfo.Name, upgrade.ImageUrl);
            }

            AddDecision("None", (s, e) => ConfirmDecision());
            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        protected void RecoverUpgrade(GenericUpgrade upgrade)
        {
            ConfirmDecisionNoCallback();
            upgrade.TryFlipFaceUp(RollForDiscard);
        }

        protected void RollForDiscard()
        {
            var phase = Phases.StartTemporarySubPhaseNew<ScavengerCraneRollSubPhase>("Roll for discarding Scavenger Crane", () =>
            {
                Phases.FinishSubPhase(typeof(ScavengerCraneRollSubPhase));
                Triggers.FinishTrigger();
            });
            phase.scanvengerCraneUpgrade = hostUpgrade;
            phase.Start();
        }
    }

    public class ScavengerCraneRollSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade scanvengerCraneUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;
            Selection.ActiveShip = scanvengerCraneUpgrade.Host;
            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Blank)
            {
                Messages.ShowInfoToHuman(string.Format("{0} has to discard Scavenger Crane", scanvengerCraneUpgrade.Host.PilotName));
                scanvengerCraneUpgrade.TryDiscard(CallBack);
            }
            else
            {
                CallBack();
            }
        }
    }

}