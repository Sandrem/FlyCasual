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
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(HostShip, destroyedShip);            
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
            var phase = Phases.StartTemporarySubPhaseNew("Select upgrade to recover", typeof(SubPhases.ScavengerCraneDecisonSubPhase), Triggers.FinishTrigger) as SubPhases.ScavengerCraneDecisonSubPhase;
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

    public class ScavengerCraneDecisonSubPhase : DecisionSubPhase
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
            upgrade.TryFlipFaceUp(RollForDiscard);
            ConfirmDecision();
        }

        protected void RollForDiscard()
        {
            var phase = Phases.StartTemporarySubPhaseNew("Roll for discarding Scavenger Crane", typeof(SubPhases.ScavengerCraneDiscardCheckSubPhase), Triggers.FinishTrigger) as SubPhases.ScavengerCraneDiscardCheckSubPhase;
            phase.scanvengerCraneUpgrade = hostUpgrade;            
            phase.Start();
        }

        public override void Resume()
        {
            base.Resume();
            Initialize();

            UI.ShowSkipButton();
        }

        public override void SkipButton()
        {
            UI.HideSkipButton();            
            CallBack();
        }

    }

    public class ScavengerCraneDiscardCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade scanvengerCraneUpgrade;

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

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

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }

        private void SufferDamage()
        {
            foreach (var dice in CurrentDiceRoll.DiceList)
            {
                Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(dice);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Saturation Salvo Damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
                    Skippable = true,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Saturation Salvo",
                        DamageType = DamageTypes.CardAbility
                    }
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

    }

}