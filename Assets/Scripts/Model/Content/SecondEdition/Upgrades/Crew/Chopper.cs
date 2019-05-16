using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;

namespace UpgradesList.SecondEdition
{
    public class ChopperCrew : GenericUpgrade
    {
        public ChopperCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Chopper\" (crew)",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.ChopperCrewAbility),
                seImageNumber: 83
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ChopperCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnPerformActionStepStart += CheckPeformActionStep;
            HostShip.OnActionIsPerformed += ChecksAbilityDamage;
            HostShip.OnActionIsSkipped += TurnOffAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnPerformActionStepStart -= CheckPeformActionStep;
            HostShip.OnActionIsPerformed -= ChecksAbilityDamage;
            HostShip.OnActionIsSkipped -= TurnOffAbility;
        }

        private void ChecksAbilityDamage(GenericAction action)
        {
            if (HostShip.CanPerformActionsWhileStressed && HostShip.IsStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ResolveOwnAbility);
            }
            TurnOffAbility(HostShip);
        }

        private void TurnOffAbility(GenericShip ship)
        {
            HostShip.CanPerformActionsWhileStressed = false;
        }

        private void ResolveOwnAbility(object sender, EventArgs e)
        {
            if (HostShip.Damage.HasFacedownCards)
            {
                AskToUseAbility(
                    AlwaysUseByDefault,
                    delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        SufferDamage();
                    },
                    delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        ExposeDamageCard();
                    },
                    infoText: "\"Chopper\": Do you want to suffer damage instead of exposing of damage card?"
                );
            }
            else
            {
                SufferDamage();
            }
        }

        private void ExposeDamageCard()
        {
            Messages.ShowInfo("\"Chopper\": Random damage card is exposed");
            HostShip.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
        }

        private void SufferDamage()
        {
            Messages.ShowInfo("\"Chopper\": Damage is suffered");
            HostShip.Damage.TryResolveDamage(
                1,
                new DamageSourceEventArgs()
                {
                    DamageType = DamageTypes.CardAbility,
                    Source = HostUpgrade
                },
                Triggers.FinishTrigger
            );
        }

        private void CheckPeformActionStep()
        {
            if (HostShip.IsStressed)
            {
                Messages.ShowInfo("\"Chopper\": You may perform 1 action, even while stressed");
                HostShip.CanPerformActionsWhileStressed = true;
            }
        }
    }
}