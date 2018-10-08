using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using SubPhases;
using UpgradesList;
using RuleSets;
using System.Linq;
using System;

namespace UpgradesList
{

    public class ChopperAstromech : GenericUpgrade, ISecondEditionUpgrade
    {
        public ChopperAstromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "\"Chopper\"";
            Cost = 1;

            isUnique = true;

            UpgradeAbilities.Add(new ChopperAstromechAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 2;

            UpgradeAbilities.RemoveAll(a => a is ChopperAstromechAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.ChopperAstromechAbility());

            SEImageNumber = 99;
        }
    }
}

namespace Abilities
{
    public class ChopperAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += R2F2AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= R2F2AddAction;
        }

        private void R2F2AddAction(GenericShip host)
        {
            GenericAction action = new ChopperAstromechAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                DoAction = AskToDiscardAnotherUpgrade
            };
            host.AddAvailableAction(action);
        }

        private void AskToDiscardAnotherUpgrade()
        {
            var phase = Phases.StartTemporarySubPhaseNew<ChopperAstromechAUpgradeDecisionSubPhase>("\"Chopper\"'s ability", Phases.CurrentSubPhase.CallBack);
            phase.chopperUpgrade = HostUpgrade as ChopperAstromech;
            phase.Start();
        }

        public class ChopperAstromechAUpgradeDecisionSubPhase : DecisionSubPhase
        {
            public ChopperAstromech chopperUpgrade;

            public override void PrepareDecision(System.Action callBack)
            {
                InfoText = "Select upgrade to discard:";
                RequiredPlayer = chopperUpgrade.Host.Owner.PlayerNo;

                var upgrades = chopperUpgrade.Host.UpgradeBar.GetUpgradesOnlyFaceup();
                foreach (var upgrade in upgrades)
                {
                    if (upgrade != chopperUpgrade) AddDecision(upgrade.Name, (s, e) => DiscardUpgrade(upgrade), upgrade.ImageUrl);
                }

                DefaultDecisionName = upgrades[0].Name;

                DecisionViewType = DecisionViewTypes.ImagesUpgrade;

                callBack();
            }

            protected void DiscardUpgrade(GenericUpgrade upgrade)
            {
                // TODO: Chopper's sound
                upgrade.TryDiscard(RestoreShield);
            }

            private void RestoreShield()
            {
                chopperUpgrade.Host.TryRegenShields();
                ConfirmDecision();
            }
        }
    }
}

namespace ActionsList
{
    public class ChopperAstromechAction : GenericAction
    {
        public ChopperAstromechAction()
        {
            Name = DiceModificationName = "\"Chopper\": Resotore a shield";
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

            if (upgrades.Any(u => u.Charges > 0) && HostShip.Shields < HostShip.MaxShields)
            {
                ship.AddAvailableAction(new RecoverShieldAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,                    
                    Host = HostShip,
                    Source = this.HostUpgrade,
                    GetAvailableUpgrades = GetNonRecurringChargeUpgrades
                });
            }
            if (upgrades.Any(u => u.Charges < u.MaxCharges) && HostShip.Shields >= 2)
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
                .Where(u => u.UsesCharges && !u.RegensCharges)
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
                return upgrade.Charges > 0;
            }

            protected override GenericUpgrade SelectUpgradeForAI()
            {
                //prioritize upgrades with more remaining charges
                return GetAvailableUpgrades().OrderByDescending(u => u.Charges).First();
            }

            protected override void UpgradeSelected(GenericUpgrade upgrade, Action callback)
            {
                upgrade.SpendCharge();
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
                return upgrade.Charges < upgrade.MaxCharges;
            }

            protected override GenericUpgrade SelectUpgradeForAI()
            {
                //prioritize upgrades with few remaining charges
                return GetAvailableUpgrades().OrderBy(u => u.Charges).First();
            }

            protected override void UpgradeSelected(GenericUpgrade upgrade, Action callback)
            {
                Host.LoseShield();
                Host.LoseShield();
                upgrade.RestoreCharge();
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
                    decisionPhase.AddDecision(u.Name, delegate { UpgradeSelected(u, DecisionSubPhase.ConfirmDecision); });
                });

                decisionPhase.DefaultDecisionName = SelectUpgradeForAI().Name;

                decisionPhase.Start();
            }
        }
    }
}