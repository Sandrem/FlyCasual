using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using System;

namespace UpgradesList.SecondEdition
{
    public class CoaxiumHyperfuel : GenericUpgrade
    {
        public CoaxiumHyperfuel() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Coaxium Hyperfuel",
                UpgradeType.Illicit,
                cost: 2,
                restriction: new ActionBarRestriction(typeof(SlamAction)),
                abilityType: typeof(Abilities.SecondEdition.CoaxiumHyperfuelAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6e/de/6ede85e0-1f8b-459d-be5d-34cb56cf301c/swz63_coaxium-hyperfuel.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //You can perform the slam action even while stressed. If you do, you suffer 1 crit damage unless you expose 1 of your damage cards.
    //After you partially execute a maneuver, you may expose 1 of your damage cards or suffer 1 crit damage to perform a slam action.
    public class CoaxiumHyperfuelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(SlamAction));

            HostShip.BeforeActionIsPerformed += RegisterSlamDamageTrigger;
            HostShip.OnMovementFinishUnsuccessfully += RegisterActionTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(SlamAction));

            HostShip.BeforeActionIsPerformed -= RegisterSlamDamageTrigger;
            HostShip.OnMovementFinishUnsuccessfully -= RegisterActionTrigger;
        }

        private void RegisterSlamDamageTrigger(GenericAction action, ref bool isFree)
        {
            if (action is SlamAction && HostShip.IsStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskDamageDecision);
            }
        }

        private void RegisterActionTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskSlamAction);
        }

        private void AskSlamAction(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSlamActionDamageTrigger; 
            HostShip.AskPerformFreeAction(                
                new SlamAction(true),
                delegate
                {
                    HostShip.BeforeActionIsPerformed -= RegisterSlamActionDamageTrigger;
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "You may expose 1 of your damage cards or suffer 1 critical damage to perform a slam action",
                HostShip
            );
        }

        private void RegisterSlamActionDamageTrigger(GenericAction action, ref bool isFree)
        {
            if (action is SlamAction)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskDamageDecision);
            }
        }

        private void AskDamageDecision(object sender, EventArgs e)
        {
            if (HostShip.Damage.HasFacedownCards)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        SufferDamage();
                    },
                    delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        ExposeDamageCard();
                    },
                    descriptionLong: "Do you want to suffer critical damage instead of exposing 1 damage card?",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                SufferDamage();
            }
        }

        private void ExposeDamageCard()
        {
            Messages.ShowInfo($"{HostName} Random damage card is exposed");
            HostShip.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
        }

        private void SufferDamage()
        {
            Messages.ShowInfo($"{HostName}: Critical damage is suffered");
            HostShip.Damage.TryResolveDamage(
                0,
                new DamageSourceEventArgs()
                {
                    DamageType = DamageTypes.CardAbility,
                    Source = HostUpgrade
                },
                Triggers.FinishTrigger,
                1
            );
        }
    }
}