using Upgrade;
using Ship;
using System.Collections.Generic;
using ActionsList;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class ChopperAstromech : GenericUpgrade
    {
        public ChopperAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Chopper\"",
                UpgradeType.Astromech,
                cost: 1,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.ChopperAstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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
            phase.chopperUpgrade = HostUpgrade as UpgradesList.FirstEdition.ChopperAstromech;
            phase.Start();
        }

        public class ChopperAstromechAUpgradeDecisionSubPhase : DecisionSubPhase
        {
            public UpgradesList.FirstEdition.ChopperAstromech chopperUpgrade;

            public override void PrepareDecision(System.Action callBack)
            {
                InfoText = "Select upgrade to discard:";
                RequiredPlayer = chopperUpgrade.HostShip.Owner.PlayerNo;

                var upgrades = chopperUpgrade.HostShip.UpgradeBar.GetUpgradesOnlyFaceup();
                foreach (var upgrade in upgrades)
                {
                    if (upgrade != chopperUpgrade) AddDecision(upgrade.UpgradeInfo.Name, (s, e) => DiscardUpgrade(upgrade), upgrade.ImageUrl);
                }

                DefaultDecisionName = upgrades[0].UpgradeInfo.Name;

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
                chopperUpgrade.HostShip.TryRegenShields();
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
            Name = DiceModificationName = "\"Chopper\": Restore a shield";
        }
    }
}