using Ship;
using Upgrade;
using Abilities;
using System.Linq;
using System;

namespace UpgradesList
{
    public class ScavengerCrane : GenericUpgrade
    {
        public ScavengerCrane() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Scavenger Crane";
            Cost = 2;

            UpgradeAbilities.Add(new ScavengerCraneAbility());
        }
    }
}

namespace Abilities
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
            Board.ShipDistanceInfo distanceInfo = new Board.ShipDistanceInfo(HostShip, destroyedShip);            
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
                .Where(upgrade => allowedTypes.Any(type => upgrade.hasType(type)))
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
        public ScavengerCraneAbility hostAbility;

        public override void PrepareDecision(System.Action callBack)
        {            
            InfoText = "Select upgrade to recover";
            var discardedUpgrades = hostAbility.GetRecoverableUpgrades();
                        
            foreach (var upgrade in discardedUpgrades)
            {
                var thisUpgrade = upgrade;
                AddDecision(upgrade.Name, (s, e) => RecoverUpgrade(thisUpgrade));
                AddTooltip(upgrade.Name, upgrade.ImageUrl);
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
            diceType = DiceKind.Attack;
            diceCount = 1;
            Selection.ActiveShip = scanvengerCraneUpgrade.Host;
            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if(CurrentDiceRoll.DiceList[0].Side == DieSide.Blank)
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