using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using SubPhases;
using UpgradesList;

namespace UpgradesList
{

    public class ChopperAstromech : GenericUpgrade
    {
        public ChopperAstromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "\"Chopper\"";
            Cost = 1;

            isUnique = true;

            UpgradeAbilities.Add(new ChopperAstromechAbility());
        }
    }

}

namespace Abilities
{
    public class ChopperAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += R2F2AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= R2F2AddAction;
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

                DecisionViewType = DecisionViewTypes.ImageButtons;

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
            Name = EffectName = "\"Chopper\": Resotore a shield";
        }
    }
}