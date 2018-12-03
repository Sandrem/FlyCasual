using Upgrade;
using Ship;
using System.Collections.Generic;
using ActionsList;
using SubPhases;
using System.Linq;
using System;

namespace UpgradesList.SecondEdition
{
    public class ChopperAstromech : GenericUpgrade
    {
        public ChopperAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Chopper\"",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ChopperAstromechAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                seImageNumber: 99
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //Action: Spend 1 non-recurring charge from another equipped upgrade to recover 1 shield.
    //Action: Spend 2 shields to recover 1 non-recurring charge on an equipped upgrade.
    public class ChopperAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddActions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddActions;
        }

        private void AddActions(GenericShip ship)
        {
            var upgrades = GetNonRecurringChargeUpgrades();

            if (upgrades.Any(u => u.State.Charges > 0) && HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
            {
                ship.AddAvailableAction(new RecoverShieldAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip,
                    Source = this.HostUpgrade,
                    GetAvailableUpgrades = GetNonRecurringChargeUpgrades
                });
            }
            if (upgrades.Any(u => u.State.Charges < u.State.MaxCharges) && HostShip.State.ShieldsCurrent >= 2)
            {
                ship.AddAvailableAction(new RecoverChargeAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip,
                    Source = this.HostUpgrade,
                    GetAvailableUpgrades = GetNonRecurringChargeUpgrades
                });
            }
        }

        private IEnumerable<GenericUpgrade> GetNonRecurringChargeUpgrades()
        {
            return HostShip
                .UpgradeBar
                .GetUpgradesAll()
                .Where(u => u.State.UsesCharges && !u.UpgradeInfo.RegensCharges)
                .ToList();
        }

        private class RecoverShieldAction : ChopperActionBase
        {
            public RecoverShieldAction()
            {
                Name = "Chopper: Recover 1 shield";
                InfoText = "Select which upgrade should spend a charge";
            }

            protected override bool IsUsable(GenericUpgrade upgrade)
            {
                return upgrade.State.Charges > 0;
            }

            protected override GenericUpgrade SelectUpgradeForAI()
            {
                //prioritize upgrades with more remaining charges
                return GetAvailableUpgrades().OrderByDescending(u => u.State.Charges).First();
            }

            protected override void UpgradeSelected(GenericUpgrade upgrade, Action callback)
            {
                upgrade.State.SpendCharge();
                Host.TryRegenShields();
                callback();
            }
        }

        private class RecoverChargeAction : ChopperActionBase
        {
            public RecoverChargeAction()
            {
                Name = "Chopper: Recover 1 charge";
                InfoText = "Select upgrade to recover charge";
            }

            protected override bool IsUsable(GenericUpgrade upgrade)
            {
                return upgrade.State.Charges < upgrade.State.MaxCharges;
            }

            protected override GenericUpgrade SelectUpgradeForAI()
            {
                //prioritize upgrades with few remaining charges
                return GetAvailableUpgrades().OrderBy(u => u.State.Charges).First();
            }

            protected override void UpgradeSelected(GenericUpgrade upgrade, Action callback)
            {
                Host.LoseShield();
                Host.LoseShield();
                upgrade.State.RestoreCharge();
                callback();
            }
        }

        public abstract class ChopperActionBase : GenericAction
        {
            protected string InfoText { get; set; }
            public Func<IEnumerable<GenericUpgrade>> GetAvailableUpgrades { get; set; }

            protected abstract bool IsUsable(GenericUpgrade upgrade);

            protected abstract void UpgradeSelected(GenericUpgrade upgrade, Action callback);

            protected abstract GenericUpgrade SelectUpgradeForAI();

            public override void ActionTake()
            {
                var decisionPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    DecisionSubPhase.ConfirmDecision
                );

                decisionPhase.InfoText = InfoText;

                GetAvailableUpgrades().Where(IsUsable).ToList().ForEach(u =>
                {
                    decisionPhase.AddDecision(u.UpgradeInfo.Name, delegate { UpgradeSelected(u, DecisionSubPhase.ConfirmDecision); });
                });

                decisionPhase.DefaultDecisionName = SelectUpgradeForAI().UpgradeInfo.Name;

                decisionPhase.Start();
            }
        }
    }
}